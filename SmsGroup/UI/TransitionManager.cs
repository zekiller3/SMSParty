using System;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
namespace SmsGroup
{
	public class TransitionManager
	{
		public static void AddNewGroup(UINavigationController navigation)
		{
			UIView.BeginAnimations(null,IntPtr.Zero);
			UIView.SetAnimationDuration(0.4);
			UIView.SetAnimationTransition(UIViewAnimationTransition.CurlUp,navigation.View,true);
			UIView.CommitAnimations();
		}
		
		public static void ReturnToMainScreen(UINavigationController navigation)
		{
			UIView.BeginAnimations(null,IntPtr.Zero);
			UIView.SetAnimationDuration(0.4);
			UIView.SetAnimationTransition(UIViewAnimationTransition.CurlDown,navigation.View,true);
			UIView.CommitAnimations();
		}
		
		/*
kCATransitionFade
kCATransitionPush
kCATransitionMoveIn
kCATransitionReveal
@"suckEffect"
@"spewEffect"
@"genieEffect"
@"unGenieEffect"
@"rippleEffect"
@"twist"
@"tubey"
@"swirl"
@"charminUltra"
@"zoomyIn"
@"zoomyOut"
CATransition transition = new CATransition();
			transition.Duration = 1;
			transition.Type = "kCATransitionMoveIn";
			navigation.View.Layer.AddAnimation(transition,"mykey");
*/
		public static void EditGroup(UINavigationController navigation)
		{
			AddNewGroup(navigation);
		}
		
		public static void SendSms(UINavigationController navigation)
		{
			UIView.BeginAnimations(null,IntPtr.Zero);
			UIView.SetAnimationDuration(0.4);
			UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromRight, navigation.View, true);
			UIView.CommitAnimations();
		}
	}
}

