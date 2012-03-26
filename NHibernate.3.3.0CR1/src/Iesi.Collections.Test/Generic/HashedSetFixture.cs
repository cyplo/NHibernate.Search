using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using IESI = Iesi.Collections.Generic;

namespace Iesi.Collections.Test.Generic
{
	/// <summary>
	/// Summary description for HashedSetFixture.
	/// </summary>
	[TestFixture]
	public class HashedSetFixture : GenericSetFixture
	{
        protected override IESI.ISet<string> CreateInstance()
		{
            return new IESI.HashedSet<string>();
		}

        protected override IESI.ISet<string> CreateInstance(ICollection<string> init)
		{
            return new IESI.HashedSet<string>(init);
		}

		protected override Type ExpectedType
		{
            get { return typeof(IESI.HashedSet<string>); }
		}

		[Test]
		public void Serialization()
		{
			// serialize and then deserialize the ISet.
			Stream stream = new MemoryStream();
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, _set);
			stream.Position = 0;

			IESI.ISet<string> desSet = (IESI.ISet<string>) formatter.Deserialize(stream);
			stream.Close();

			Assert.AreEqual(3, desSet.Count, "should have des 3 items");
			Assert.IsTrue(desSet.Contains(one), "should contain one");
			Assert.IsTrue(desSet.Contains(two), "should contain two");
			Assert.IsTrue(desSet.Contains(three), "should contain three");
		}
	}
}