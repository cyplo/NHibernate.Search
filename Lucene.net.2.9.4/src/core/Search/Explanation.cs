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

namespace Lucene.Net.Search
{
	
	/// <summary>Expert: Describes the score computation for document and query. </summary>
	[Serializable]
	public class Explanation
	{
		private float value_Renamed; // the value of this node
		private System.String description; // what it represents
		private List<Explanation> details; // sub-explanations
		
		///<summary>
		/// Default Constructor
		///</summary>
		public Explanation()
		{
		}
		
		///<summary>
		/// Class Constructor
		///</summary>
        ///<param name="value_Renamed">The value assigned to this explanation node.</param>
        ///<param name="description">The description of this explanation node.</param>
		public Explanation(float value_Renamed, System.String description)
		{
			this.value_Renamed = value_Renamed;
			this.description = description;
		}
		
		/// <summary> Indicates whether or not this Explanation models a good match.
		/// 
		/// <p/>
		/// By default, an Explanation represents a "match" if the value is positive.
		/// <p/>
		/// </summary>
		/// <seealso cref="GetValue">
		/// </seealso>
		public virtual bool IsMatch()
		{
			return (0.0f < GetValue());
		}
		
		
		
		/// <summary>The value assigned to this explanation node. </summary>
		public virtual float GetValue()
		{
			return value_Renamed;
		}
		/// <summary>Sets the value assigned to this explanation node. </summary>
		public virtual void  SetValue(float value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}
		
		/// <summary>A description of this explanation node. </summary>
		public virtual System.String GetDescription()
		{
			return description;
		}
		/// <summary>Sets the description of this explanation node. </summary>
		public virtual void  SetDescription(System.String description)
		{
			this.description = description;
		}
		
		/// <summary> A short one line summary which should contain all high level
		/// information about this Explanation, without the "Details"
		/// </summary>
		protected internal virtual System.String GetSummary()
		{
			return GetValue() + " = " + GetDescription();
		}
		
		/// <summary>The sub-nodes of this explanation node. </summary>
		public virtual Explanation[] GetDetails()
		{
			if (details == null)
				return null;
			return details.ToArray();
		}
		
		/// <summary>Adds a sub-node to this explanation node. </summary>
		public virtual void  AddDetail(Explanation detail)
		{
            if (details == null)
                details = new List<Explanation>();
			details.Add(detail);
		}
		
		/// <summary>Render an explanation as text. </summary>
		public override System.String ToString()
		{
			return ToString(0);
		}
		///<summary>
        /// Render an explanation as text to a detail level. 
		///</summary>
		///<param name="depth"> Depth of detail</param>
		///<returns></returns>
		public /*protected internal*/ virtual System.String ToString(int depth)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			for (int i = 0; i < depth; i++)
			{
				buffer.Append("  ");
			}
			buffer.Append(GetSummary());
			buffer.Append("\n");
			
			Explanation[] details = GetDetails();
			if (details != null)
			{
				for (int i = 0; i < details.Length; i++)
				{
					buffer.Append(details[i].ToString(depth + 1));
				}
			}
			
			return buffer.ToString();
		}
		
		
		/// <summary>Render an explanation as HTML. </summary>
		public virtual System.String ToHtml()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			buffer.Append("<ul>\n");
			
			buffer.Append("<li>");
			buffer.Append(GetSummary());
			buffer.Append("<br />\n");
			
			Explanation[] details = GetDetails();
			if (details != null)
			{
				for (int i = 0; i < details.Length; i++)
				{
					buffer.Append(details[i].ToHtml());
				}
			}
			
			buffer.Append("</li>\n");
			buffer.Append("</ul>\n");
			
			return buffer.ToString();
		}
		
		/// <summary> Small Utility class used to pass both an idf factor as well as an
		/// explanation for that factor.
		/// 
		/// This class will likely be held on a <see cref="Weight"/>, so be aware 
		/// before storing any large or un-serializable fields.
		/// 
		/// </summary>
		/// 
		[Serializable]
		public class IDFExplanation
		{
			/// <returns> the idf factor
			/// </returns>
			[NonSerialized]
			public Func<float> GetIdf;
			/// <summary> This should be calculated lazily if possible.
			/// 
			/// </summary>
			/// <returns> the explanation for the idf factor.
			/// </returns>
			[NonSerialized]
			public Func<System.String> Explain;
		}
	}
}