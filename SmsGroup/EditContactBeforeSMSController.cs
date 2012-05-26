using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading;
namespace SmsGroup
{
	public partial class EditContactBeforeSMSController : BaseDialogViewController
	{
		private const int SEGMENT_ALL = 0;
		private const int SEGMENT_NONE = 1;
		private const string LocalizedKey = "ContactListController";
		private SegmentedSection contact = null;
		public ABPerson[] Phones
		{
			get
			{
				if(contact != null){
					List<ABPerson> p = new List<ABPerson>();
					foreach(var element in contact.Elements)
					{
						ABPersonElement personElement = (ABPersonElement)element;
						if(personElement.IsChecked)
						{
							p.Add(personElement.Person);
						}
					}
					
					return p.ToArray();
				}
				
				return null;
			}
		}
		public SmsGroupObject Sms {get;set;}
		public EditContactBeforeSMSController (SmsGroupObject g) : base (true, ToolbarItemOption.Refresh, false)
		{
			Root = new RootElement("Edit");
			contact = new SegmentedSection("Contacts");
			this.Sms = g;
			
			contact.SegmentedControl.InsertSegment(
				Settings.GetLocalizedString("All", LocalizedKey),
				0,true);
			contact.SegmentedControl.InsertSegment(
				Settings.GetLocalizedString("None", LocalizedKey),
				1,true);
			contact.SegmentedControl.ValueChanged += HandleContactSegmentedControlAllTouchEvents;
			this.Root.Add(contact);
			Initialize();
		}

		public void Initialize()
		{
			if(contact != null)
			{
				contact.Clear();
			}
			
			contact.AddAll(Contacts.GetABPersonElementFrom(this.Sms, true));
		}
		
		void HandleContactSegmentedControlAllTouchEvents (object sender, EventArgs e)
		{
			switch(this.contact.SegmentedControl.SelectedSegment)
			{
				case SEGMENT_ALL:
					SelectAllContact ();
					break;
				case SEGMENT_NONE:
					SelectNoneContact();
					break;
				default:
					break;
			}
			
			this.tableView.ReloadRows(this.tableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
			BeginInvokeOnMainThread(()=>{
				Thread.Sleep(100);
				this.contact.SegmentedControl.SelectedSegment = -1;
			});
		}
		
		private void SelectAllContact()
		{
			foreach(var element in contact.Elements)
			{
				ABPersonElement p = (ABPersonElement)element;
				//p.Accessory  = UITableViewCellAccessory.Checkmark;
				p.IsChecked  = true;
			}
		}
		
		private void SelectNoneContact()
		{
			foreach(var element in contact.Elements)
			{
				ABPersonElement p = (ABPersonElement)element;
				//p.Accessory  = UITableViewCellAccessory.None;
				p.IsChecked = false;
				
			}
		}
		
		public override void HandleRefreshClicked (object sender, EventArgs e)
		{
			ThreadPool.QueueUserWorkItem ((evt) => {
				Contacts.RefreshAddressBook();
				InvokeOnMainThread(()=>{ 
					Initialize();
					this.ReloadData();
				});
			});
		}
	}
}
