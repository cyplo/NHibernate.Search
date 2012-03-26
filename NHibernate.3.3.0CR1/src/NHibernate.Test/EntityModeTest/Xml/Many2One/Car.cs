using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.EntityModeTest.Xml.Many2One
{
	[Serializable]
	public class Car
	{
		private IESI.ISet<CarPart> carParts = new IESI.HashedSet<CarPart>();

		public virtual long Id { get; set; }
		public virtual string Model { get; set; }
		public virtual CarType CarType { get; set; }

		public IESI.ISet<CarPart> CarParts
		{
			get { return carParts; }
			set { carParts = value; }
		}
	}
}