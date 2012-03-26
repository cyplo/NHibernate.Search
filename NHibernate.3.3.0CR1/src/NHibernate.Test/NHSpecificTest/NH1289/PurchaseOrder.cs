using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	public class PurchaseOrder : WorkflowItem
	{
		public virtual IESI.ISet<PurchaseItem> PurchaseItems
		{
			get;
			set;
		}
	}
}