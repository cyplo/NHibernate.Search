using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for AddClassFixture.
	/// </summary>
	[TestFixture]
	public class AddClassFixture
	{
		[Test]
		public void ClassMissingMappingFile()
		{
			Configuration cfg = new Configuration();
			try
			{
				cfg.AddClass(typeof(A));
			}
			catch (MappingException me)
			{
				Assert.AreEqual("Resource not found: " + typeof(A).FullName + ".hbm.xml", me.Message);
			}
		}

		[Test]
		public void AddClassNotFound()
		{
			Configuration cfg = new Configuration();
			try
			{
				cfg.AddResource("NHibernate.Test.MappingExceptions.A.ClassNotFound.hbm.xml", this.GetType().Assembly);
			}
			catch (MappingException me)
			{
				Assert.IsTrue(me.InnerException is MappingException);
				MappingException innerMe = (MappingException) me.InnerException;
				Assert.AreEqual("persistent class " + typeof(A).FullName + " not found", innerMe.Message);
			}
		}
	}
}