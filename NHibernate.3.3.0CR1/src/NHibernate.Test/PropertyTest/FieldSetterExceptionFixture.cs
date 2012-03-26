using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the exception messages that come out a FieldSetter when
	/// invalid values are passed in.
	/// </summary>
	[TestFixture]
	public class FieldSetterExceptionFixture
	{
		protected IPropertyAccessor _accessor;
		protected ISetter _setter;

		[SetUp]
		public void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field");
			_setter = _accessor.GetSetter(typeof(A), "Id");
		}

		[Test]
		public void SetInvalidType()
		{
			A instance = new A();
			var e = Assert.Throws<PropertyAccessException>(() =>_setter.Set(instance, "wrong type"));
			Assert.That(e.Message, Is.EqualTo("The type System.String can not be assigned to a field of type System.Int32 setter of NHibernate.Test.PropertyTest.FieldSetterExceptionFixture+A.Id"));
		}

		public class A
		{
			public int Id = 0;
		}
	}
}