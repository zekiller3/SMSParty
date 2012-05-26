using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.MessageUI;
using MonoTouch.AddressBook;
namespace SmsGroup
{
	public class BatchMFMessageComposeViewController : MFMessageComposeViewController
	{
		public MainScreenGroup Parent {get;set;}
		public int BatchSize {get;set;}
		public int IndexSent {get;set;}
		public bool MagicExpressionEnabled {get;set;}
		public Stack<ABPerson> LeftRecipients;
		public bool HasRecipientsLeft
		{
			get
			{
				return LeftRecipients.Count > 0;
			}
		}
		public BatchMFMessageComposeViewController (MainScreenGroup parent, ABPerson[] recipients, string body, int batchSize, bool magicExpressionEnabled) : base()
		{
			this.Parent = parent;
			IndexSent = 0;
			this.Body = body;
			this.BatchSize = batchSize;
			LeftRecipients = new Stack<ABPerson>(recipients);
			this.MessageComposeDelegate = new CustomMessageComposeDelegate();
			this.MagicExpressionEnabled = magicExpressionEnabled;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			ABPerson person = null;
			List<string> tempRecipients = new List<string>();
			for(int i = 0; i< BatchSize; i ++)
			{
				if(LeftRecipients.Count == 0)
				{
					break;
				}
				
				person = LeftRecipients.Pop();
				string rec = person.GetPhones().First(x => x.Label.ToString().ToLower().Contains("mobile")).Value;
				tempRecipients.Add(rec);
			}
			
			this.Recipients = tempRecipients.ToArray();
#if FULL
			// Replace magic expression
			if(this.MagicExpressionEnabled && person != null)
			{
				this.Body = this.Body.Replace(Settings.MagicExpressionFirstName, person.FirstName)
					.Replace(Settings.MagicExpressionLastName, person.LastName);
			}
#endif	
			base.ViewWillAppear (animated);
		}
	}
	
	public class CustomMessageComposeDelegate : MFMessageComposeViewControllerDelegate
	{
		
		
		public override void Finished (MFMessageComposeViewController controller, MessageComposeResult result)
		{
			Console.WriteLine("Custom, Finished");
			BatchMFMessageComposeViewController c = controller as BatchMFMessageComposeViewController;
			if(c != null && c.LeftRecipients.Count == 0)
			{
				c.Dispose();
			}
		}
	}
}

