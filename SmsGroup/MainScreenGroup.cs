using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.AddressBookUI;
using System.Drawing;
namespace SmsGroup
{
	public partial class MainScreenGroup : BaseDialogViewController
	{
		private const string LocalizedKey = "MainScreenGroup";
		public Section ListGroupSection {get;set;}
		public MainScreenGroup () : base (true, ToolbarItemOption.Add)
		{
#if LITE
			Root = new RootElement("SMS Party Lite");
#else
			Root = new RootElement("SMS Party");
#endif
		}
		
		public void LoadGroups()
		{
			InvokeOnMainThread(()=> {
				foreach(var c in Contacts.GetAll())
				{
					this.AddGroup(c);
				}
			});
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);	
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Label Header
			if(this.ListGroupSection == null){
				var header = Settings.GenerateHeaderFooter("Existing groups", LocalizedKey);
				
				this.ListGroupSection = new Section(header);
				Root.Add(this.ListGroupSection);
			}
			
#if LITE
			List<UIBarButtonItem> defaultBar = new List<UIBarButtonItem>(this.defaultBarButtonItems);
			UIBarButtonItem full = new UIBarButtonItem(UIImage.FromBundle("Images/shopping.png"), UIBarButtonItemStyle.Plain, null);
			full.Clicked+=  (sender, evt) => {
				UIAlertView goToPaid = new UIAlertView("Go to SMS Party full version", "You are about to go to the App Store", null, "No", "Yes");
				goToPaid.Clicked += (sndr, e) =>
				{
					if(e.ButtonIndex == goToPaid.CancelButtonIndex) return;
					string url = string.Format(Appirater.TEMPLATE_VIEW_URL,526844540);
					InvokeOnMainThread(()=> {
						UIApplication.SharedApplication.OpenUrl (NSUrl.FromString (url));});
				};
				goToPaid.Show();                       
			};
			
			defaultBar.Insert(0, full);
			this.defaultBarButtonItems = defaultBar.ToArray();
#endif
			ThreadPool.QueueUserWorkItem ((e) => {
          		LoadGroups();
			});
			
		}
		
		public void CreateButtonTapped()
		{	
			this.NavigationController.PushViewController(new ContactListController(this), false);
			TransitionManager.AddNewGroup(this.NavigationController);
		}

		public void AddGroup(SmsGroupObject sms)
		{
			GroupDetailElement e = new GroupDetailElement(this, sms);
			this.ListGroupSection.Add(e);
			this.ReloadData();
		}
		
		public void RemoveGroup(string name)
		{
			Element e = this.ListGroupSection.Elements.FirstOrDefault(x => ((GroupDetailElement)x).Sms.Name.Equals(name));
			if(e != null)
			{
				this.ListGroupSection.Remove(e);
			}
		}
		
		public override void HandleAddButtonClicked (object sender, EventArgs e)
		{
#if LITE
			if(Contacts.GetAll().Count >= Settings.MaxGroupFreeVersion)
			{
				UIAlertView alert = new UIAlertView(Settings.GetLocalizedString("SMS Party Free", LocalizedKey),
				                                    Settings.GetLocalizedString("SMS Party Free only allows a maximum of 5 groups to be created", LocalizedKey),
				                                    null, "OK");
				alert.Show();
				alert.Dispose();
				return;
			}
#endif
			CreateButtonTapped();
		}
		
	}
}
