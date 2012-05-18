// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmsGroup
{
	[Register ("SmsComposerViewController")]
	partial class SmsComposerViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel LabelComposeMessage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView Message { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelUseMagicExpression { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CharacterCount { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelMaxContact { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton MaskButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SendButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider Slider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SliderCount { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch MagicExpressionSwitch { get; set; }

		[Action ("MaxRecipientInfoTouchUpInside:")]
		partial void MaxRecipientInfoTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("MagicExpressionInfoTouchUpInside:")]
		partial void MagicExpressionInfoTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("SendButtonTouchedUp:")]
		partial void SendButtonTouchedUp (MonoTouch.Foundation.NSObject sender);

		[Action ("MagicExpressionValueChanged:")]
		partial void MagicExpressionValueChanged (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LabelComposeMessage != null) {
				LabelComposeMessage.Dispose ();
				LabelComposeMessage = null;
			}

			if (Message != null) {
				Message.Dispose ();
				Message = null;
			}

			if (LabelUseMagicExpression != null) {
				LabelUseMagicExpression.Dispose ();
				LabelUseMagicExpression = null;
			}

			if (CharacterCount != null) {
				CharacterCount.Dispose ();
				CharacterCount = null;
			}

			if (LabelMaxContact != null) {
				LabelMaxContact.Dispose ();
				LabelMaxContact = null;
			}

			if (MaskButton != null) {
				MaskButton.Dispose ();
				MaskButton = null;
			}

			if (SendButton != null) {
				SendButton.Dispose ();
				SendButton = null;
			}

			if (Slider != null) {
				Slider.Dispose ();
				Slider = null;
			}

			if (SliderCount != null) {
				SliderCount.Dispose ();
				SliderCount = null;
			}

			if (MagicExpressionSwitch != null) {
				MagicExpressionSwitch.Dispose ();
				MagicExpressionSwitch = null;
			}
		}
	}
}
