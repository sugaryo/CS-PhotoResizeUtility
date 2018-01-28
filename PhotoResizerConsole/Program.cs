using System;
using System.Collections.Generic;
using System.IO;
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
				string[] directories = args
					.AsEnumerable()
					.Select( x => new DirectoryInfo(x) )
					.Where( x => x.Exists )
					.Select( x => x.FullName )
					.ToArray();

				Resize( directories );

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine( "press any key to exit." );
				Console.ReadKey( true );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex.Message );
				Console.WriteLine( ex.StackTrace );
			}
		}
		
		private static void Resize(string[] directories)
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

			
			var files = ResizerUtility
					.AsFiles( directories, FolderOption.SearchFilesShallow )
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
