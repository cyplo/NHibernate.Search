using System;
using System.Reflection;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;
using IESI = Iesi.Collections.Generic;


namespace NHibernate.Proxy.Map
{
	public class MapProxyFactory : IProxyFactory
	{
		private string entityName;

		#region IProxyFactory Members

		public void PostInstantiate(string entityName, System.Type persistentClass, IESI.ISet<System.Type> interfaces,
																MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
																IAbstractComponentType componentIdType)
		{
			this.entityName = entityName;
		}

		public INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			return new MapProxy(new MapLazyInitializer(entityName, id, session));
		}

		public object GetFieldInterceptionProxy(object getInstance)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
