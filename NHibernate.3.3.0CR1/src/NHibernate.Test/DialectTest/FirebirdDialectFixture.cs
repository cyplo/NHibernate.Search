using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class FirebirdDialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			FirebirdDialect d = new FirebirdDialect();

			SqlString str = d.GetLimitString(new SqlString("SELECT * FROM fish"), null, new SqlString("10"));
			Assert.AreEqual("SELECT first 10 * FROM fish", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("5"), new SqlString("15"));
			Assert.AreEqual("SELECT first 15 skip 5 * FROM fish ORDER BY name", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name DESC"), new SqlString("7"), new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 * FROM fish ORDER BY name DESC", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), null, new SqlString("28"));
			Assert.AreEqual("SELECT first 28 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), new SqlString("7"), new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());
		}
	}
}