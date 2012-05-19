using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch;
using MonoTouch.iAd;

namespace SmsGroup
{
	public class AdManager
	{
		public static int RefreshMilliSecond = 60000;
		
		public static ADBannerView Ad 
		{
			get
			{
				if(ad == null)
				{
					SizeF bannerSize = ADBannerView.SizeFromContentSizeIdentifier(ADBannerView.SizeIdentifierPortrait);
					ad = new ADBannerView(new RectangleF(new PointF(0,0), bannerSize));
					ad.Delegate = new SmsAdDelegate();
					ad.Hidden = true;
				}
				
				return ad;
			}
		}
		
		private static ADBannerView ad = null;
		public AdManager ()
		{
		}
		
		public static void LoadBanner()
		{
			SizeF bannerSize = ADBannerView.SizeFromContentSizeIdentifier(ADBannerView.SizeIdentifierPortrait);
			ad = new ADBannerView(new RectangleF(new PointF(0,0), bannerSize));
			ad.Delegate = new SmsAdDelegate();
			ad.Hidden = true;
		}
		
		public static ADBannerView GetAd(float x, float y)
		{
			if(Ad.Frame.X != x || Ad.Frame.Y != y)
			{
				Ad.Frame = new RectangleF(new PointF(x,y), Ad.Frame.Size);
			}
			
			return Ad;
		}
	}
}

