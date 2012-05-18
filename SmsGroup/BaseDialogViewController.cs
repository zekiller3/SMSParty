using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace SmsGroup
{
	public abstract class BaseDialogViewController : DialogViewController
	{
		public class MainScreenEditViewController : DialogViewController.Source
		{
			public MainScreenEditViewController (DialogViewController dvc) : base (dvc) {}
			
			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{

				// uncomment these lines to authorize delete only on blob and not "folder"
//				var section = Container.Root [indexPath.Section];
//				return section [indexPath.Row] is S3ObjectElement;
				return true;
			}
			
			
			public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// trivial implementation: show a delete button always
				//return (UITableViewCellEditingStyle)3;
				return UITableViewCellEditingStyle.Delete;
			}
			
			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if(editingStyle == UITableViewCellEditingStyle.Delete)
				{
					var section = Container.Root [indexPath.Section];
                	var element = section [indexPath.Row] as GroupDetailElement;
					element.Delete();
					section.Remove(element);
				}
			}
		}
		
		protected virtual UIBarButtonItem[] EditBarButtonItems {get;set;}
		protected UIBarButtonItem[] defaultBarButtonItems {get;set;}
		
		public BaseDialogViewController (bool isPushed) : base (UITableViewStyle.Grouped, null, isPushed)
		{
			EnableSearch = true;
			AutoHideSearch = false;
			defaultBarButtonItems = DefaultBarButton();
			EditBarButtonItems  = new UIBarButtonItem[0];
		}
		
		/// <summary>
		/// Defaults the bar button.
		/// Provides basic buttons
		/// </summary>
		/// <returns>
		/// The bar button.
		/// </returns>
		protected UIBarButtonItem[] DefaultBarButton()
		{
			UIBarButtonItem space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			UIBarButtonItem addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
			addButton.Clicked += HandleAddButtonClicked;
			addButton.SetBackgroundVerticalPositionAdjustment(UIScreen.MainScreen.Bounds.Width - 50, UIBarMetrics.Default);
			UIBarButtonItem[] buttons = new UIBarButtonItem[]{
				space,
				addButton
			};
			
			return buttons;
		}
		
		#region UI
		public override Source CreateSizingSource (bool unevenRows)
		{
			//if (unevenRows)
			//	throw new NotImplementedException ("You need to create a new SourceSizing subclass, this sample does not have it");
			return new MainScreenEditViewController (this);
		}
		
		protected void ConfigEdit ()
		{
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Edit, delegate {
				// Activate editing
				this.TableView.SetEditing (true, true);
				this.NavigationItem.HidesBackButton = true;
				ConfigDone ();
				this.ToolbarItems = this.EditBarButtonItems;
				
			});
		}
		
		protected void ConfigDone ()
		{
			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
				// Deactivate editing
				this.TableView.SetEditing (false, true);
				this.NavigationItem.HidesBackButton = false;
				ConfigEdit ();
				this.ToolbarItems = defaultBarButtonItems;
			});
		}
		
		#endregion UI
		#region override
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);			
			this.NavigationController.ToolbarHidden = false;
			this.ToolbarItems = defaultBarButtonItems;
		}		
		
		public override void ViewDidAppear (bool animated)
		{
			if(this.TableView.Editing)
			{
				ConfigDone();
			}else
			{
				ConfigEdit();
			}
			base.ViewDidAppear(animated);
		}
		
		#endregion
		
		public abstract void HandleAddButtonClicked (object sender, EventArgs e);
	}
}
