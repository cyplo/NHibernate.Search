using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Interceptor
{
	public class User
	{
		private IESI.ISet<string> actions = new IESI.HashedSet<string>();
		private DateTime? created;
		private DateTime? lastUpdated;
		private string name;
		private string password;

		public User() {}

		public User(string name, string password)
		{
			this.name = name;
			this.password = password;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Password
		{
			get { return password; }
			set { password = value; }
		}

		public virtual IESI.ISet<string> Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		public virtual DateTime? LastUpdated
		{
			get { return lastUpdated; }
			set { lastUpdated = value; }
		}

		public virtual DateTime? Created
		{
			get { return created; }
			set { created = value; }
		}
	}
}