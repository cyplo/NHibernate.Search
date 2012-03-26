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
using ToStringUtils = Lucene.Net.Util.ToStringUtils;

namespace Lucene.Net.Search.Spans
{
	
	/// <summary>Matches spans containing a term. </summary>
	[Serializable]
	public class SpanTermQuery:SpanQuery
	{
		protected internal Term term;
		
		/// <summary>Construct a SpanTermQuery matching the named term's spans. </summary>
		public SpanTermQuery(Term term)
		{
			this.term = term;
		}
		
		/// <summary>Return the term whose spans are matched. </summary>
		public virtual Term GetTerm()
		{
			return term;
		}
		
		public override System.String GetField()
		{
			return term.Field();
		}
		
		/// <summary>Returns a collection of all terms matched by this query.</summary>
		/// <deprecated> use extractTerms instead
		/// </deprecated>
		/// <seealso cref="ExtractTerms(Set)">
		/// </seealso>
        [Obsolete("use extractTerms instead")]
        public override IList<Lucene.Net.Index.Term> GetTerms()
		{
            List<Lucene.Net.Index.Term> terms = new List<Term>();
			terms.Add(term);
			return terms;
		}
        public override void ExtractTerms(ICollection<Lucene.Net.Index.Term> terms)
		{
            terms.Add(term);
		}
		
		public override System.String ToString(System.String field)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			if (term.Field().Equals(field))
				buffer.Append(term.Text());
			else
			{
				buffer.Append(term.ToString());
			}
			buffer.Append(ToStringUtils.Boost(GetBoost()));
			return buffer.ToString();
		}
		
		public override int GetHashCode()
		{
			int prime = 31;
			int result = base.GetHashCode();
			result = prime * result + ((term == null)?0:term.GetHashCode());
			return result;
		}
		
		public  override bool Equals(System.Object obj)
		{
			if (this == obj)
				return true;
			if (!base.Equals(obj))
				return false;
			if (GetType() != obj.GetType())
				return false;
			SpanTermQuery other = (SpanTermQuery) obj;
			if (term == null)
			{
				if (other.term != null)
					return false;
			}
			else if (!term.Equals(other.term))
				return false;
			return true;
		}
		
		public override Spans GetSpans(IndexReader reader)
		{
			return new TermSpans(reader.TermPositions(term), term);
		}
	}
}