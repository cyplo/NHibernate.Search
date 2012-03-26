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

using Lucene.Net.Util;

namespace Lucene.Net.Search
{
	
	/// <summary> Expert: Collects sorted results from Searchable's and collates them.
	/// The elements put into this queue must be of type FieldDoc.
	/// 
	/// <p/>Created: Feb 11, 2004 2:04:21 PM
	/// 
	/// </summary>
	/// <since>   lucene 1.4
	/// </since>
	/// <version>  $Id: FieldDocSortedHitQueue.java 695514 2008-09-15 15:42:11Z otis $
	/// </version>
	class FieldDocSortedHitQueue:PriorityQueue<ScoreDoc>
	{
		
		// this cannot contain AUTO fields - any AUTO fields should
		// have been resolved by the time this class is used.
		internal volatile SortField[] fields;
		
		// used in the case where the fields are sorted by locale
		// based strings
		internal volatile System.Globalization.CompareInfo[] collators;
		
		
		/// <summary> Creates a hit queue sorted by the given list of fields.</summary>
		/// <param name="fields">Fieldable names, in priority order (highest priority first).
		/// </param>
		/// <param name="size"> The number of hits to retain.  Must be greater than zero.
		/// </param>
		internal FieldDocSortedHitQueue(SortField[] fields, int size)
		{
			this.fields = fields;
			this.collators = HasCollators(fields);
			Initialize(size);
		}
		
		
		/// <summary> Allows redefinition of sort fields if they are <code>null</code>.
		/// This is to handle the case using ParallelMultiSearcher where the
		/// original list contains AUTO and we don't know the actual sort
		/// type until the values come back.  The fields can only be set once.
		/// This method is thread safe.
		/// </summary>
		/// <param name="fields">
		/// </param>
		internal virtual void  SetFields(SortField[] fields)
		{
			lock (this)
			{
				if (this.fields == null)
				{
					this.fields = fields;
					this.collators = HasCollators(fields);
				}
			}
		}
		
		
		/// <summary>Returns the fields being used to sort. </summary>
		internal virtual SortField[] GetFields()
		{
			return fields;
		}
		
		
		/// <summary>Returns an array of collators, possibly <code>null</code>.  The collators
		/// correspond to any SortFields which were given a specific locale.
		/// </summary>
		/// <param name="fields">Array of sort fields.
		/// </param>
		/// <returns> Array, possibly <code>null</code>.
		/// </returns>
		private System.Globalization.CompareInfo[] HasCollators(SortField[] fields)
		{
			if (fields == null)
				return null;
			System.Globalization.CompareInfo[] ret = new System.Globalization.CompareInfo[fields.Length];
			for (int i = 0; i < fields.Length; ++i)
			{
				System.Globalization.CultureInfo locale = fields[i].GetLocale();
				if (locale != null)
					ret[i] = locale.CompareInfo;
			}
			return ret;
		}
		
		
		/// <summary> Returns whether <code>a</code> is less relevant than <code>b</code>.</summary>
		/// <param name="a">ScoreDoc
		/// </param>
		/// <param name="b">ScoreDoc
		/// </param>
		/// <returns> <code>true</code> if document <code>a</code> should be sorted after document <code>b</code>.
		/// </returns>
        public override bool LessThan(ScoreDoc docA1, ScoreDoc docB1)
		{

            FieldDoc docA = (FieldDoc)docA1;
            FieldDoc docB = (FieldDoc)docB1;
			int n = fields.Length;
			int c = 0;
			for (int i = 0; i < n && c == 0; ++i)
			{
				int type = fields[i].GetType();
				switch (type)
				{
					
					case SortField.SCORE:  {
							float r1 = (float) ((System.Single) docA.fields[i]);
							float r2 = (float) ((System.Single) docB.fields[i]);
							if (r1 > r2)
								c = - 1;
							if (r1 < r2)
								c = 1;
							break;
						}
					
					case SortField.DOC: 
					case SortField.INT:  {
							int i1 = ((System.Int32) docA.fields[i]);
							int i2 = ((System.Int32) docB.fields[i]);
							if (i1 < i2)
								c = - 1;
							if (i1 > i2)
								c = 1;
							break;
						}
					
					case SortField.LONG:  {
							long l1 = (long) ((System.Int64) docA.fields[i]);
							long l2 = (long) ((System.Int64) docB.fields[i]);
							if (l1 < l2)
								c = - 1;
							if (l1 > l2)
								c = 1;
							break;
						}
					
					case SortField.STRING:  {
							System.String s1 = (System.String) docA.fields[i];
							System.String s2 = (System.String) docB.fields[i];
							// null values need to be sorted first, because of how FieldCache.getStringIndex()
							// works - in that routine, any documents without a value in the given field are
							// put first.  If both are null, the next SortField is used
							if (s1 == null)
								c = (s2 == null)?0:- 1;
							else if (s2 == null)
								c = 1;
							// 
							else if (fields[i].GetLocale() == null)
							{
								c = String.CompareOrdinal(s1, s2);
							}
							else
							{
								c = collators[i].Compare(s1.ToString(), s2.ToString());
							}
							break;
						}
					
					case SortField.FLOAT:  {
							float f1 = (float) ((System.Single) docA.fields[i]);
							float f2 = (float) ((System.Single) docB.fields[i]);
							if (f1 < f2)
								c = - 1;
							if (f1 > f2)
								c = 1;
							break;
						}
					
					case SortField.DOUBLE:  {
							double d1 = ((System.Double) docA.fields[i]);
							double d2 = ((System.Double) docB.fields[i]);
							if (d1 < d2)
								c = - 1;
							if (d1 > d2)
								c = 1;
							break;
						}
					
					case SortField.BYTE:  {
							int i1 = (sbyte) ((System.SByte) docA.fields[i]);
							int i2 = (sbyte) ((System.SByte) docB.fields[i]);
							if (i1 < i2)
								c = - 1;
							if (i1 > i2)
								c = 1;
							break;
						}
					
					case SortField.SHORT:  {
							int i1 = (short) ((System.Int16) docA.fields[i]);
							int i2 = (short) ((System.Int16) docB.fields[i]);
							if (i1 < i2)
								c = - 1;
							if (i1 > i2)
								c = 1;
							break;
						}
					
					case SortField.CUSTOM:  {
							c = docA.fields[i].CompareTo(docB.fields[i]);
							break;
						}
					
					case SortField.AUTO:  {
							// we cannot handle this - even if we determine the type of object (Float or
							// Integer), we don't necessarily know how to compare them (both SCORE and
							// FLOAT contain floats, but are sorted opposite of each other). Before
							// we get here, each AUTO should have been replaced with its actual value.
							throw new System.SystemException("FieldDocSortedHitQueue cannot use an AUTO SortField");
						}
					
					default:  {
							throw new System.SystemException("invalid SortField type: " + type);
						}
					
				}
				if (fields[i].GetReverse())
				{
					c = - c;
				}
			}
			
			// avoid random sort order that could lead to duplicates (bug #31241):
			if (c == 0)
				return docA.Doc > docB.Doc;
			
			return c > 0;
		}
	}
}