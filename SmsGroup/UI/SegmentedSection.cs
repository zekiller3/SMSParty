using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
namespace SmsGroup
{
	public class SegmentedSection : Section
	{
	    public UISegmentedControl SegmentedControl;
	
	    public SegmentedSection(string title)
	        : base(title)
	    {
	        InitializeSection();
			
			// this.SegmentedControl.ControlStyle = UISegmentedControlStyle.Bezeled;
			this.SegmentedControl.ControlStyle = UISegmentedControlStyle.Bar;
	    }
	
		
	    private void InitializeSection()
	    {
			UIView v = new UIView(new RectangleF(0,0,UIScreen.MainScreen.ApplicationFrame.Width, 50));
			v.AutosizesSubviews = false;
			
	        SegmentedControl = new UISegmentedControl(new RectangleF(0,0, 180, 30));
			SegmentedControl.Center = v.Center;
	        // initialize _SegmenentedControl here...
	        // make sure to set Frame appropriate relative to HeaderView bounds.
			SegmentedControl.AutoresizingMask = UIViewAutoresizing.None;
			v.AddSubview(SegmentedControl);
			this.HeaderView = v;
			//this.HeaderView = SegmentedControl;
			
			
	        //this.HeaderView.AddSubview(SegmentedControl);
	    }
	}
}

