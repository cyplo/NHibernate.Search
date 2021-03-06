using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.DynamicEntity
{
	public interface Person
	{
		long Id { get; set;}
		string Name { get; set;}
		Address Address { get;set;}
		IESI.ISet<Person> Family { get; set;}
	}
}