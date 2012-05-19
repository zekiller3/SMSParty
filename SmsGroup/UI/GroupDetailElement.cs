using System;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
using System.Threading;
using MonoTouch.CoreAnimation;
namespace SmsGroup
{
	public class GroupDetailElement : StyledStringElement
	{
		private const string LocalizedKey = "GroupDetailElement";
		private ContactListController contactListController = null;
		private SmsComposerViewController smsComposerViewController = null;
		
		private MainScreenGroup parent;
		private SmsGroupObject sms = null;
		public SmsGroupObject Sms {
			get{
				return this.sms;
			}
			
			set{
				this.sms = value;
				this.Caption = 
				sms.Name.Length > 24 ? 
					sms.Name.Substring(0, 24) + "..." :
					sms.Name;
				this.Value = string.Format("{0} {1}",sms.Count, sms.Count > 1? 
				                           Settings.GetLocalizedString("contacts", LocalizedKey) : 
				                           Settings.GetLocalizedString("contact", LocalizedKey));
			}
		}
		public GroupDetailElement (MainScreenGroup parent, SmsGroupObject sms) : base("")
		{
			this.parent = parent;
			this.Sms = sms;
			this.style = UITableViewCellStyle.Subtitle;
			this.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
			this.AccessoryTapped += ShowGroupDetail;
			this.Tapped += SendSms;
			this.LineBreakMode = UILineBreakMode.TailTruncation;
		}
		
		public void SendSms()
		{
			if(!parent.tableView.Editing)
			{
				if(this.Sms.Count > 0){
					TransitionManager.SendSms(this.parent.NavigationController);
					if(smsComposerViewController == null)
					{
						smsComposerViewController = new SmsComposerViewController(parent, Sms);
					}
					this.parent.NavigationController.PushViewController(smsComposerViewController, false);
				}
			}
		}
		
		public void ShowGroupDetail()
		{
			if(!this.parent.tableView.Editing){
				TransitionManager.EditGroup(this.parent.NavigationController);
				if(contactListController == null)
				{
					contactListController = new ContactListController(parent, this);
				}
				
				this.parent.NavigationController.PushViewController(contactListController, false);
			}
		}
		
		public void Delete()
		{
			Contacts.Remove(this.Sms.Name);
		}
	}
}

