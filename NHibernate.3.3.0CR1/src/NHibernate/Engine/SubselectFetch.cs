using System.Collections.Generic;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Engine
{
	public class SubselectFetch
	{
		private readonly string alias;
		private readonly ILoadable loadable;
		private readonly QueryParameters queryParameters;
		private readonly SqlString queryString;
        private readonly IESI.ISet<EntityKey> resultingEntityKeys;

		public SubselectFetch(string alias, ILoadable loadable, QueryParameters queryParameters,
                              IESI.ISet<EntityKey> resultingEntityKeys)
		{
			this.resultingEntityKeys = resultingEntityKeys;
			this.queryParameters = queryParameters;
			this.loadable = loadable;
			this.alias = alias;

			queryString = queryParameters.ProcessedSql.GetSubselectString();
		}

		public QueryParameters QueryParameters
		{
			get { return queryParameters; }
		}

        public IESI.ISet<EntityKey> Result
		{
			get { return resultingEntityKeys; }
		}

		public SqlString ToSubselectString(string ukname)
		{
			string[] joinColumns = ukname == null
			                       	? StringHelper.Qualify(alias, loadable.IdentifierColumnNames)
			                       	: ((IPropertyMapping) loadable).ToColumns(alias, ukname);

			SqlString sqlString =
				new SqlStringBuilder().Add("select ").Add(StringHelper.Join(", ", joinColumns)).Add(queryString).ToSqlString();
			return sqlString;
		}

		public override string ToString()
		{
			return "SubselectFetch(" + queryString + ')';
		}
	}
}