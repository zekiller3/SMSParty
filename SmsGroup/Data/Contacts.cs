using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.AddressBook;
using MonoTouch.CoreData;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace SmsGroup
{
	public class Contacts
	{
		private static GroupPersonDatabase connection;
		public static ABAddressBook AddressBook = new ABAddressBook();
		
		private static Dictionary<string,SmsGroupObject> Groups {get;set;}
		static Contacts ()
		{
			connection = new GroupPersonDatabase("db_smsgrouped.db3");
			Groups = new Dictionary<string,SmsGroupObject>();
			try
			{
				Load();
			}catch(Exception ex)
			{
				Console.WriteLine(ex);
				UIAlertView alert = new UIAlertView("Error Load()", ex.ToString(), null, "OK");
				alert.Show();
			}
		}
		
		
		// Loads data and convert it to SmsGrouObject
		private static void Load()
		{
			// Should just load basic data such as name and count...
			foreach(var sms in connection.GetSummarizedInfo())
			{
				Groups.Add(sms.Name, sms);
			}
		}
		
		public static List<ABPerson> GetPersonsFromGroup(string groupName)
		{
			List<ABPerson> result = new List<ABPerson>();
			foreach(var g in connection.GetPersonsFromGroup(groupName))
			{
				ABPerson p = null;
				IEnumerable<ABRecord> record = AddressBook.Where(x => x.Id == g.ABPersonId);
				if(record != null)
				{
					p = record.FirstOrDefault() as ABPerson;
				}
				
				// ABPerson is unfindable! remove it from database!
				if(p == null)
				{
					int removedId = connection.RemoveGroupPerson(g);
				}
				
				result.Add(p);
			}
			
			return result;
		}
		
		public void LoadAllDatabase()
		{
			foreach(var g in connection.GetAll())
			{
				//Map the stored id with the addressbook id.
				ABPerson p = null;
				IEnumerable<ABRecord> record = AddressBook.Where(x => x.Id == g.ABPersonId);
				if(record != null)
				{
					p = record.FirstOrDefault() as ABPerson;
				}
				
				// ABPerson is unfindable! remove it from database!
				if(p == null)
				{
					int removedId = connection.RemoveGroupPerson(g);	
				}
				
				AddPersonToGroup(g.GroupName, p);
			}
		}
		
		//Get all objects
		public static List<SmsGroupObject> GetAll()
		{
			return Groups.Values.ToList();
		}
		
		/// <summary>
		/// Add the specified g and force.
		/// </summary>
		/// <param name='g'>
		/// The group to add.
		/// </param>
		public static void Add(SmsGroupObject g)
		{
			
			//deletes if exists
			if(Groups.ContainsKey(g.Name))
			{
				Remove (g.Name);
				Groups[g.Name] = g;
			}else
			{
				Groups.Add(g.Name, g);
			}
			
			//inserts it anyway
			DatabaseInsert(g);
		}
		
		public static bool Contains(string groupName)
		{
			return Groups.ContainsKey(groupName);
		}
		
		// delete from database
		public static bool Remove(string groupName)
		{
			if(Groups.ContainsKey(groupName))
			{
				connection.RemoveGroup(groupName);
				return Groups.Remove(groupName);	
			}
			
			return false;
		}
		
		private static bool AddPersonToGroup(string groupname, ABPerson p)
		{
			if(!Groups.ContainsKey(groupname))
			{
				Groups.Add(groupname, new SmsGroupObject() {Name = groupname});
			}
			
			if(!Groups[groupname].Persons.Any(x => x.Id == p.Id))
			{
				Groups[groupname].Persons.Add(p);
				return true;
			}
			
			return false;
		}
		
		private static void DatabaseInsert(SmsGroupObject g)
		{
			foreach(var p in g.Persons)
			{
				var data = new GroupPersonData(){ GroupName = g.Name, ABPersonId = p.Id };
				connection.AddGroupPerson(data);
			}
		}
		
		public static void RefreshAddressBook()
		{
			if(AddressBook != null)
			{
				AddressBook.Dispose();
			}
			
			AddressBook = new ABAddressBook();
			
		}
		
		private static void DatabaseRemove(SmsGroupObject g)
		{
			
		}
	}
}

