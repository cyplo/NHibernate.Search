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

namespace Lucene.Net.Index
{
	
	/// <summary> For each Field, store position by position information.  It ignores frequency information
	/// <p/>
	/// This is not thread-safe.
	/// </summary>
	public class PositionBasedTermVectorMapper:TermVectorMapper
	{
		private Support.Dictionary<String, Support.Dictionary<int,TVPositionInfo>> fieldToTerms;
		
		private System.String currentField;
		/// <summary> A Map of Integer and TVPositionInfo</summary>
		private Support.Dictionary<int,TVPositionInfo> currentPositions;
		private bool storeOffsets;
		
		
		
		
		/// <summary> 
		/// 
		/// </summary>
		public PositionBasedTermVectorMapper():base(false, false)
		{
		}
		
		public PositionBasedTermVectorMapper(bool ignoringOffsets):base(false, ignoringOffsets)
		{
		}
		
		/// <summary> Never ignores positions.  This mapper doesn't make much sense unless there are positions</summary>
		/// <returns> false
		/// </returns>
		public override bool IsIgnoringPositions()
		{
			return false;
		}
		
		/// <summary> Callback for the TermVectorReader. </summary>
		/// <param name="term">
		/// </param>
		/// <param name="frequency">
		/// </param>
		/// <param name="offsets">
		/// </param>
		/// <param name="positions">
		/// </param>
		public override void  Map(System.String term, int frequency, TermVectorOffsetInfo[] offsets, int[] positions)
		{
			for (int i = 0; i < positions.Length; i++)
			{
				System.Int32 posVal = (System.Int32) positions[i];
				TVPositionInfo pos = (TVPositionInfo) currentPositions[posVal];
				if (pos == null)
				{
					pos = new TVPositionInfo(positions[i], storeOffsets);
					currentPositions[posVal] = pos;
				}
				pos.addTerm(term, offsets != null?offsets[i]:null);
			}
		}
		
		/// <summary> Callback mechanism used by the TermVectorReader</summary>
		/// <param name="field"> The field being read
		/// </param>
		/// <param name="numTerms">The number of terms in the vector
		/// </param>
		/// <param name="storeOffsets">Whether offsets are available
		/// </param>
		/// <param name="storePositions">Whether positions are available
		/// </param>
		public override void  SetExpectations(System.String field, int numTerms, bool storeOffsets, bool storePositions)
		{
			if (storePositions == false)
			{
				throw new System.SystemException("You must store positions in order to use this Mapper");
			}
			if (storeOffsets == true)
			{
				//ignoring offsets
			}
            fieldToTerms = new Support.Dictionary<string, Support.Dictionary<int, TVPositionInfo>>(numTerms);
			this.storeOffsets = storeOffsets;
			currentField = field;
            currentPositions = new Support.Dictionary<int, TVPositionInfo>();
			fieldToTerms[currentField] = currentPositions;
		}
		
		/// <summary> Get the mapping between fields and terms, sorted by the comparator
		/// 
		/// </summary>
		/// <returns> A map between field names and a Map.  The sub-Map key is the position as the integer, the value is {@link Lucene.Net.Index.PositionBasedTermVectorMapper.TVPositionInfo}.
		/// </returns>
        public virtual Support.Dictionary<String, Support.Dictionary<int, TVPositionInfo>> GetFieldToTerms()
		{
			return fieldToTerms;
		}
		
		/// <summary> Container for a term at a position</summary>
		public class TVPositionInfo
		{
			/// <summary> </summary>
			/// <returns> The position of the term
			/// </returns>
			virtual public int Position
			{
				get
				{
					return position;
				}
				
			}
			/// <summary> Note, there may be multiple terms at the same position</summary>
			/// <returns> A List of Strings
			/// </returns>
			virtual public IList<string> Terms
			{
				get
				{
					return terms;
				}
				
			}
			/// <summary> Parallel list (to {@link #getTerms()}) of TermVectorOffsetInfo objects.  There may be multiple entries since there may be multiple terms at a position</summary>
			/// <returns> A List of TermVectorOffsetInfo objects, if offsets are store.
			/// </returns>
			virtual public IList<TermVectorOffsetInfo> Offsets
			{
				get
				{
					return offsets;
				}
				
			}
			private int position;
			//a list of Strings
            private IList<string> terms;
			//A list of TermVectorOffsetInfo
            private IList<TermVectorOffsetInfo> offsets;
			
			
			public TVPositionInfo(int position, bool storeOffsets)
			{
				this.position = position;
				terms = new List<string>();
				if (storeOffsets)
				{
					offsets = new List<TermVectorOffsetInfo>();
				}
			}
			
			internal virtual void  addTerm(System.String term, TermVectorOffsetInfo info)
			{
				terms.Add(term);
				if (offsets != null)
				{
					offsets.Add(info);
				}
			}
		}
	}
}