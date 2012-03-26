using System.Collections.Generic;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Insertordering
{
	public class User
	{
		private IESI.ISet<Membership> memberships;
		public User()
		{
			memberships = new IESI.HashedSet<Membership>();
		}
		public virtual int Id { get; protected set; }
		public virtual string UserName { get; set; }
		public virtual IEnumerable<Membership> Memberships
		{
			get { return memberships; }
		}

		public virtual Membership AddMembership(Group group)
		{
			var membership = new Membership(this, group);
			memberships.Add(membership);
			return membership;
		}
	}
}