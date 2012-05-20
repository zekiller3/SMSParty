using System;
using System.Threading;
using MonoTouch.iAd;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;
namespace SmsGroup
{
	public class SmsAdDelegate : ADBannerViewDelegate
	{
		private bool isVisible = false;
		private BaseDialogViewController controller = null;
		
		public SmsAdDelegate (BaseDialogViewController c = null)
		{
			this.controller = c;
			UpdateBannerVisibility(AdManager.Ad, false);
		}
		
		public void ShowAdInController()
		{
			if(AdManager.Ad.BannerLoaded && controller != null)
			{
				controller.tableView.Frame = new RectangleF(0,
			                                      controller.tableView.Frame.Y,
			                                      controller.tableView.Frame.Width,
			                                      controller.tableView.Frame.Size.Height - AdManager.Ad.Frame.Height);
			}
		}
		
		public override void WillLoad (ADBannerView banner)
		{
			Console.WriteLine("Ad Will Load");
			banner.Hidden = false;
			UpdateBannerVisibility (banner, false);
			//RefreshAd(banner);
		}
		
		protected override void Dispose (bool disposing)
		{
			if(controller != null && this.isVisible)
			{
				controller.tableView.Frame = new RectangleF(0,
			                                      controller.tableView.Frame.Y,
			                                      controller.tableView.Frame.Width,
			                                      controller.tableView.Frame.Size.Height + AdManager.Ad.Frame.Height);
			}
			
			base.Dispose (disposing);
		}
		
		public override void AdLoaded (ADBannerView banner)
		{
			Console.WriteLine("Ad Loaded");
			banner.Hidden = false;
			UpdateBannerVisibility(banner, false);
		}
		
		public override void FailedToReceiveAd (ADBannerView banner, MonoTouch.Foundation.NSError error)
		{
			Console.WriteLine("FailedToReceiveAd {0}:{1}", error.Code, error.Domain); 
			UpdateBannerVisibility(banner, true);
		}
		
		private void UpdateBannerVisibility(ADBannerView banner, bool mustDisappear)
		{
			if((mustDisappear || !banner.BannerLoaded) && this.isVisible){
				banner.Hidden = true;
				this.isVisible = false;
				UIView.BeginAnimations("animateAdBannerOff",IntPtr.Zero);
				banner.Frame.X = UIScreen.MainScreen.ApplicationFrame.Width;
				UIView.CommitAnimations();
				
				if(controller != null)
				{
					controller.tableView.Frame = new RectangleF(0,
			                                      controller.tableView.Frame.Y,
			                                      controller.tableView.Frame.Width,
			                                      controller.tableView.Frame.Size.Height + AdManager.Ad.Frame.Height);
					banner.RemoveFromSuperview();
				}
			}
			
			if(!mustDisappear && banner.BannerLoaded && !this.isVisible)
			{
				UIView.BeginAnimations("animateAdBannerOn",IntPtr.Zero);
				banner.Frame.X = 0;
				UIView.CommitAnimations();
				isVisible = true;
				
				if(this.controller != null)
				{
					Console.WriteLine("TableView Height before : " + controller.tableView.Frame.Size.Height);
					controller.tableView.Frame = new RectangleF(0,
					                                      controller.tableView.Frame.Y,
					                                      controller.tableView.Frame.Width,
					                                      controller.tableView.Frame.Size.Height - AdManager.Ad.Frame.Height);
					controller.MainView.AddSubview(banner);
					Console.WriteLine("TableView Height after : " + controller.tableView.Frame.Size.Height);
				}
			}
		}
	}
}

