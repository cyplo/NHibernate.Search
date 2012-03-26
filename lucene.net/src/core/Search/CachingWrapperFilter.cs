/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IndexReader = Lucene.Net.Index.IndexReader;
using DocIdBitSet = Lucene.Net.Util.DocIdBitSet;
using OpenBitSetDISI = Lucene.Net.Util.OpenBitSetDISI;
using Lucene.Net.Util;

namespace Lucene.Net.Search
{
	
	/// <summary> Wraps another filter's result and caches it.  The purpose is to allow
	/// filters to simply filter, and then wrap with this class to add caching.
	/// </summary>
	[Serializable]
	public class CachingWrapperFilter:Filter
	{
		protected internal Filter filter;
		
	    /**
       * Expert: Specifies how new deletions against a reopened
       * reader should be handled.
       *
       * <p>The default is IGNORE, which means the cache entry
       * will be re-used for a given segment, even when that
       * segment has been reopened due to changes in deletions.
       * This is a big performance gain, especially with
       * near-real-timer readers, since you don't hit a cache
       * miss on every reopened reader for prior segments.</p>
       *
       * <p>However, in some cases this can cause invalid query
       * results, allowing deleted documents to be returned.
       * This only happens if the main query does not rule out
       * deleted documents on its own, such as a toplevel
       * ConstantScoreQuery.  To fix this, use RECACHE to
       * re-create the cached filter (at a higher per-reopen
       * cost, but at faster subsequent search performance), or
       * use DYNAMIC to dynamically intersect deleted docs (fast
       * reopen time but some hit to search performance).</p>
       */
        [Serializable]
        public class DeletesMode : Parameter
        {
            private DeletesMode(String name) : base(name)
            {
            }
            public static DeletesMode IGNORE = new DeletesMode("IGNORE");
            public static DeletesMode RECACHE = new DeletesMode("RECACHE");
            public static DeletesMode DYNAMIC = new DeletesMode("DYNAMIC");
        }

		internal FilterCache<DocIdSet> cache;

        [Serializable]
        internal class FilterCache<T>
        {
            /**
             * A transient Filter cache (package private because of test)
             */
            // NOTE: not final so that we can dynamically re-init
            // after de-serialize
            volatile IDictionary<object,T> cache;

            private DeletesMode deletesMode;

            public FilterCache(DeletesMode deletesMode, Func<IndexReader, object, object> mergeDeletes)
            {
                this.deletesMode = deletesMode;
                this.MergeDeletes = mergeDeletes;
            }

            public T Get(IndexReader reader, object coreKey, object delCoreKey)
            {
                lock (this)
                {
                    T value;

                    if (cache == null)
                    {
                        cache = new Support.WeakDictionary<object,T>();
                    }

                    if (deletesMode == DeletesMode.IGNORE)
                    {
                        // key on core
                        value = cache[coreKey];
                    }
                    else if (deletesMode == DeletesMode.RECACHE)
                    {
                        // key on deletes, if any, else core
                        value = cache[delCoreKey];
                    }
                    else
                    {

                        System.Diagnostics.Debug.Assert(deletesMode == DeletesMode.DYNAMIC);

                        // first try for exact match
                        value = cache[delCoreKey];

                        if (value == null)
                        {
                            // now for core match, but dynamically AND NOT
                            // deletions
                            value = cache[coreKey];
                            if (value != null && reader.HasDeletions())
                            {
                                value = (T)MergeDeletes(reader, value);
                            }
                        }
                    }
                    return value;
                }

            }
       
            //protected abstract object MergeDeletes(IndexReader reader, object value);
            Func<IndexReader, object, object> MergeDeletes;

            public void Put(object coreKey, object delCoreKey, T value)
            {
                if (deletesMode == DeletesMode.IGNORE)
                {
                    cache[coreKey]= value;
                }
                else if (deletesMode == DeletesMode.RECACHE)
                {
                    cache[delCoreKey]=value;
                }
                else
                {
                    cache[coreKey]= value;
                    cache[delCoreKey]= value;
                }
            }
        }

        /**
          * New deletes are ignored by default, which gives higher
          * cache hit rate on reopened readers.  Most of the time
          * this is safe, because the filter will be AND'd with a
          * Query that fully enforces deletions.  If instead you
          * need this filter to always enforce deletions, pass
          * either {@link DeletesMode#RECACHE} or {@link
          * DeletesMode#DYNAMIC}.
          * @param filter Filter to cache results of
          */
        public CachingWrapperFilter(Filter filter) : this(filter, DeletesMode.IGNORE)
		{
		}

         /**
   * Expert: by default, the cached filter will be shared
   * across reopened segments that only had changes to their
   * deletions.  
   *
   * @param filter Filter to cache results of
   * @param deletesMode See {@link DeletesMode}
   */
        public CachingWrapperFilter(Filter filter, DeletesMode deletesMode)
        {
            this.filter = filter;
            cache = new FilterCache<DocIdSet>(deletesMode,
                (reader,docIdSet)=>{
                return new FilteredDocIdSet((DocIdSet)docIdSet, 
                    (docid) =>
                    {
                        return !reader.IsDeleted(docid);
                    });
            });
            
            //cache = new FilterCache(deletesMode) 
            // {
            //  public Object mergeDeletes(final IndexReader r, final Object docIdSet) {
            //    return new FilteredDocIdSet((DocIdSet) docIdSet) {
            //      protected boolean match(int docID) {
            //        return !r.isDeleted(docID);
            //      }
            //    };
            //  }
            //};
        }
		
		/// <summary>Provide the DocIdSet to be cached, using the DocIdSet provided
		/// by the wrapped Filter.
		/// This implementation returns the given DocIdSet.
		/// </summary>
		protected internal virtual DocIdSet DocIdSetToCache(DocIdSet docIdSet, IndexReader reader)
		{
            if (docIdSet == null)
            {
                // this is better than returning null, as the nonnull result can be cached
                return DocIdSet.EMPTY_DOCIDSET;
            }
            else if (docIdSet.IsCacheable()) {
				return docIdSet;
			}
			else
			{
				DocIdSetIterator it = docIdSet.Iterator();
				// null is allowed to be returned by iterator(),
				// in this case we wrap with the empty set,
				// which is cacheable.
				return (it == null) ? DocIdSet.EMPTY_DOCIDSET : new OpenBitSetDISI(it, reader.MaxDoc());
			}
		}

        // for testing
        public int hitCount, missCount;
		
		public override DocIdSet GetDocIdSet(IndexReader reader)
		{
			object coreKey = reader.GetFieldCacheKey();
            object delCoreKey = reader.HasDeletions() ? reader.GetDeletesCacheKey() : coreKey;

            DocIdSet docIdSet = cache.Get(reader, coreKey, delCoreKey);
            if (docIdSet != null)
            {
                hitCount++;
                return docIdSet;
            }

            missCount++;

            // cache miss
            docIdSet = DocIdSetToCache(filter.GetDocIdSet(reader), reader);

            if (docIdSet != null)
            {
                cache.Put(coreKey, delCoreKey, docIdSet);
            }

            return docIdSet;
		}
		
		public override System.String ToString()
		{
			return "CachingWrapperFilter(" + filter + ")";
		}
		
		public  override bool Equals(System.Object o)
		{
			if (!(o is CachingWrapperFilter))
				return false;
			return this.filter.Equals(((CachingWrapperFilter) o).filter);
		}
		
		public override int GetHashCode()
		{
			return filter.GetHashCode() ^ 0x1117BF25;
		}
	}
}