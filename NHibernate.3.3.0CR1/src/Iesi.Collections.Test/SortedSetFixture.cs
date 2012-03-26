using System;
using IESI = Iesi.Collections;

using NUnit.Framework;
using System.Collections;

namespace Iesi.Collections.Test
{
	/// <summary>
	/// Summary description for SortedSetFixture.
	/// </summary>
	[TestFixture]
	public class SortedSetFixture : SetFixture
	{
		protected override ISet CreateInstance()
		{
			return new SortedSet();
		}

		protected override ISet CreateInstance(ICollection init)
		{
            return new IESI.SortedSet(init);
		}

		protected override Type ExpectedType
		{
            get { return typeof(IESI.SortedSet); }
		}

		[Test]
		public void OrderedEnumeration()
		{
			ArrayList expectedOrder = new ArrayList(3);
			expectedOrder.Add(one);
			expectedOrder.Add(two);
			expectedOrder.Add(three);
			expectedOrder.Sort();

			int index = 0;
			foreach (object obj in _set)
			{
				Assert.AreEqual(obj, expectedOrder[index], index.ToString() + " did not have same value");
				index++;
			}
		}

		[Test]
		public void OrderedCaseInsensitiveEnumeration()
		{
			ArrayList expectedOrder = new ArrayList(3);
			expectedOrder.Add("ONE");
			expectedOrder.Add("two");
			expectedOrder.Add("tHree");

			SortedSet theSet = new SortedSet(expectedOrder, new CaseInsensitiveComparer());

			expectedOrder.Sort(new CaseInsensitiveComparer());

			int index = 0;
			foreach (object obj in theSet)
			{
				Assert.AreEqual(obj, expectedOrder[index], index.ToString() + " did not have same value");
				index++;
			}
		}
	}
}