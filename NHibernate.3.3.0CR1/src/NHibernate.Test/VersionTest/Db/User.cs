using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.VersionTest.Db
{
	public class User
	{
		public virtual long Id { get; set; }

		public virtual DateTime Timestamp { get; set; }

		public virtual string Username { get; set; }

		public virtual IESI.ISet<Group> Groups { get; set; }

		public virtual IESI.ISet<Permission> Permissions { get; set; }
	}
}