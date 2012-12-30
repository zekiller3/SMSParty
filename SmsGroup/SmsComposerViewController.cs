using System;
using System.Linq;
using System.Drawing;
using MonoTouch.MessageUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;
using MonoTouch.Dialog;
using MonoTouch.AddressBook;
using MonoTouch.iAd;
namespace SmsGroup
{
	public partial class SmsComposerViewController : UIViewController
	{
		private const string LocalizedKey = "SmsComposerViewController";
		private bool hasFinishedLoadedPersons = false;
		public MainScreenGroup Parent {get;set;}
		public string UserMessage 
		{
			get
			{
				return this.Message.Text;
			}
			set
			{
				this.Message.Text = value;
			}
		}
		public SmsGroupObject Sms 
		{
			get;set;
		}
		public ABPerson[] Phones {get;set;}
		private EditContactBeforeSMSController editContactController = null;
		private ADBannerView Ad;
		public int BatchSize
		{
			get
			{
				return (int)Slider.Value;
			}
		}
		
		public SmsComposerViewController(MainScreenGroup parent, SmsGroupObject sms) : this()
		{
			this.Title = string.Format ("{0} ({1})",sms.Name, sms.Count);
			Parent = parent;
			Sms = sms;
			this.Slider.MinValue = 1;
			this.Slider.MaxValue = Sms.Count;
			this.Slider.Value = Sms.Count > 10 ? 10: Sms.Count;
			this.SliderCount.Text = this.Slider.Value.ToString();
			this.Slider.ValueChanged += HandleSliderhandleValueChanged;
			this.Message.Layer.CornerRadius = 10;
			
			// Localize Labels
			LabelComposeMessage.Text = Settings.GetLocalizedString(LabelComposeMessage.Text, LocalizedKey);
			LabelMaxContact.Text = Settings.GetLocalizedString(LabelMaxContact.Text, LocalizedKey);
			LabelUseMagicExpression.Text = Settings.GetLocalizedString(LabelUseMagicExpression.Text, LocalizedKey);
			
			ThreadPool.QueueUserWorkItem ((e) => {
				Sms.LoadPersons();
				hasFinishedLoadedPersons = true;
			});
			
#if LITE
			MagicExpressionSwitch.Enabled = false;
			
#endif
// 			UITextView debug = null;
//			debug = new UITextView(new RectangleF(0, SendButton.Frame.Y + 2,160,20));
//			this.Add(debug);
//			ThreadPool.QueueUserWorkItem ((e) => {
//				while(true){
//					if(Ad!= null){
//						InvokeOnMainThread(() => {
//							debug.Text = 
//								string.Format("Loaded {0}, AdSection {1},", Ad.BannerLoaded, Ad.AdvertisingSection);
//						});
//						
//						Thread.Sleep(1000);
//					}
//				}
//			});
		}
		
		void HandleSliderhandleValueChanged (object sender, EventArgs e)
		{
			DismissKeyboard();
			SliderCount.Text = BatchSize.ToString();
			Slider.Value = BatchSize;
		}
		
		private UIBarButtonItem edit = null;
		private UIBarButtonItem done = null;
		public SmsComposerViewController () : base ("SmsComposerViewController", null)
		{
			NSBundle.MainBundle.LoadNib("SmsComposerViewController",this,null);
			// Perform any additional setup after loading the view, typically from a nib.
			edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit);
			edit.Clicked += EditContact;
			done = new UIBarButtonItem(UIBarButtonSystemItem.Done);
			done.Clicked += HandleDoneClicked;
			this.NavigationItem.RightBarButtonItem = edit;
			this.Message.Started += HandleMessagehandleStarted;
			this.Message.Changed += HandleMessagehandleChanged;
			this.MaskButton.AllTouchEvents += HandleMaskButtonhandleAllTouchEvents;

		}

		void HandleDoneClicked (object sender, EventArgs e)
		{
			DismissKeyboard();
		}

		void HandleMessagehandleStarted (object sender, EventArgs e)
		{
			this.NavigationItem.RightBarButtonItem = done;
		}
		
		private void DismissKeyboard()
		{
			if(this.Message.CanResignFirstResponder){
				this.Message.ResignFirstResponder();
				this.NavigationItem.RightBarButtonItem = edit;
			}
		}
		
		void HandleMaskButtonhandleAllTouchEvents (object sender, EventArgs e)
		{
			DismissKeyboard();	
		}

		partial void SendButtonTouchedUp (MonoTouch.Foundation.NSObject sender)
		{
			SendSms();
		}

		void HandleMessagehandleChanged (object sender, EventArgs e)
		{
			this.CharacterCount.Text = this.Message.Text.Count().ToString();
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated); 
			this.View.BackgroundColor = UIColor.FromPatternImage(Settings.Background);
#if LITE
			ThreadPool.QueueUserWorkItem((e) => {
				Thread.Sleep(100);
				int toolbarheight = this.Parent.NavigationController.ToolbarHidden ? 0 : 87;
				Ad = AdManager.GetAd(0,UIScreen.MainScreen.ApplicationFrame.Height - toolbarheight - AdManager.Ad.Frame.Height);
				Ad.Delegate = new SmsAdDelegate();
				InvokeOnMainThread(()=> { this.Add(Ad); });
			});
#endif
			
