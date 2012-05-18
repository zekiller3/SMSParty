using System.Linq;
using System.Collections.Generic;
using System;
using SQLite;
using System.IO;
namespace SmsGroup
{
	public class GroupPersonDatabase : SQLiteConnection
	{
		protected string GroupPersonTableName {
			get
			{
				if(string.IsNullOrEmpty(this.groupPersonTableName))
				{
					this.groupPersonTableName = this.Table<GroupPersonData>().Table.TableName;
				}
				
				return this.groupPersonTableName;
			}
		}
		
		private string groupPersonTableName = string.Empty;
		
		// This method checks to see if the database exists, and if it doesn't, it creates
		// it and inserts some data
		protected void CheckAndCreateDatabase (string dbName)
		{
			// create a connection object. if the database doesn't exist, it will create 
			// a blank database
			using(SQLiteConnection db = new SQLiteConnection (GetDBPath (dbName)))
			{				
				// create the tables
				db.CreateTable<GroupPersonData> ();

				// close the connection
				db.Close ();
			}
		}
		
		protected static string GetDBPath (string dbName)
		{
			// get a reference to the documents folder
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

			// create the db path
			string db = Path.Combine (documents, dbName);

			return db;
		}
		
		public GroupPersonDatabase (string path) : base(GetDBPath(path))
		{
			CheckAndCreateDatabase(path);
		}
		
		public IEnumerable<SmsGroupObject> GetSummarizedInfo()
		{
			List<SmsGroupObject> result = new List<SmsGroupObject>();
			string query = string.Format ("select Distinct(GroupName), count(*) as Count from \"{0}\" group by GroupName", this.GroupPersonTableName);
			var obj = this.Query<SummarizedInfo>(query);
			foreach(var o in obj)
			{
				result.Add(new SmsGroupObject(o.GroupName, o.Count));
			}
			
			return result;
		}
		
		public IEnumerable<GroupPersonData> GetAll()
		{
			IEnumerable<GroupPersonData> groups = (from i in this.Table<GroupPersonData>() select i);
			return groups;
		}
		
		public IEnumerable<GroupPersonData> GetPersonsFromGroup(string groupName)
		{
			string query = string.Format ("select * from \"{0}\" where \"{1}\" = ?", this.GroupPersonTableName, "GroupName");
			return this.Query<GroupPersonData>(query, groupName);
		}
		
		public int AddGroupPerson(GroupPersonData g)
		{
			return Insert(g);
		}
		
		public int RemoveGroup(string groupName)
		{
			try{
				var q = string.Format ("delete from \"{0}\" where \"{1}\" = ?", GroupPersonTableName, "GroupName");
				return Execute(q, groupName);
			}catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			return -1;
		}
		
		public int RemoveGroupPerson(GroupPersonData g)
		{
			return Delete(g);
		}
	}
	
	public class SummarizedInfo
	{
		public SummarizedInfo()
		{
		}
		
		public string GroupName{get;set;}
		public int Count {get;set;}
	}
}

