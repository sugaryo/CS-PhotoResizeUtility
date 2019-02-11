using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PhotoResizer;
using static PhotoResizer.ResizerUtility;

namespace PhotoResizerConsole
{
	class Program
    {
        [Obsolete("取り敢えず動作確認レベルで実装、最終的にはargs経由でフォルダパスと動作オプション指定する")]
        static void Main( string[] args )
		{
			// 今は動作確認用のテスト実装
			try
			{
#warning 取り敢えずフォルダを直接食わせる形で暫定実装。
                test(args);

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine();
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex.Message );
				Console.WriteLine( ex.StackTrace );
            }
            Console.WriteLine("press any key to exit.");
            Console.ReadKey(true);
        }
		
		private static void test(string[] paths)
		{
			PhotoResizer.Resizer resizer = new PhotoResizer.Resizer()
			{
				OutputPath      = @"/save", // 画像の相対パスで save フォルダを作ってそこに出す。
				Mode            = System.Drawing.Drawing2D.InterpolationMode.Bicubic,
				UnitSize        = 4,
				OverWrite       = true,
				ModeExtension   = true,


				MinimumSize = new System.Drawing.Size( 256, 256 ),
				MaximumSize = new System.Drawing.Size( 4096, 4096 ),
			};


			var files = ResizerUtility
					.AsFiles( paths, FolderOption.SearchFilesShallow )
					.ToList();

			int n = files.Count;

			Console.WriteLine( "files:" + n.ToString() );

			if ( 0 < files.Count )
			{
				resizer.Resize( files, 0.5 );
			}

			Console.WriteLine( "complete." );
		}
	}
}
