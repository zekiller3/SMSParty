using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading;
using System.Drawing;
namespace SmsGroup
{
	public partial class ContactListController : BaseDialogViewController
	{	
		private const string LocalizedKey = "ContactListController";
		private const int SEGMENT_ALL = 0;
		private const int SEGMENT_NONE = 1;
		
		public MainScreenGroup Parent {get;set;}
		
		private EntryElement nameElement = null;
		private SegmentedSection contactSection = null;
		/// <summary>
		/// If true, the controller has been invoked to edit an existing group, false to create a new group.
		/// </summary>
		private bool isEditing = false;
		private GroupDetailElement smsGroup = null;
		public ContactListController (MainScreenGroup parent, GroupDetailElement smsGroup = null) : base (true, false)
		{
			this.EnableSearch = true;
			this.AutoHideSearch = true;
			this.Parent = parent;
			this.smsGroup = smsGroup;
			
			Root = new RootElement (Settings.GetLocalizedString("Contact List", LocalizedKey));
			this.isEditing = this.smsGroup != null;
			Section groupName = new Section(
				Settings.GetLocalizedString("Group Name", LocalizedKey),
			    Settings.GetLocalizedString("The name that will best describe your group", LocalizedKey));
			nameElement = new EntryElement(
				Settings.GetLocalizedString("Name", LocalizedKey),
				Settings.GetLocalizedString("Group Name", LocalizedKey), smsGroup != null && smsGroup.Sms != null ? smsGroup.Sms.Name : "");
			nameElement.ClearButtonMode = UITextFieldViewMode.WhileEditing;
			groupName.Add(nameElement);
			
			//SegmentedSection
			contactSection = new SegmentedSection("Contacts");
			contactSection.Footer = 
				Settings.GetLocalizedString("Displays only contacts that have a mobile phone set", LocalizedKey);
			contactSection.SegmentedControl.InsertSegment(
				Settings.GetLocalizedString("All", LocalizedKey),
				0,true);
			contactSection.SegmentedControl.InsertSegment(
				Settings.GetLocalizedString("None", LocalizedKey),
				1,true);
			contactSection.SegmentedControl.ValueChanged += HandleContactSectionSegmentedControlTouchUpInside;
			
			ThreadPool.QueueUserWorkItem ((e) => {
				InvokeOnMainThread(()=>{ 
					Initialize();
					this.ReloadData();
				});
			});
			
			Root.Add(groupName);
			Root.Add(contactSection);
			
			UIBarButtonItem done = new UIBarButtonItem(UIBarButtonSystemItem.Done);
			done.Clicked += HandleDoneClicked;
			this.NavigationItem.RightBarButtonItem = done;
			UIBarButtonItem space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			refresh = new UIBarButtonItem(UIBarButtonSystemItem.Refresh);
			
			refresh.Clicked += HandleRefreshClicked;
			InvokeOnMainThread(() => {
				activityView = new UIActivityIndicatorView(new RectangleF(0,0,25,25));
				
				activityView.Hidden = false;				
				activityView.HidesWhenStopped = true;
				activityView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
				//loading = new UIBarButtonItem(activityView);
				this.defaultBarButtonItems = new []{space, refresh};
			});
		}
		
		private UIActivityIndicatorView activityView = null;
		private UIBarButtonItem refresh = null;
		//private UIBarButtonItem loading = null;
		void HandleRefreshClicked (object sender, EventArgs e)
		{
				
			ThreadPool.QueueUserWorkItem ((evt) => {
				Contacts.RefreshAddressBook();
				InvokeOnMainThread(()=>{ 
					Initialize();
					this.ReloadData();
				});
			});
		}
		
		void Initialize()
		{
			List<Element> personElements = new List<Element>();
			foreach(var a in Contacts.AddressBook.Where (x => x.Type == ABRecordType.Person))
			{
				ABPerson person = a as ABPerson;
				
				foreach(var phone in person.GetPhones())
				{
					if(phone.Label.ToString().ToLower().Contains("mobile")){
						bool isInGroup = false;
						try{
							isInGroup = (smsGroup != null && smsGroup.Sms != null && smsGroup.Sms.Persons.Any(x => x.Id == a.Id));
						}catch(Exception ex)
						{
							Console.WriteLine(ex);
						}
						
						ABPersonElement el = new ABPersonElement(phone.Value, isInGroup, person);
						personElements.Add(el);	
					}
				}
			}
			contactSection.Elements = personElements.OrderBy(x => ((ABPersonElement)x).Person.FirstName).ToList<Element>();
		}
		
