﻿/*
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

using Analyzer = Lucene.Net.Analysis.Analyzer;
using TokenStream = Lucene.Net.Analysis.TokenStream;
using Term = Lucene.Net.Index.Term;
using BooleanQuery = Lucene.Net.Search.BooleanQuery;
using IndexSearcher = Lucene.Net.Search.IndexSearcher;
using Query = Lucene.Net.Search.Query;
using TermQuery = Lucene.Net.Search.TermQuery;
using BooleanClause = Lucene.Net.Search.BooleanClause;
using Lucene.Net.Analysis.Tokenattributes;

namespace Similarity.Net
{

    /// <summary> Simple similarity measures.
    /// 
    /// 
    /// </summary>
    /// <seealso cref="MoreLikeThis">
    /// </seealso>
    public sealed class SimilarityQueries
    {
        /// <summary> </summary>
        private SimilarityQueries()
        {
        }

        /// <summary> Simple similarity query generators.
        /// Takes every unique word and forms a boolean query where all words are optional.
        /// After you get this you'll use to to query your {@link IndexSearcher} for similar docs.
        /// The only caveat is the first hit returned <b>should be</b> your source document - you'll
        /// need to then ignore that.
        /// 
        /// <p>
        /// 
        /// So, if you have a code fragment like this:
        /// <br>
        /// <code>
        /// Query q = formSimilaryQuery( "I use Lucene to search fast. Fast searchers are good", new StandardAnalyzer(), "contents", null);
        /// </code>
        /// 
        /// <p>
        /// 
        /// </summary>
        /// <summary> The query returned, in string form, will be <code>'(i use lucene to search fast searchers are good')</code>.
        /// 
        /// <p>
        /// The philosophy behind this method is "two documents are similar if they share lots of words".
        /// Note that behind the scenes, Lucenes scoring algorithm will tend to give two documents a higher similarity score if the share more uncommon words.
        /// 
        /// <P>
        /// This method is fail-safe in that if a long 'body' is passed in and
        /// {@link BooleanQuery#add BooleanQuery.add()} (used internally)
        /// throws
        /// {@link org.apache.lucene.search.BooleanQuery.TooManyClauses BooleanQuery.TooManyClauses}, the
        /// query as it is will be returned.
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="body">the body of the document you want to find similar documents to
        /// </param>
        /// <param name="a">the analyzer to use to parse the body
        /// </param>
        /// <param name="field">the field you want to search on, probably something like "contents" or "body"
        /// </param>
        /// <param name="stop">optional set of stop words to ignore
        /// </param>
        /// <returns> a query with all unique words in 'body'
        /// </returns>
        /// <throws>  IOException this can't happen... </throws>
        public static Query FormSimilarQuery(System.String body, Analyzer a, System.String field, System.Collections.Hashtable stop)
        {
            TokenStream ts = a.TokenStream(field, new System.IO.StringReader(body));
            TermAttribute termAtt = (TermAttribute)ts.AddAttribute(typeof(TermAttribute));

            BooleanQuery tmp = new BooleanQuery();
            System.Collections.Hashtable already = new System.Collections.Hashtable(); // ignore dups
            while (ts.IncrementToken())
            {
                String word = termAtt.Term();
                // ignore opt stop words
                if (stop != null && stop.Contains(word))
                    continue;
                // ignore dups
                if (already.Contains(word) == true)
                    continue;
                already.Add(word, word);
                // add to query
                TermQuery tq = new TermQuery(new Term(field, word));
                try
                {
                    tmp.Add(tq, BooleanClause.Occur.SHOULD);
                }
                catch (BooleanQuery.TooManyClauses)
                {
                    // fail-safe, just return what we have, not the end of the world
                    break;
                }
            }
            return tmp;
        }
    }
}