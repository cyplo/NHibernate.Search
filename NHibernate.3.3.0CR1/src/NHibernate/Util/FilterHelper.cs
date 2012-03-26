using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Util
{
	/// <summary></summary>
	public sealed class FilterHelper
	{
		private readonly string[] filterNames;
		private readonly string[] filterConditions;

		public FilterHelper(IDictionary<string, string> filters, Dialect.Dialect dialect, SQLFunctionRegistry sqlFunctionRegistry)
		{
			int filterCount = filters.Count;
			filterNames = new string[filterCount];
			filterConditions = new string[filterCount];
			filterCount = 0;
			foreach (KeyValuePair<string, string> entry in filters)
			{
				filterNames[filterCount] = entry.Key;
				filterConditions[filterCount] =
					Template.RenderWhereStringTemplate(entry.Value, FilterImpl.MARKER, dialect, sqlFunctionRegistry);
				filterConditions[filterCount] =
					StringHelper.Replace(filterConditions[filterCount], ":", ":" + filterNames[filterCount] + ".");
				filterCount++;
			}
		}

		public bool IsAffectedBy(IDictionary<string, IFilter> enabledFilters)
		{
			for (int i = 0, max = filterNames.Length; i < max; i++)
			{
				if (enabledFilters.ContainsKey(filterNames[i]))
				{
					return true;
				}
			}
			return false;
		}

		public string Render(String alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder buffer = new StringBuilder();
			Render(buffer, alias, enabledFilters);
			return buffer.ToString();
		}

		public void Render(StringBuilder buffer, string alias, IDictionary<string, IFilter> enabledFilters)
		{
			if (filterNames != null && filterNames.Length > 0)
			{
				for (int i = 0, max = filterNames.Length; i < max; i++)
				{
					if (enabledFilters.ContainsKey(filterNames[i]))
					{
						string condition = filterConditions[i];
						if (StringHelper.IsNotEmpty(condition))
						{
							buffer.Append(" and ");
							buffer.Append(StringHelper.Replace(condition, FilterImpl.MARKER, alias));
						}
					}
				}
			}
		}

		/// <summary>
		/// Get only filters enabled for many-to-one association.
		/// </summary>
		/// <param name="enabledFilters">All enabled filters</param>
		/// <returns>A new <see cref="IDictionary{TKey,TValue}"/> for filters enabled for many to one.</returns>
		public static IDictionary<string, IFilter> GetEnabledForManyToOne(IDictionary<string, IFilter> enabledFilters)
		{
			var enabledFiltersForManyToOne = new Dictionary<string, IFilter>();
			foreach (var enabledFilter in enabledFilters)
			{
				if (enabledFilter.Value.FilterDefinition.UseInManyToOne)
					enabledFiltersForManyToOne.Add(enabledFilter.Key, enabledFilter.Value);
			}
			return enabledFiltersForManyToOne;
		}
	}
}