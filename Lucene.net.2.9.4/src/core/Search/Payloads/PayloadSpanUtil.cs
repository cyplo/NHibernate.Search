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
using System.Collections.Generic;

using IndexReader = Lucene.Net.Index.IndexReader;
using Term = Lucene.Net.Index.Term;
using BooleanClause = Lucene.Net.Search.BooleanClause;
using BooleanQuery = Lucene.Net.Search.BooleanQuery;
using DisjunctionMaxQuery = Lucene.Net.Search.DisjunctionMaxQuery;
using FilteredQuery = Lucene.Net.Search.FilteredQuery;
using MultiPhraseQuery = Lucene.Net.Search.MultiPhraseQuery;
using PhraseQuery = Lucene.Net.Search.PhraseQuery;
using Query = Lucene.Net.Search.Query;
using TermQuery = Lucene.Net.Search.TermQuery;
using SpanNearQuery = Lucene.Net.Search.Spans.SpanNearQuery;
using SpanOrQuery = Lucene.Net.Search.Spans.SpanOrQuery;
using SpanQuery = Lucene.Net.Search.Spans.SpanQuery;
using SpanTermQuery = Lucene.Net.Search.Spans.SpanTermQuery;

namespace Lucene.Net.Search.Payloads
{
	
	/// <summary> Experimental class to get set of payloads for most standard Lucene queries.
	/// Operates like Highlighter - IndexReader should only contain doc of interest,
	/// best to use MemoryIndex.
	/// 
	/// <p/>
	/// <font color="#FF0000">
	/// WARNING: The status of the <b>Payloads</b> feature is experimental.
	/// The APIs introduced here might change in the future and will not be
	/// supported anymore in such a case.</font>
	/// 
	/// </summary>
	public class PayloadSpanUtil
	{
		private IndexReader reader;
		
		/// <param name="reader">that contains doc with payloads to extract
		/// </param>
		public PayloadSpanUtil(IndexReader reader)
		{
			this.reader = reader;
		}
		
		/// <summary> Query should be rewritten for wild/fuzzy support.
		/// 
		/// </summary>
		/// <param name="query">
		/// </param>
		/// <returns> payloads Collection
		/// </returns>
		/// <throws>  IOException </throws>
		public virtual ICollection<byte[]> GetPayloadsForQuery(Query query)
		{
			ICollection<byte[]> payloads = new List<byte[]>();
			QueryToSpanQuery(query, payloads);
			return payloads;
		}
		
		private void  QueryToSpanQuery(Query query, ICollection<byte[]> payloads)
		{
			if (query is BooleanQuery)
			{
				BooleanClause[] queryClauses = ((BooleanQuery) query).GetClauses();
				
				for (int i = 0; i < queryClauses.Length; i++)
				{
					if (!queryClauses[i].IsProhibited())
					{
						QueryToSpanQuery(queryClauses[i].GetQuery(), payloads);
					}
				}
			}
			else if (query is PhraseQuery)
			{
				Term[] phraseQueryTerms = ((PhraseQuery) query).GetTerms();
				SpanQuery[] clauses = new SpanQuery[phraseQueryTerms.Length];
				for (int i = 0; i < phraseQueryTerms.Length; i++)
				{
					clauses[i] = new SpanTermQuery(phraseQueryTerms[i]);
				}
				
				int slop = ((PhraseQuery) query).GetSlop();
				bool inorder = false;
				
				if (slop == 0)
				{
					inorder = true;
				}
				
				SpanNearQuery sp = new SpanNearQuery(clauses, slop, inorder);
				sp.SetBoost(query.GetBoost());
				GetPayloads(payloads, sp);
			}
			else if (query is TermQuery)
			{
				SpanTermQuery stq = new SpanTermQuery(((TermQuery) query).GetTerm());
				stq.SetBoost(query.GetBoost());
				GetPayloads(payloads, stq);
			}
			else if (query is SpanQuery)
			{
				GetPayloads(payloads, (SpanQuery) query);
			}
			else if (query is FilteredQuery)
			{
				QueryToSpanQuery(((FilteredQuery) query).GetQuery(), payloads);
			}
			else if (query is DisjunctionMaxQuery)
			{
                for (IEnumerator<Query> iterator = ((DisjunctionMaxQuery) query).Iterator(); iterator.MoveNext(); )
                {
                    QueryToSpanQuery(iterator.Current, payloads);
                }
			}
			else if (query is MultiPhraseQuery)
			{
				MultiPhraseQuery mpq = (MultiPhraseQuery) query;
				List<Term[]> termArrays = mpq.GetTermArrays();
				int[] positions = mpq.GetPositions();
				if (positions.Length > 0)
				{
					
					int maxPosition = positions[positions.Length - 1];
					for (int i = 0; i < positions.Length - 1; ++i)
					{
						if (positions[i] > maxPosition)
						{
							maxPosition = positions[i];
						}
					}

                    List<Query>[] disjunctLists = new List<Query>[maxPosition + 1];
					int distinctPositions = 0;
					
					for (int i = 0; i < termArrays.Count; ++i)
					{
						Term[] termArray = termArrays[i];
						System.Collections.IList disjuncts = disjunctLists[positions[i]];
						if (disjuncts == null)
						{
							disjuncts = (disjunctLists[positions[i]] = new List<Query>(termArray.Length));
							++distinctPositions;
						}
						for (int j = 0; j < termArray.Length; ++j)
						{
							disjuncts.Add(new SpanTermQuery(termArray[j]));
						}
					}
					
					int positionGaps = 0;
					int position = 0;
					SpanQuery[] clauses = new SpanQuery[distinctPositions];
					for (int i = 0; i < disjunctLists.Length; ++i)
					{
                        List<Query> disjuncts = disjunctLists[i];
						if (disjuncts != null)
						{
                            clauses[position++] = new SpanOrQuery((SpanQuery[]) (disjuncts.ToArray()));
						}
						else
						{
							++positionGaps;
						}
					}
					
					int slop = mpq.GetSlop();
					bool inorder = (slop == 0);
					
					SpanNearQuery sp = new SpanNearQuery(clauses, slop + positionGaps, inorder);
					sp.SetBoost(query.GetBoost());
					GetPayloads(payloads, sp);
				}
			}
		}
		
		private void  GetPayloads(ICollection<byte[]> payloads, SpanQuery query)
		{
			Lucene.Net.Search.Spans.Spans spans = query.GetSpans(reader);
			
			while (spans.Next() == true)
			{
				if (spans.IsPayloadAvailable())
				{
					//ICollection<byte[]> payload = spans.GetPayload();
                    ICollection<byte[]> payload = spans.GetPayload();
					//IEnumerator<byte[]> it = payload.GetEnumerator();
                    foreach (byte[] bytes in payload)
                    {
                        payloads.Add(bytes);
                    }
				}
			}
		}
	}
}