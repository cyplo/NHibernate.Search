using System;
using System.Collections.Generic;
using NUnit.Framework;
using IESI = Iesi.Collections.Generic;
namespace Iesi.Collections.Test.Generic
{
	/// <summary>
	/// Summary description for HashedSetFixture.
	/// </summary>
	[TestFixture]
	public class ImmutableSetFixture : GenericSetFixture
	{
        protected override IESI.ISet<string> CreateInstance()
		{
            return new IESI.ImmutableSet<string>(new IESI.HashedSet<string>());
		}

        protected override IESI.ISet<string> CreateInstance(ICollection<string> init)
		{
            return new IESI.ImmutableSet<string>(new IESI.HashedSet<string>(init));
		}

		protected override Type ExpectedType
		{
            get { return typeof(IESI.ImmutableSet<string>); }
		}
	}
}