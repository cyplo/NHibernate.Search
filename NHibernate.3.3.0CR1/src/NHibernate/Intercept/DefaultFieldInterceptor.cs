using System;
using System.Collections.Generic;
using NHibernate.Engine;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Intercept
{
	[Serializable]
	public class DefaultFieldInterceptor : AbstractFieldInterceptor
	{
		public DefaultFieldInterceptor(ISessionImplementor session, IESI.ISet<string> uninitializedFields, IESI.ISet<string> unwrapProxyFieldNames, string entityName, System.Type mappedClass)
			: base(session, uninitializedFields, unwrapProxyFieldNames, entityName, mappedClass)
		{
		}
	}
}