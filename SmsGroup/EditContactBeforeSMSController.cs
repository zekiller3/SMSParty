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
	public partial class EditContactBeforeSMSController : DialogViewController
	{
		private const int SEGMENT_ALL = 0;
		private const int SEGMENT_NONE = 1;
		
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
		
		public EditContactBeforeSMSController (SmsGroupObject g) : base (UITableViewStyle.Grouped, null, true)
		{
			Root = new RootElement("Edit");
			contact = new SegmentedSection("Contacts");
			foreach(var person in g.Persons)
			{
				foreach(var phone in person.GetPhones())
				{			
					if(phone.Label.ToString().ToLower().Contains("mobile")){
						ABPersonElement el = new ABPersonElement(phone.Value, true, person);
						contact.Add(el);
					}
				}
			}
			
			contact.SegmentedControl.InsertSegment("All",0,true);
			contact.SegmentedControl.InsertSegment("None",1,true);
			contact.SegmentedControl.ValueChanged += HandleContactSegmentedControlAllTouchEvents;
			this.Root.Add(contact);
		}

		void HandleContactSegmentedControlAllTouchEvents (object sender, EventArgs e)
		{
			Console.WriteLine("EditContact SegmentedControl : {0}", this.contact.SegmentedControl.SelectedSegment);
			
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
			
			this.TableView.ReloadRows(this.TableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
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
	}
}
