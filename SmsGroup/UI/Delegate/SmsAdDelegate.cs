using System;
using MonoTouch.iAd;
namespace SmsGroup
{
	public class SmsAdDelegate : ADBannerViewDelegate
	{
		public SmsAdDelegate ()
		{
		}
		
		public override void WillLoad (ADBannerView bannerView)
		{
			bannerView.Hidden = false;
		}
		
		public override void AdLoaded (ADBannerView banner)
		{
			banner.Hidden = false;
		}
		
		public override void FailedToReceiveAd (ADBannerView banner, MonoTouch.Foundation.NSError error)
		{
			banner.Hidden = true;
		}
	}
}

