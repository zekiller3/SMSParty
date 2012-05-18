using System;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.AddressBook;
using MonoTouch.UIKit;
namespace SmsGroup
{
	public class ABPersonElement : StyledStringElement
	{
		public ABPerson Person {get;set;}
		private bool isChecked = false;
		
		public bool IsChecked 
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				this.isChecked = value;
			}
		}
		
		public string Phone
		{
			get
			{
				return this.Person.GetPhones().First(x => x.Label.ToString().ToLower().Contains("mobile")).Value;
			}
		}
		
		public ABPersonElement (string phone, bool isInGroup, ABPerson person) : base(string.Format("{0} {1}",person.FirstName, person.LastName),phone,MonoTouch.UIKit.UITableViewCellStyle.Subtitle)
		{
			this.Person = person;
			IsChecked = isInGroup;
		}
		
		UITableViewCell ConfigCell (UITableViewCell cell)
		{
			cell.Accessory = IsChecked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			return cell;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			return  ConfigCell (base.GetCell (tv));
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			IsChecked = !IsChecked;
			var cell = tableView.CellAt (path);
			ConfigCell (cell);
			base.Selected (dvc, tableView, path);
		}
	}
}

