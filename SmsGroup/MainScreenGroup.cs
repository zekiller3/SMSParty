using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.AddressBookUI;
namespace SmsGroup
{
	public partial class MainScreenGroup : BaseDialogViewController
	{
		private const string LocalizedKey = "MainScreenGroup";
		public Section ListGroupSection {get;set;}
		public MainScreenGroup () : base (true)
		{
#if LITE
			Root = new RootElement("SMS Party Free");
#else
			Root = new RootElement("SMS Party");
#endif
			this.ListGroupSection = new Section(Settings.GetLocalizedString("Existing groups", LocalizedKey));
			Root.Add(this.ListGroupSection);
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
//			UIBarButtonItem contact = new UIBarButtonItem("Contact",UIBarButtonItemStyle.Bordered, ShowAddressBook);
//			UIBarButtonItem flex = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
//			UIBarButtonItem addButton =
//				new UIBarButtonItem(UIBarButtonSystemItem.Add, CreateButtonTapped);
//			this.ToolbarItems = new UIBarButtonItem[]{contact, flex, addButton};
			ThreadPool.QueueUserWorkItem ((e) => {
          		LoadGroups();
			});
			
		}
		
		void ShowAddressBook(object sender, EventArgs e)
		{
			//this.NavigationController.PushViewController(c, true);
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
