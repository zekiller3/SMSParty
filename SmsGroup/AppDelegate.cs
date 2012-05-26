using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmsGroup
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UINavigationController viewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new UINavigationController ();
			viewController.PushViewController(new MainScreenGroup(), true);
			viewController.NavigationBar.Opaque = true;
			if(viewController.NavigationBar.RespondsToSelector(new MonoTouch.ObjCRuntime.Selector("setBackgroundImage:forBarMetrics:"))){
	//			viewController.NavigationBar.SetBackgroundImage(UIImage.FromBundle("Images/navigationbar.png"), UIBarMetrics.Default);
			}
			window.MakeKeyAndVisible ();
			
#if LITE
			AdManager.LoadBanner();
#endif
			
			// On iOS5 we use the new window.RootViewController, on older versions, we add the subview
            if (UIDevice.CurrentDevice.CheckSystemVersion (5, 0))
				window.RootViewController = viewController;	
			else
				window.AddSubview (viewController.View);
			
			return true;
		}
	}
}

