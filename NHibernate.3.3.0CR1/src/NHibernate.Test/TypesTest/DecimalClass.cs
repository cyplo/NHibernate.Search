using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for DecimalClass.
	/// </summary>
	public class DecimalClass
	{
		private int _id;
		private decimal _decimalValue;

		public DecimalClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public decimal DecimalValue
		{
			get { return _decimalValue; }
			set { _decimalValue = value; }
		}
	}
}