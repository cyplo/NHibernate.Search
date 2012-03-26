using System;
using System.Collections.Generic;
using IESI = Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// Convenience base class for <see cref="IAuxiliaryDatabaseObject">AuxiliaryDatabaseObjects</see>.
	/// </summary>
	/// <remarks>
	/// This implementation performs dialect scoping checks strictly based on
	/// dialect name comparisons.  Custom implementations might want to do
	/// instanceof-type checks. 
	/// </remarks>
	[Serializable]
	public abstract class AbstractAuxiliaryDatabaseObject : IAuxiliaryDatabaseObject
	{
        private readonly IESI.HashedSet<string> dialectScopes;
		private IDictionary<string, string> parameters = new Dictionary<string, string>();

		protected AbstractAuxiliaryDatabaseObject()
		{
            dialectScopes = new IESI.HashedSet<string>();
		}

		protected AbstractAuxiliaryDatabaseObject(IESI.HashedSet<string> dialectScopes)
		{
			this.dialectScopes = dialectScopes;
		}

		public void AddDialectScope(string dialectName)
		{
			dialectScopes.Add(dialectName);
		}

		public IESI.HashedSet<string> DialectScopes
		{
			get { return dialectScopes; }
		}

		public IDictionary<string, string> Parameters
		{
			get { return parameters; }
		}

		public bool AppliesToDialect(Dialect.Dialect dialect)
		{
			// empty means no scoping
			return dialectScopes.IsEmpty || dialectScopes.Contains(dialect.GetType().FullName);
		}

		public abstract string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema);
		public abstract string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema);

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			this.parameters = parameters;
		}

	}
}