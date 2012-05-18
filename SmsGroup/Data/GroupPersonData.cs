using System;
using System.Linq;
using SQLite;
using MonoTouch.AddressBook;

namespace SmsGroup
{
	public class GroupPersonData
	{
		private ABPerson person;
		public GroupPersonData ()
		{
		}
		
		[PrimaryKey, AutoIncrement]
		public int Id {get;set;}
		public string GroupName {get;set;}
		public int ABPersonId {get;set;}
		public int PhoneId {get;set;}
	}
}

