using System;
using System.Threading;
using MonoTouch.iAd;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;
using System.Linq;
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
				Console.WriteLine("Disposing, resizing table frame");
				// UpdateBannerVisibility(AdManager.Ad, true);
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
			Console.WriteLine("{3} : Must Disappear {0}, Is Banner Loaded {1}, is visible {2}", mustDisappear, banner.BannerLoaded, this.isVisible, DateTime.Now.ToString("hh:MM:ss"));
			if((mustDisappear || !banner.BannerLoaded) && this.isVisible){
				Console.WriteLine("Removing Banner");
				banner.Hidden = true;
				this.isVisible = false;
				
				UIView.BeginAnimations("animateAdBannerOff",IntPtr.Zero);
				banner.Frame = new RectangleF(UIScreen.MainScreen.ApplicationFrame.Width, banner.Frame.Y, banner.Frame.Width, banner.Frame.Height);
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
				Console.WriteLine("Adding Banner");
				isVisible = true;
				banner.Hidden = false;
				if(this.controller != null)
				{
					ThreadPool.QueueUserWorkItem((e) => {
						Thread.Sleep(100);
						controller.tableView.Frame = new RectangleF(0,
						                                      controller.tableView.Frame.Y,
						                                      controller.tableView.Frame.Width,
						                                      controller.tableView.Frame.Size.Height - AdManager.Ad.Frame.Height);
						controller.InvokeOnMainThread(()=> {controller.MainView.AddSubview(banner);});
						Console.WriteLine("View in base controller : {0}", controller.MainView.Subviews.Count());
					});
				}
				
				UIView.BeginAnimations("animateAdBannerOn",IntPtr.Zero);
				banner.Frame = new RectangleF(0, banner.Frame.Y, banner.Frame.Width, banner.Frame.Height);
				UIView.CommitAnimations();
			}
		}
	}
}

