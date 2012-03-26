using System;
using System.Collections.Generic;
using NUnit.Framework;
using IESI = Iesi.Collections.Generic;

namespace Iesi.Collections.Test.Generic
{
	/// <summary>
	/// Summary description for OrderedSetFixture.
	/// </summary>
	[TestFixture]
	public class OrderedSetFixture : GenericSetFixture
	{
        protected override IESI.ISet<string> CreateInstance()
		{
			return new IESI.OrderedSet<string>();
		}

        protected override IESI.ISet<string> CreateInstance(ICollection<string> init)
		{
            return new IESI.OrderedSet<string>(init);
		}

		protected override Type ExpectedType
		{
            get { return typeof(IESI.OrderedSet<string>); }
		}

		[Test]
		public void OrderedEnumeration()
		{
			List<string> expectedOrder = new List<string>(3) {one, two, three};

			int index = 0;
			foreach (string str in _set)
			{
				Assert.AreEqual(str, expectedOrder[index], index + " did not have same value");
				index++;
			}
		}
	}
}
