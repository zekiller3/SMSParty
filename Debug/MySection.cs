using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace Debug
{
	public class MySection : Section
	{
		public MySection ()
		{
			UIView v0 = new UIView(new RectangleF(0,0,320, 60));
			v0.AutoresizingMask = UIViewAutoresizing.All;
			v0.AutosizesSubviews = true;
			UIView v = new UIView(new RectangleF(0,0,30,30));
			v.Center = v0.Center;
			v.BackgroundColor = UIColor.Red;
			v0.AddSubview(v);
			this.HeaderView = v0;
		}
	}
}

