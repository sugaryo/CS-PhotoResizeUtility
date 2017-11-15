using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoResizer;

namespace PhotoResizerTest
{
	[TestClass]
	public class extensionsTest
	{
		public extensionsTest()
		{
			//
			// TODO: コンストラクター ロジックをここに追加します
			//
		}

		private TestContext testContextInstance;

		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		//
		// テストを作成する際には、次の追加属性を使用できます:
		//
		// クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// 各テストを実行する前に、TestInitialize を使用してコードを実行してください
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// 各テストを実行した後に、TestCleanup を使用してコードを実行してください
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion


		#region extensions.scale(this int, double)
		
		[TestMethod]
		public void scaleテスト01__1_1_1() { scale_test( 1, 1.0, 1 ); }
		[TestMethod]
		public void scaleテスト02__2_2_4() { scale_test( 2, 2.0, 4 ); }
		[TestMethod]
		public void scaleテスト03__4_1p5_6() { scale_test( 4, 1.5, 6 ); }
		[TestMethod]
		public void scaleテスト03__10_10_100() { scale_test( 10, 10.0, 100 ); }
		[TestMethod]
		public void scaleテスト05__10000_0p5_5000() { scale_test( 10000, 0.5, 5000 ); }
		[TestMethod]
		public void scaleテスト06__10000_2p5_25000() { scale_test( 10000, 2.5, 25000 ); }
		
		[TestMethod]
		public void scaleテスト_a1__9999_0p3_2999() { scale_test( 9999, 0.3, 2999 ); }
		[TestMethod]
		public void scaleテスト_a2__10000_0p3333_3333() { scale_test( 10000, 0.3333, 3333 ); }
		[TestMethod]
		public void scaleテスト_a3__1_0p9999_0() { scale_test( 1, 0.9999, 0 ); }

		private void scale_test( int n, double b, int expect )
		{
			// double の演算誤差を考慮したほうが良いか、、、？
			int actual = extensions.scale( n, b );
			Assert.AreEqual( expect, actual );
		}
		#endregion

		#region extensions.unit(this int, int)

		[TestMethod]
		public void unitテスト01__m1_4_0() { unit_test( -1, 4, 0 ); }
		[TestMethod]
		public void unitテスト02__0_4_0() { unit_test( 0, 4, 0 ); }
		[TestMethod]
		public void unitテスト03__1_4_4() { unit_test( 1, 4, 4 ); }
		[TestMethod]
		public void unitテスト04__4_4_4() { unit_test( 4, 4, 4 ); }
		[TestMethod]
		public void unitテスト05__7_4_4() { unit_test( 7, 4, 4 ); }
		[TestMethod]
		public void unitテスト06__8_4_8() { unit_test( 8, 4, 8 ); }
		[TestMethod]
		public void unitテスト07__11_4_8() { unit_test( 11, 4, 8 ); }
		[TestMethod]
		public void unitテスト08__12_4_12() { unit_test( 12, 4, 12 ); }
		[TestMethod]
		public void unitテスト09__5_1_5() { unit_test( 5, 1, 5 ); }
		[TestMethod]
		public void unitテスト10__5_0_5() { unit_test( 5, 0, 5 ); }
		[TestMethod]
		public void unitテスト11__5_m1_5() { unit_test( 5, -1, 5 ); }
		[TestMethod]
		public void unitテスト12__0_1_0() { unit_test( 0, 1, 0 ); }
		[TestMethod]
		public void unitテスト13__m1_m1_0() { unit_test( -1, -1, 0 ); }

		private void unit_test( int n, int unit, int expect )
		{
			int actual = extensions.unit( n, unit );
			Assert.AreEqual( expect, actual );
		}
		#endregion
	}
}