			// contacts might have changed, we need to update UI
			if(this.editContactController != null)
			{
				float previousValue = this.Slider.Value;
				this.Slider.MaxValue = this.editContactController.Phones.Count();
				this.Title = string.Format ("{0} ({1})",this.Sms.Name, this.Slider.MaxValue);
				if(previousValue > this.Slider.MaxValue)
				{
					this.Slider.SetValue(this.Slider.MaxValue, true);
					this.SliderCount.Text = this.Slider.MaxValue.ToString();
					this.Title = string.Format ("{0} ({1})",this.Sms.Name, this.SliderCount.Text);
				}
				
				
			}
			else
			{
				
			}
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if(Ad != null){
				this.Ad.Delegate.Dispose();
				this.Ad.RemoveFromSuperview();
			}
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		protected override void Dispose (bool disposing)
		{
			if(this.editContactController != null)
			{
				this.editContactController.Dispose();
			}
			
			base.Dispose (disposing);
		}
		
		public void EditContact(object sender, EventArgs e)
		{
			DismissKeyboard();
			while(!hasFinishedLoadedPersons)
			{
				Thread.Sleep(100);
			}
			if(editContactController == null)
			{
				editContactController = new EditContactBeforeSMSController(this.Sms);
			}
			
			this.NavigationController.PushViewController(editContactController, true);
		}
			                                                             
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
		
		partial void MagicExpressionValueChanged (MonoTouch.Foundation.NSObject sender)
		{
#if LITE
			return;
#endif
			if(MagicExpressionSwitch.On)
			{
				Slider.SetValue (1,true);
				this.SliderCount.Text = 1.ToString();
				this.Slider.Enabled = false;
			}
			else
			{
				this.Slider.Enabled = true;
			}
		}
		
		public void SendSms()
		{
			if(MFMessageComposeViewController.CanSendText)
			{
				if(editContactController != null)
				{
					Phones = editContactController.Phones;
				}else
				{
					Phones = Sms.Persons.ToArray();
				}
				
				BatchMFMessageComposeViewController smscontroller = 
					new BatchMFMessageComposeViewController(this.Parent, Phones , Message.Text, BatchSize, this.MagicExpressionSwitch.On);
				smscontroller.Finished += HandleSmscontrollerFinished;
				this.Parent.NavigationController.PresentModalViewController(smscontroller, true);
			}
		}

		void HandleSmscontrollerFinished (object sender, MFMessageComposeResultEventArgs e)
		{
			BatchMFMessageComposeViewController batchController = sender as BatchMFMessageComposeViewController;
			
			switch (e.Result)
			{
				
				case MessageComposeResult.Sent:
					
					if(batchController.HasRecipientsLeft)
					{
						batchController.DismissModalViewControllerAnimated(false);
						Thread.Sleep(100);
						BatchMFMessageComposeViewController c = new BatchMFMessageComposeViewController(
							batchController.Parent,
							batchController.LeftRecipients.ToArray(), 
							this.Message.Text,
							batchController.BatchSize,
							batchController.MagicExpressionEnabled);
						c.Finished += HandleSmscontrollerFinished;
						batchController.Dispose();
						this.Parent.NavigationController.PresentModalViewController(c, true);
					}
					else
					{
						batchController.ResignFirstResponder();
						batchController.MessageComposeDelegate.Dispose();
						batchController.DismissModalViewControllerAnimated(false);
						batchController.Dispose();
						ThreadPool.QueueUserWorkItem ((evt) => {
							Thread.Sleep(500);
							UIAlertView alert = new UIAlertView(
								Settings.GetLocalizedString("Sms is being sent", LocalizedKey),
								Settings.GetLocalizedString("Your message will be sent soon", LocalizedKey),
								null, "OK");
							this.InvokeOnMainThread(()=> { alert.Show();});
						
						((AppDelegate)UIApplication.SharedApplication.Delegate).Apprater.UserDidSignificantEvent(true);
						});
					}
					break;
				
				case MessageComposeResult.Cancelled:
					batchController.ResignFirstResponder();
					batchController.DismissModalViewControllerAnimated(false);
					batchController.Dispose();
					break;
				default:
					break; 
			}
		}
		
		partial void MaxRecipientInfoTouchUpInside (MonoTouch.Foundation.NSObject sender)
		{
			ShowMaxRecipientInfo(); 
		}
		
		public void ShowMaxRecipientInfo()
		{
			UIAlertView alert = new UIAlertView(
				Settings.GetLocalizedString("Max Recipient", LocalizedKey),
			   Settings.GetLocalizedString("If you have too many recipients and would like to send by smaller group at a time,\r\nyou can modify the slider to specify the maximum number of recipients per text message", LocalizedKey),
			   null,"OK");
			alert.Show();   
		}
		
		public void ShowMagicExpressionInfo()
		{
			UIAlertView alert = new UIAlertView(
#if LITE
				string.Format("{0}{1}",
			              Settings.GetLocalizedString("Magic Expression", LocalizedKey),
			              "\r\n" + Settings.GetLocalizedString("(only available in full version)", LocalizedKey)),
#else
			    
				Settings.GetLocalizedString("Magic Expression", LocalizedKey),
#endif
				
		   		Settings.GetLocalizedString("Magic Expression allows you to add first and/or last name of your recipient in your custom message.\r\nFor example if you want to wish happy new year to everyone but you want to write their name,\r\nYour message will look like this \r\n'Happy New Year /f!!'\r\n/f : add first name\r\n/l : add last name\r\nWhen Magic Expression is enabled, you can send only 1 sms at a time.", LocalizedKey),
				null, "OK");
			alert.Show();
		}
		
		partial void MagicExpressionInfoTouchUpInside (MonoTouch.Foundation.NSObject sender)
		{
			ShowMagicExpressionInfo();
		}
	}
}

