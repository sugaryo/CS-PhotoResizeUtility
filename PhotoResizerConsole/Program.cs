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
		static void Main( string[] args )
		{
			// 今は動作確認用のテスト実装
			try
			{
				test();

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
		
		[Obsolete("取り敢えず動作確認レベルで実装、最終的にはargs経由でフォルダパスと動作オプション指定する")]
		private static void test()
		{
			PhotoResizer.Resizer resizer = new PhotoResizer.Resizer()
			{
				OutputPath      = @"/save",
				Mode            = System.Drawing.Drawing2D.InterpolationMode.Bicubic,
				UnitSize        = 4,
				OverWrite       = true,
				ModeExtension   = true,


				MinimumSize = new System.Drawing.Size( 256, 256 ),
				MaximumSize = new System.Drawing.Size( 4096, 4096 ),
			};


			string[] paths = {
				@"C:\photo",
				@"C:\test",
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
