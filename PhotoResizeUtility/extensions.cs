using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoResizer
{
	public static class extensions
	{
		public static int scale( this int n, double s )
		{
			return (int)( n * s );
		}
		public static int unit( this int n, int u )
		{
			if ( u <= 1 ) return n;

			//int a;
			//int r = Math.DivRem( n, u , out a );
			//
			//return ( 0 == a ? r : r + 1 ) * u;
			
			int r = n / u;

			return ( 0 == r ? 1 : r ) * u;
		}

		public static string[] split( this string s, 
				string by, 
				StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries )
		{
			return s.Split( new[] { by }, option );
		}

		public static bool startsWith( this string s, params string[] with )
		{
			foreach ( var w in with )
			{
				if ( s.StartsWith( w ) ) return true;
			}
			return false;
		}
		public static bool startsWith( this string s, params char[] with )
		{
			if ( s.Length < 1 ) return false;

			char c = s[0];

			foreach ( char w in with )
			{
				if ( c == w ) return true;
			}
			return false;
		}
	}

}
