using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace Debug
{
	public partial class Dialog : DialogViewController
	{
		public Dialog () : base (UITableViewStyle.Grouped, null)
		{
			Root = new RootElement ("Dialog") 
			{
				new MySection()
				{
					new StringElement("Why this?"),
					new StringElement("Help Me!")
				}
			};
		}
	}
}
