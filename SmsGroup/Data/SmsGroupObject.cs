using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBook;
namespace SmsGroup
{
	public class SmsGroupObject
	{
		public string Name {get;set;}
		public List<ABPerson> Persons 
		{
			get
			{
				if(this.persons == null)
				{
					// Loads from database! it's lazy loading!
					this.persons = Contacts.GetPersonsFromGroup(this.Name);
				}
				
				return this.persons;
			}
			set
			{
				this.persons = value;
			}
		}
		
		private List<ABPerson> persons = null;
		
		public SmsGroupObject ()
		{
			persons = new List<ABPerson>();
		}
		
		public SmsGroupObject(string groupName, int tempCount)
		{
			this.Name = groupName;
			this.tempCount = tempCount;
		}
		
		private readonly int tempCount = -1;
		
		public int Count {
			get
			{
				if(this.persons == null)
				{
					return tempCount;
				}
				
				return this.Persons.Count;
			}
		}
		
		public void LoadPersons()
		{
			if(this.persons == null){
				this.persons = Contacts.GetPersonsFromGroup(this.Name);
			}
		}
		
		public string[] Phones
		{
			get
			{
				List<string> phones = new List<string>();
				foreach(ABPerson p in Persons)
				{
					foreach(var phone in p.GetPhones().Where(x => x.Label.ToString().ToLower().Contains("mobile")))
					{
						phones.Add(phone.Value);
					}
				}
				
				return phones.ToArray();
			}
		}
	}
}