		void HandleContactSectionSegmentedControlTouchUpInside (object sender, EventArgs e)
		{
			switch(this.contactSection.SegmentedControl.SelectedSegment)
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
			
			List<NSIndexPath> visibleindex = this.tableView.IndexPathsForVisibleRows.ToList();
			if(visibleindex.Contains(nameElement.IndexPath))
			{
				visibleindex.Remove(nameElement.IndexPath);	
			}
			this.tableView.ReloadRows(visibleindex.ToArray(), UITableViewRowAnimation.None);
			BeginInvokeOnMainThread(()=>{
				Thread.Sleep(100);
				this.contactSection.SegmentedControl.SelectedSegment = -1;
			});
		}
		
		private void SelectAllContact()
		{
			foreach(var element in contactSection.Elements)
			{
				ABPersonElement p = (ABPersonElement)element;
				//p.Accessory  = UITableViewCellAccessory.Checkmark;
				p.IsChecked  = true;
			}
		}
		
		private void SelectNoneContact()
		{
			foreach(var element in contactSection.Elements)
			{
				ABPersonElement p = (ABPersonElement)element;
				//p.Accessory  = UITableViewCellAccessory.None;
				p.IsChecked = false;
				
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			activityView.StartAnimating();
		}
		
		void HandleDoneClicked (object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(nameElement.Value))
			{
				UIAlertView alert = new UIAlertView(
					Settings.GetLocalizedString("Group Name Error",LocalizedKey),
				    Settings.GetLocalizedString("You must specify a group name", LocalizedKey), null, "OK");
				alert.Show();
				return;
			}
			
			// 3 cases :
			// 1. New Group = check that the group does not exist, prompt if exist
			// 2. Editing = Update everything..
			// 3. Edit group result in overriding existing group
			if(!isEditing){
				if(Contacts.GetAll().Any(x => x.Name.ToLower().Equals(nameElement.Value.ToLower())))
				{
					UIAlertView alert = new UIAlertView(
						Settings.GetLocalizedString("Group Name Error", LocalizedKey),
						Settings.GetLocalizedString("Can't create the group because a group with the same name exists", LocalizedKey),
						null, "OK");
					alert.Show();
					return;
				}
			}
			else
			{
				if(this.smsGroup.Sms.Name.ToLower() != nameElement.Value.ToLower() && Contacts.GetAll().Any(x => x.Name.ToLower().Equals(nameElement.Value.ToLower())))
				{
					UIAlertView alert = new UIAlertView(
						Settings.GetLocalizedString("Group Name Error", LocalizedKey),
						Settings.GetLocalizedString("Can't update the group because a group with the same name exists", LocalizedKey),
						null, "OK");
					alert.Show();
					return;
				}
			}
			
			SaveGroup();
			ReturnToMainScreen();
		}
		
		public List<ABPerson> GetCheckedContact()
		{
			List<ABPerson> result = new List<ABPerson>();
			foreach(var el in contactSection.Elements)
			{
				ABPersonElement check = el as ABPersonElement;
				if(check.IsChecked)
				{
					result.Add(check.Person);
				}
			}
			
			return result;
		}
		
		// add a new group!
		public void SaveGroup()
		{
			if(contactSection != null && nameElement != null)
			{
				// adds to the data holder
				SmsGroupObject g = new SmsGroupObject();
				g.Name = nameElement.Value;	
				g.Persons = GetCheckedContact();
				
				if(isEditing)
				{
					Contacts.Remove(this.smsGroup.Sms.Name);
					// it will update if exists
					Contacts.Add(g);
					// check if the group exist, update its inner data with new group
					foreach(var element in Parent.ListGroupSection.Elements)
					{
						GroupDetailElement detail = (GroupDetailElement)element; 
						if(detail.Sms.Name == this.smsGroup.Sms.Name)
						{
							InvokeOnMainThread(()=> {
								detail.Sms = g;
								Parent.ReloadData();
							});
							// Updates UI here, if needed!
							break;
						}
					}
				}
				else
				{
					Contacts.Add(g);
					Parent.AddGroup(g);
				}
			}
		}
		
		public void ReturnToMainScreen()
		{
			this.NavigationController.PopToRootViewController(false);
		}
		
		#region abstract members
		
		#endregion
	}
}
