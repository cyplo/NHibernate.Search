using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.GenericTest.OrderedSetGeneric
{
	public class A
	{
        private IESI.ISet<B> _items = new IESI.OrderedSet<B>();

		public int Id { get; set; }

		public string Name { get; set; }

		public virtual IESI.ISet<B> Items
		{
			get { return _items; }
			set { _items = value; }
		}
	}
}
