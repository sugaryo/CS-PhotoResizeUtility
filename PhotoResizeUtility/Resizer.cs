using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using Mode = System.Drawing.Drawing2D.InterpolationMode;
using System.Drawing.Imaging;
using System.Threading;

namespace PhotoResizer
{
	public class Resizer
	{
		public enum NoticeType
		{
			Ignore,
			Skiped,
			Collision,
			Resized,
		}


		#region 処理の通知構造
		// Noticeプロパティのデリゲート型
		public delegate void NoticeAction( NoticeType notice, FileInfo src, FileInfo dst );

		// Noticeプロパティとバッキングフィールド
		private NoticeAction notice = DefaultNoticeAction;
		public NoticeAction Notice
		{
			// null 代入の場合は NOP で置き換える。
			set { this.notice = value ?? NopAction; }
		}
		private static void NopAction( NoticeType notice, FileInfo src, FileInfo dst )
		{
			// NOP は何もしない。
		}
		private static void DefaultNoticeAction( NoticeType notice, FileInfo src, FileInfo dst )
		{
			// デフォルトのログアクション（標準出力）
			Action<string, string> log = 
				(tag, msg)=>
				{
					string message = $"- {tag.PadLeft(16)}:\t{msg}";
					Console.WriteLine( message );
				};

			// 通知タイプごとにログ内容を切り替え
			switch ( notice )
			{
				case NoticeType.Resized:
					log( "resized", src?.FullName + " -> " + dst?.FullName );
					break;

				case NoticeType.Ignore:
					log( "ignored", src?.FullName );
					break;

				case NoticeType.Collision:
					log( "collision", src?.FullName );
					break;

				case NoticeType.Skiped:
					log( "skiped", src?.FullName );
					break;
					
				default:
					break;
			}
		}
		#endregion
		
		#region リサイズに関する設定

		public Mode Mode { get; set; } = Mode.HighQualityBicubic;

		public int UnitSize { get; set; } = 4;

		#endregion

		#region その他、動作のオプション設定群
		
		public string OutputPath { get; set; } = null;

		public bool MultipleExtension { get; set; } = true;

		public bool ModeExtension { get; set; } = false;

		public bool OverWrite { get; set; } = false;

		public bool IgnoreScaledExtensionFile { get; set; } = true;

		#endregion



		public void Resize( IEnumerable<FileInfo> files, double scale )
		{
			foreach ( var file in files )
			{
				this.Resize( file, scale );
			}
		}
		
		public void Resize( FileInfo file, double scale )
		{
			// ".scaled" 複合拡張子のファイルを無視する設定の場合、拡張子をチェックして無視する。
			if ( this.IgnoreScaledExtensionFile
				  && this.IsScaledExtensionFile( file ) )
			{
				this.notice( NoticeType.Ignore, file, null );
				return;
			}

			// 設定に応じて保存パスを構築。
			string path = this.AsSavePath( file );
			FileInfo save = new FileInfo( path );

			// 同一パスになった場合は衝突エラー。
			if ( file.FullName == save.FullName )
			{
				this.notice( NoticeType.Collision, file, null );
				return;
			}

			// 既に同名のファイルが出力先に存在しており、上書きモードがオフの場合はスキップする。
			if ( save.Exists && !this.OverWrite )
			{
				this.notice( NoticeType.Skiped, file, null );
				return;
			}


			this.ResizeCore( file, save, scale );

			this.notice( NoticeType.Resized, file, save );
		}
		
		private bool IsScaledExtensionFile( FileInfo file )
		{
			List<string> extensions = file.Name
				.ToLower()    // ファイル名全体を一旦小文字化。
				.split( "." ) // 拡張子を含むファイル名全体をドットで分割。
				.Skip( 1 )    // 先頭の要素（ファイル名本体）をスキップ。
				.ToList();    // スキップして残った拡張子（複数）をリスト化。

			// 複合拡張子の中に scaled が含まれるか否かを判定。
			return extensions.Contains( "scaled" );
		}

		#region 画像リサイズのメイン処理

		private void ResizeCore( FileInfo file, FileInfo save, double scale )
		{
			// 画像のリサイズ処理を行い、出力先パスに png ファイルを保存する。
			using ( Bitmap bmp = new Bitmap( file.FullName ) )
			{
				int u = this.UnitSize;
				int w = bmp.Width.scale( scale ).unit( u );
				int h = bmp.Height.scale( scale ).unit( u );

				using ( Bitmap resized = new Bitmap( w, h ) )
				{
					using ( Graphics g = Graphics.FromImage( resized ) )
					{
						g.InterpolationMode = this.Mode;
						g.DrawImage( bmp, 0, 0, w, h );
					}

					resized.Save( save.FullName, ImageFormat.Png );
				}
			}
		}

		#endregion



		#region 出力先パスの構築処理
		
		private string SaveExtension
		{
			get
			{
				// 複合拡張子指定がオフの場合は、ImageFormatに合わせて png 拡張子だけを返す。
				if ( !this.MultipleExtension )
				{
					return ".png";
				}

				// モード別拡張子がオンの場合は、Modeごとに複合拡張子にメタ情報を入れる。
				if ( this.ModeExtension )
				{
					switch ( this.Mode )
					{
						case Mode.Invalid:
							return ".scaled" + ".inv" + ".png";

						case Mode.Default:
							return ".scaled" + ".def" + ".png";

						case Mode.Low:
							return ".scaled" + ".lo" + ".png";

						case Mode.High:
							return ".scaled" + ".hi" + ".png";

						case Mode.Bilinear:
							return ".scaled" + ".bl" + ".png";

						case Mode.Bicubic:
							return ".scaled" + ".bc" + ".png";

						case Mode.HighQualityBilinear:
							return ".scaled" + ".hqbl" + ".png";

						case Mode.HighQualityBicubic:
							return ".scaled" + ".hqbc" + ".png";

						case Mode.NearestNeighbor:
							return ".scaled" + ".nn" + ".png";

						default:
							break;
					}
				}

				return ".scaled" + ".png";
			}
		}

		private string AsSaveFolder( FileInfo file )
		{
			// 出力パス指定がない場合、直下出力モード
			if ( string.IsNullOrEmpty( this.OutputPath ) )
			{
				return file.Directory.FullName;
			}


			string output = AsOutputPath( file );

			if ( !Directory.Exists( output ) )
			{
				Directory.CreateDirectory( output );
			}

			return output;
		}

		private string AsOutputPath( FileInfo file )
		{
			// 出力先パスを設定
			// ・絶対パス指定の場合はそこに、
			// ・相対パス指定の場合はファイルのディレクトリにフォルダを切る
			return ResizerUtility.IsFullPath( this.OutputPath )
				? this.OutputPath
				: ResizerUtility.MergePath( file.Directory.FullName, this.OutputPath );
		}
		

		private string AsSavePath( FileInfo file )
		{
			string directory = this.AsSaveFolder( file );

			string name = file.Name.split( "." ).First();
			string extension = this.SaveExtension;

			string newname = name + extension;

			return Path.Combine( directory, newname );
		}
		#endregion
		
		
	}
}
