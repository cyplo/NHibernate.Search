using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.EntityModeTest.Multi
{
	public class Stock
	{
		private IESI.ISet<Valuation> valuations = new IESI.HashedSet<Valuation>();

		public virtual long Id { get; set; }

		public virtual string TradeSymbol { get; set; }

		public virtual Valuation CurrentValuation { get; set; }

		public virtual IESI.ISet<Valuation> Valuations
		{
			get { return valuations; }
			set { valuations = value; }
		}
	}
}