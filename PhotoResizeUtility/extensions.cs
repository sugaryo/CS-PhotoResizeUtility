using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoResizer
{
	/// <summary>
	/// 拡張メソッド
	/// </summary>
	/// <remarks>
	/// 拡張メソッドは通常メソッドと区別する為、
	/// メソッド名を Java ライクに lower camel case にしています。
	/// </remarks>
	public static class extensions
	{
		/// <summary>
		/// 指定した整数値 <paramref name="n"/> を <paramref name="s"/> 倍し、端数を切り捨てた整数値を返します。
		/// </summary>
		/// <param name="n">整数</param>
		/// <param name="s">倍率</param>
		/// <returns><paramref name="s"/> 倍した整数 <paramref name="n"/>（端数切捨て）</returns>
		public static int scale( this int n, double s )
		{
			return (int)( n * s );
		}

		/// <summary>
		/// 指定した整数 <paramref name="n"/> を、指定した単位 <paramref name="u"/> で丸めます。
		/// </summary>
		/// <remarks>
		/// <para>整数 <paramref name="n"/> に 0 以下の値を指定した場合、このメソッドは 0 を返します。</para>
		/// <para>単位 <paramref name="u"/> に 1 以下の値を指定した場合、このメソッドは 整数 <paramref name="n"/> をそのまま返します。</para>
		/// <para>単位 <paramref name="u"/> に 1 より大きな値を指定した場合、このメソッドは 与えた整数を指定した単位で丸め込みます。</para>
		/// <para>
		/// このメソッドは、丸め込み処理として「整数 <paramref name="n"/> を超えない 単位 <paramref name="u"/> の整数倍の値」を計算して返します。
		/// 即ち、基本的には 整数 <paramref name="n"/> を 単位 <paramref name="u"/> で割った後で、単位 <paramref name="u"/> 倍し直した値を返します。
		/// 但し、このメソッドの算出する値は最低値として、指定した単位 <paramref name="u"/> 未満（より具体的に言うと 0 ）にならない事を保証します。
		/// このメソッドが戻り値として 0 を返すのは与えられた整数が最初から 0 以下の場合に限り、丸め込み処理を行う場合は必ず 0 より大きい値を返します。
		/// </para>
		/// </remarks>
		/// 
		/// <param name="n">整数</param>
		/// <param name="u">単位</param>
		/// <returns>整数値 <paramref name="n"/> を指定した単位 <paramref name="u"/> で丸め込んだ値</returns>
		/// 
		/// <example>
		/// この拡張メソッドを使用した場合の戻り値の例です。
		/// <code>
		/// <![CDATA[
		/// -1.unit(  4 ) : 0
		///  0.unit(  4 ) : 0
		///  1.unit(  4 ) : 4
		///  4.unit(  4 ) : 4
		///  7.unit(  4 ) : 4
		///  8.unit(  4 ) : 8
		/// 11.unit(  4 ) : 8
		/// 12.unit(  4 ) : 12
		///  5.unit(  1 ) : 5
		///  5.unit(  0 ) : 5
		///  5.unit( -1 ) : 5
		///  0.unit(  1 ) : 0
		/// -1.unit( -1 ) : 0
		/// ]]>
		/// </code>
		/// </example>
		public static int unit( this int n, int u )
		{
			if ( n <= 0 ) return 0;
			
			if ( u <= 1 ) return n;

			//int a;
			//int r = Math.DivRem( n, u , out a );
			//
			//return ( 0 == a ? r : r + 1 ) * u;
			
			int r = n / u;

			return ( 0 == r ? 1 : r ) * u;
		}

		/// <summary>
		/// 文字列 <paramref name="s"/> を、指定した区切り文字列 <paramref name="by"/> で分割し <seealso cref="System.String"/>配列 を返します。
		/// </summary>
		/// <param name="s">分割する元の文字列</param>
		/// <param name="by">分割する区切り文字列</param>
		/// <param name="option">文字列の分割オプション（省略可能：default <seealso cref="StringSplitOptions.RemoveEmptyEntries"/>）</param>
		/// <returns>指定した区切り文字列で分割した文字列の配列</returns>
		public static string[] split( this string s, 
				string by, 
				StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries )
		{
			return s.Split( new[] { by }, option );
		}

		/// <summary>
		/// 文字列 <paramref name="s"/> の前方一致検証（複数文字列）
		/// </summary>
		/// <param name="s">検証する文字列</param>
		/// <param name="with">前方一致を試す複数の文字列</param>
		/// <returns>文字列 <paramref name="s"/> が、いずれかの候補 <paramref name="with"/> に前方一致するか否かを返す。</returns>
		public static bool startsWith( this string s, params string[] with )
		{
			if ( string.IsNullOrEmpty( s ) ) return false;

			foreach ( var w in with )
			{
				if ( s.StartsWith( w ) ) return true;
			}
			return false;
		}
		/// <summary>
		/// 文字列 <paramref name="s"/> の前方一致検証（複数文字）
		/// </summary>
		/// <param name="s">検証する文字列</param>
		/// <param name="with">前方一致を試す複数の文字</param>
		/// <returns>文字列 <paramref name="s"/> が、いずれかの候補 <paramref name="with"/> に前方一致するか否かを返す。</returns>
		public static bool startsWith( this string s, params char[] with )
		{
			if ( string.IsNullOrEmpty( s ) ) return false;

			if ( s.Length < 1 ) return false;

			char c = s[0];

			foreach ( var w in with )
			{
				if ( c == w ) return true;
			}
			return false;
		}
	}

}
