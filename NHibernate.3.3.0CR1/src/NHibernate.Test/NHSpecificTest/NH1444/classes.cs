using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1444
{
	public class xparent
	{
		public virtual long Id { get; set; }
		public virtual long? A { get; set; }
		public virtual IESI.ISet<xchild> Children { get; set; }
	}

	public class xchild
	{
		public virtual long Id { get; set; }
		public virtual long? B { get; set; }
		public virtual xparent Parent { get; set; }
	}
}