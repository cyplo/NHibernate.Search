using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.CollectionTest
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual IDictionary<int, DateTime?> TypedDates { get; set; }
		public virtual IDictionary UntypedDates { get; set; }

		public Parent()
		{
			TypedDates = new Dictionary<int, DateTime?>();
			UntypedDates = new Hashtable();
		}
	}
}
