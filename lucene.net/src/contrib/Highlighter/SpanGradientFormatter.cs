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

namespace Lucene.Net.Highlight
{
	/// <summary> Formats text with different color intensity depending on the score of the
	/// term using the span tag.  GradientFormatter uses a bgcolor argument to the font tag which
	/// doesn't work in Mozilla, thus this class.
	/// 
	/// </summary>
	/// <seealso cref="GradientFormatter">
	/// </seealso>
	/// <author>  David Spencer dave@searchmorph.com
	/// </author>
	
	public class SpanGradientFormatter:GradientFormatter
	{
		public SpanGradientFormatter(float maxScore, System.String minForegroundColor, System.String maxForegroundColor, System.String minBackgroundColor, System.String maxBackgroundColor):base(maxScore, minForegroundColor, maxForegroundColor, minBackgroundColor, maxBackgroundColor)
		{
		}
		
		
		
		public override System.String HighlightTerm(System.String originalText, TokenGroup tokenGroup)
		{
			if (tokenGroup.GetTotalScore() == 0)
				return originalText;
			float score = tokenGroup.GetTotalScore();
			if (score == 0)
			{
				return originalText;
			}
			
			// try to size sb correctly
			System.Text.StringBuilder sb = new System.Text.StringBuilder(originalText.Length + EXTRA);
			
			sb.Append("<span style=\"");
			if (highlightForeground)
			{
				sb.Append("color: ");
				sb.Append(GetForegroundColorString(score));
				sb.Append("; ");
			}
			if (highlightBackground)
			{
				sb.Append("background: ");
				sb.Append(GetBackgroundColorString(score));
				sb.Append("; ");
			}
			sb.Append("\">");
			sb.Append(originalText);
			sb.Append("</span>");
			return sb.ToString();
		}
		
		// guess how much extra text we'll add to the text we're highlighting to try to avoid a  StringBuffer resize
		private const System.String TEMPLATE = "<span style=\"background: #EEEEEE; color: #000000;\">...</span>";
		private static readonly int EXTRA = TEMPLATE.Length;
	}
}