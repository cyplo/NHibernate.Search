using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for DoubleClass.
	/// </summary>
	public class DoubleClass
	{
		private int _id;
		private Double _doubleValue;

		public DoubleClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public double DoubleValue
		{
			get { return _doubleValue; }
			set { _doubleValue = value; }
		}
	}
}