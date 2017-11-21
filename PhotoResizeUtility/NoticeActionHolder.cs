using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoResizer
{
	public class NoticeActionHolder
	{
		private NoticeAction notice;

		public NoticeAction Execute
		{
			get
			{
				return this.notice;
			}

			set
			{
				// null が与えられた場合は NullObject として NopDelegate に置き換える。
				this.notice = value ?? Nop;
			}
		}
		
		private static void Nop( NoticeType notice, FileInfo src, FileInfo dst )
		{
			// NOP は何もしない。
		}

		#region ctor
		public NoticeActionHolder(NoticeAction action = null)
		{
			this.Execute = action;
		}
		#endregion
	}
}
