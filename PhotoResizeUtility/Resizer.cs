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
			Canceled,
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

				case NoticeType.Canceled:
					log( "cancel", src?.FullName );
					break;

				default:
					log( notice.ToString(), src?.FullName + "  " + dst?.FullName );
					break;
			}
		}
		#endregion

		#region リサイズに関する設定

		public Mode Mode { get; set; } = Mode.HighQualityBicubic;

		public int UnitSize { get; set; } = 4;

		public Size? MinimumSize { get; set; } = null;

		public Size? MaximumSize { get; set; } = null;

		#endregion

		#region その他、動作のオプション設定群

		public string OutputPath { get; set; } = null;

		public bool MultipleExtension { get; set; } = true;

		public bool ModeExtension { get; set; } = false;

		public bool OverWrite { get; set; } = false;

		public bool IgnoreScaledExtensionFile { get; set; } = true;

		#endregion


		#region Resize (facade)
		public void Resize( IEnumerable<FileInfo> files, double scale )
		{
			ValidateScale( scale );

			foreach ( var file in files )
			{
				this.ResizeCore( file, scale );
			}
		}

		public void Resize( FileInfo file, double scale )
		{
			ValidateScale( scale );

			this.ResizeCore( file, scale );
		}

		private static void ValidateScale( double scale )
		{
			// ゼロ以下は論外。
			if ( scale <= 0.0 ) throw new ArgumentException( $"指定したスケール値{scale}が小さすぎます。" );

			// 等倍は意味が無いので無視する。
			if ( 1.0 == scale ) throw new ArgumentException( $"指定したスケール値{scale}が等倍です。" );

			// 百倍越えは流石に大きすぎるので。
			const double MAX = 100.0;
			if ( MAX < scale ) throw new ArgumentException( $"指定したスケール値{scale}が大きすぎます。（最大{MAX}）" );
		}
		#endregion


		#region Resize 内部処理
		
		private void ResizeCore( FileInfo file, double scale )
		{
			// ".scaled" 複合拡張子のファイルを無視する設定の場合、拡張子をチェックして無視する。
			if ( this.IgnoreScaledExtensionFile
				       && IsScaledExtensionFile( file ) )
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
			
			bool resized = this.DoResize( file, save, scale );
			if ( resized )
			{
				this.notice( NoticeType.Resized, file, save );
			}
			else
			{
				this.notice( NoticeType.Canceled, file, save );
			}
		}
		
		private static bool IsScaledExtensionFile( FileInfo file )
		{
			List<string> extensions = file.Name
				.ToLower()    // ファイル名全体を一旦小文字化。
				.split( "." ) // 拡張子を含むファイル名全体をドットで分割。
				.Skip( 1 )    // 先頭の要素（ファイル名本体）をスキップ。
				.ToList();    // スキップして残った拡張子（複数）をリスト化。

			// 複合拡張子の中に scaled が含まれるか否かを判定。
			return extensions.Contains( "scaled" );
		}
		


		// リサイズのメイン処理
		private bool DoResize( FileInfo file, FileInfo save, double scale )
		{
			// 画像のリサイズ処理を行い、出力先パスに png ファイルを保存する。
			using ( Bitmap bmp = new Bitmap( file.FullName ) )
			{
				int w = bmp.Width.scale( scale ).unit( this.UnitSize );
				int h = bmp.Height.scale( scale ).unit( this.UnitSize );


#warning 強制的にスキップにしないで、同一ファイルの単純コピーにしても良いかも？（オプション追加検討）
				if ( !this.IsResizable( bmp.Size, w, h, scale ) ) return false;


				using ( Bitmap resized = new Bitmap( w, h ) )
				{
					using ( Graphics g = Graphics.FromImage( resized ) )
					{
						g.InterpolationMode = this.Mode;
						g.DrawImage( bmp, 0, 0, w, h );
					}

					resized.Save( save.FullName, ImageFormat.Png );

					return true;
				}
			}
		}
		private bool IsResizable( Size org, int w, int h, double scale )
		{
			return IsResizable( org, w, h, scale < 1.0 );
		}
		private bool IsResizable( Size org, int w, int h, bool sizedown )
		{
			// ■同一サイズ判定：

			#region same size check

			// リサイズ後のサイズが同じだった場合は処理しない。
			// ※ scaleの等倍指定は最初に弾いているが、scaleとunit-size指定によっては等倍化する事もある。
			if ( org.Width == w && org.Height == h ) return false;

			#endregion


			// ■限界サイズ判定：

			#region limited size check

			// 縮小scaleの場合で、且つ最小サイズ指定がある場合：
			if ( sizedown )
			{
				// 幅高さどちらかが指定の最小値を超えている場合はNG
				if ( null != this.MinimumSize )
				{
					Size min = this.MinimumSize.Value;

					if ( w < min.Width ) return false;
					if ( h < min.Height ) return false;
				}
			}
			// 拡大scaleの場合で、且つ最大サイズ指定がある場合：
			else
			{
				// 幅高さどちらかが指定の最大値を超えている場合はNG
				if ( null != this.MaximumSize )
				{
					Size max = this.MaximumSize.Value;

					if ( max.Width  < w ) return false;
					if ( max.Height < h ) return false;
				}
			}

			#endregion


			// 何れのチェックにも引っ掛からない場合はリサイズ可能。
			return true;
		}
		
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
		

		#endregion		
	}
}
