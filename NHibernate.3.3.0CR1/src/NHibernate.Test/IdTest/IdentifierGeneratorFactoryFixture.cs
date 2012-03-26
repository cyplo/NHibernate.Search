using NHibernate.Dialect;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class IdentifierGeneratorFactoryFixture
	{
		/// <summary>
		/// Testing NH-325 to make sure that an exception is actually
		/// caught with a bad class name passed in.
		/// </summary>
		[Test]
		public void NonCreatableStrategy()
		{
			Assert.Throws<IdentifierGenerationException>(() => IdentifierGeneratorFactory.Create("Guid", NHibernateUtil.Guid, null, new MsSql2000Dialect()),
				"Could not interpret id generator strategy: Guid");
		}
	}
}