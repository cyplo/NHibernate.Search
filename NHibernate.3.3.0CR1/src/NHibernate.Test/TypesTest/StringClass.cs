using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringClass.
	/// </summary>
	public class StringClass
	{
		private int _id;
		private string _stringValue;
		private string _longStringValue;

		public StringClass()
		{
			_id = 0;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string StringValue
		{
			get { return _stringValue; }
			set { _stringValue = value; }
		}

		public string LongStringValue
		{
			get { return _longStringValue; }
			set { _longStringValue = value; }
		}
	}
}