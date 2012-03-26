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
using Lucene.Net.Util;
using ToStringUtils = Lucene.Net.Util.ToStringUtils;
using Query = Lucene.Net.Search.Query;

namespace Lucene.Net.Search.Spans
{
	
	/// <summary>Matches the union of its clauses.</summary>
	[Serializable]
	public class SpanOrQuery:SpanQuery, System.ICloneable
	{
		private class AnonymousClassSpans : Spans
		{
			public AnonymousClassSpans(Lucene.Net.Index.IndexReader reader, SpanOrQuery enclosingInstance)
			{
				InitBlock(reader, enclosingInstance);
			}
			private void  InitBlock(Lucene.Net.Index.IndexReader reader, SpanOrQuery enclosingInstance)
			{
				this.reader = reader;
				this.enclosingInstance = enclosingInstance;
			}
			private Lucene.Net.Index.IndexReader reader;
			private SpanOrQuery enclosingInstance;
			public SpanOrQuery Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			private SpanQueue queue = null;
			
			private bool InitSpanQueue(int target)
			{
				queue = new SpanQueue(enclosingInstance, Enclosing_Instance.clauses.Count);
                foreach (SpanQuery i in Enclosing_Instance.clauses)
                {
                    Spans spans = i.GetSpans(reader);
                    if (((target == -1) && spans.Next()) || ((target != -1) && spans.SkipTo(target)))
                    {
                        queue.Put(spans);
                    }
                }

				return queue.Size() != 0;
			}
			
			public override bool Next()
			{
				if (queue == null)
				{
					return InitSpanQueue(- 1);
				}
				
				if (queue.Size() == 0)
				{
					// all done
					return false;
				}
				
				if (Top().Next())
				{
					// move to next
					queue.AdjustTop();
					return true;
				}
				
				queue.Pop(); // exhausted a clause
				return queue.Size() != 0;
			}
			
			private Spans Top()
			{
				return (Spans) queue.Top();
			}
			
			public override bool SkipTo(int target)
			{
				if (queue == null)
				{
					return InitSpanQueue(target);
				}
				
				bool skipCalled = false;
				while (queue.Size() != 0 && Top().Doc() < target)
				{
					if (Top().SkipTo(target))
					{
						queue.AdjustTop();
					}
					else
					{
						queue.Pop();
					}
					skipCalled = true;
				}
				
				if (skipCalled)
				{
					return queue.Size() != 0;
				}
				return Next();
			}
			
			public override int Doc()
			{
				return Top().Doc();
			}
			public override int Start()
			{
				return Top().Start();
			}
			public override int End()
			{
				return Top().End();
			}
			
			// TODO: Remove warning after API has been finalized
			public override ICollection<byte[]> GetPayload()
			{
				ICollection<byte[]> result = null;
				Spans theTop = Top();
				if (theTop != null && theTop.IsPayloadAvailable())
				{
					result = theTop.GetPayload();
				}
				return result;
			}
			
			// TODO: Remove warning after API has been finalized
			public override bool IsPayloadAvailable()
			{
				Spans top = Top();
				return top != null && top.IsPayloadAvailable();
			}
			
			public override System.String ToString()
			{
				return "spans(" + Enclosing_Instance + ")@" + ((queue == null)?"START":(queue.Size() > 0?(Doc() + ":" + Start() + "-" + End()):"END"));
			}
		}
		private Support.EquatableList<SpanQuery> clauses;
		private System.String field;
		
		/// <summary>Construct a SpanOrQuery merging the provided clauses. </summary>
		public SpanOrQuery(params SpanQuery[] clauses)
		{
			
			// copy clauses array into an ArrayList
			this.clauses = new Support.EquatableList<SpanQuery>(clauses.Length);
			for (int i = 0; i < clauses.Length; i++)
			{
				SpanQuery clause = clauses[i];
				if (i == 0)
				{
					// check field
					field = clause.GetField();
				}
				else if (!clause.GetField().Equals(field))
				{
					throw new System.ArgumentException("Clauses must have same field.");
				}
				this.clauses.Add(clause);
			}
		}
		
