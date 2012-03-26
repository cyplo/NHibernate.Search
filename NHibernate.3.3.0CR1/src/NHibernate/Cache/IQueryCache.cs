using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines the contract for caches capable of storing query results.  These
	/// caches should only concern themselves with storing the matching result ids.
	/// The transactional semantics are necessarily less strict than the semantics
	/// of an item cache.
	/// </summary>
	public interface IQueryCache
	{
		ICache Cache { get;}
		string RegionName { get;}

		void Clear();
		bool Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session);
        IList Get(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, IESI.ISet<string> spaces, ISessionImplementor session);
		void Destroy();
	}
}