		/// <summary>Return the clauses whose spans are matched. </summary>
		public virtual SpanQuery[] GetClauses()
		{
			return (SpanQuery[]) clauses.ToArray();
		}
		
		public override System.String GetField()
		{
			return field;
		}
		
		/// <summary>Returns a collection of all terms matched by this query.</summary>
		/// <deprecated> use extractTerms instead
		/// </deprecated>
		/// <seealso cref="ExtractTerms(Set)">
		/// </seealso>
        [Obsolete("use ExtractTerms instead")]
		public override IList<Lucene.Net.Index.Term> GetTerms()
		{
            List<Lucene.Net.Index.Term> terms = new List<Index.Term>();
            foreach (SpanQuery clause in clauses)
            {
                terms.AddRange(clause.GetTerms());
            }
			return terms;
		}

        public override void ExtractTerms(ICollection<Lucene.Net.Index.Term> terms)
		{
            foreach(SpanQuery clause in clauses)
			{
				clause.ExtractTerms(terms);
			}
		}
		
		public override System.Object Clone()
		{
			int sz = clauses.Count;
			SpanQuery[] newClauses = new SpanQuery[sz];
			
			for (int i = 0; i < sz; i++)
			{
				SpanQuery clause = (SpanQuery) clauses[i];
				newClauses[i] = (SpanQuery) clause.Clone();
			}
			SpanOrQuery soq = new SpanOrQuery(newClauses);
			soq.SetBoost(GetBoost());
			return soq;
		}
		
		public override Query Rewrite(IndexReader reader)
		{
			SpanOrQuery clone = null;
			for (int i = 0; i < clauses.Count; i++)
			{
				SpanQuery c = (SpanQuery) clauses[i];
				SpanQuery query = (SpanQuery) c.Rewrite(reader);
				if (query != c)
				{
					// clause rewrote: must clone
					if (clone == null)
						clone = (SpanOrQuery) this.Clone();
					clone.clauses[i] = query;
				}
			}
			if (clone != null)
			{
				return clone; // some clauses rewrote
			}
			else
			{
				return this; // no clauses rewrote
			}
		}
		
		public override System.String ToString(System.String field)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			buffer.Append("spanOr([");
            int j = 0;
            foreach (SpanQuery clause in clauses)
			{
                j++;
				buffer.Append(clause.ToString(field));
                if (j < clauses.Count)
                {
                    buffer.Append(", ");
                }
			}
			buffer.Append("])");
			buffer.Append(ToStringUtils.Boost(GetBoost()));
			return buffer.ToString();
		}
		
		public  override bool Equals(System.Object o)
		{
			if (this == o)
				return true;
			if (o == null || GetType() != o.GetType())
				return false;
			
			SpanOrQuery that = (SpanOrQuery) o;
			
			if (!clauses.Equals(that.clauses))
				return false;
			if (!(clauses.Count == 0) && !field.Equals(that.field))
				return false;
			
			return GetBoost() == that.GetBoost();
		}
		
		public override int GetHashCode()
		{
			int h = clauses.GetHashCode();
			h ^= ((h << 10) | (Support.Number.URShift(h, 23)));
			h ^= System.Convert.ToInt32(GetBoost());
			return h;
		}
		
		
		private class SpanQueue:PriorityQueue<Spans>
		{
			private void  InitBlock(SpanOrQuery enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private SpanOrQuery enclosingInstance;
			public SpanOrQuery Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public SpanQueue(SpanOrQuery enclosingInstance, int size)
			{
				InitBlock(enclosingInstance);
				Initialize(size);
			}

            public override bool LessThan(Spans spans1, Spans spans2)
			{
				if (spans1.Doc() == spans2.Doc())
				{
					if (spans1.Start() == spans2.Start())
					{
						return spans1.End() < spans2.End();
					}
					else
					{
						return spans1.Start() < spans2.Start();
					}
				}
				else
				{
					return spans1.Doc() < spans2.Doc();
				}
			}
		}
		
		public override Spans GetSpans(IndexReader reader)
		{
			if (clauses.Count == 1)
			// optimize 1-clause case
				return ((SpanQuery) clauses[0]).GetSpans(reader);
			
			return new AnonymousClassSpans(reader, this);
		}
	}
}