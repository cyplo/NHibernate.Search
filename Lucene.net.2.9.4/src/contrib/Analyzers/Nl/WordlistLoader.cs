/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Lucene.Net.Analysis.Nl
{

	/* ====================================================================
	 * The Apache Software License, Version 1.1
	 *
	 * Copyright (c) 2001 The Apache Software Foundation.  All rights
	 * reserved.
	 *
	 * Redistribution and use in source and binary forms, with or without
	 * modification, are permitted provided that the following conditions
	 * are met:
	 *
	 * 1. Redistributions of source code must retain the above copyright
	 *    notice, this list of conditions and the following disclaimer.
	 *
	 * 2. Redistributions in binary form must reproduce the above copyright
	 *    notice, this list of conditions and the following disclaimer in
	 *    the documentation and/or other materials provided with the
	 *    distribution.
	 *
	 * 3. The end-user documentation included with the redistribution,
	 *    if any, must include the following acknowledgment:
	 *       "This product includes software developed by the
	 *        Apache Software Foundation (http://www.apache.org/)."
	 *    Alternately, this acknowledgment may appear in the software itself,
	 *    if and wherever such third-party acknowledgments normally appear.
	 *
	 * 4. The names "Apache" and "Apache Software Foundation" and
	 *    "Apache Lucene" must not be used to endorse or promote products
	 *    derived from this software without prior written permission. For
	 *    written permission, please contact apache@apache.org.
	 *
	 * 5. Products derived from this software may not be called "Apache",
	 *    "Apache Lucene", nor may "Apache" appear in their name, without
	 *    prior written permission of the Apache Software Foundation.
	 *
	 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
	 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
	 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	 * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
	 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
	 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
	 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
	 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
	 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
	 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	 * SUCH DAMAGE.
	 * ====================================================================
	 *
	 * This software consists of voluntary contributions made by many
	 * individuals on behalf of the Apache Software Foundation.  For more
	 * information on the Apache Software Foundation, please see
	 * <http://www.apache.org/>.
	 */

	/// <summary>
	/// Loads a text file and adds every line as an entry to a Hashtable. Every line
	/// should contain only one word. If the file is not found or on any error, an
	/// empty table is returned.
	/// 
	/// <version>$Id: WordListLoader.java,v 1.1 2004/03/09 14:55:08 otis Exp $</version>
	/// </summary>
	/// <author>Gerhard Schwarz</author>
	public class WordlistLoader
	{
		/// <param name="path">Path to the wordlist</param>
		/// <param name="wordfile">Name of the wordlist</param>
		/// <returns></returns>
        public static ICollection<string> GetWordtable(String path, String wordfile) 
		{
			if ( path == null || wordfile == null ) 
			{
				return new List<string>();
			}
			return GetWordtable(new FileInfo(path + "\\" + wordfile));
		}

		/// <param name="wordfile">Complete path to the wordlist</param>
        public static ICollection<string> GetWordtable(String wordfile) 
		{
			if ( wordfile == null ) 
			{
				return new List<string>();
			}
			return GetWordtable( new FileInfo( wordfile ) );
		}

		/// <summary>
		/// Reads a stemsdictionary. Each line contains: 
		/// word \t stem 
		/// (i.e. tab seperated)
		/// </summary>
		/// <param name="wordstemfile"></param>
		/// <returns>Stem dictionary that overrules, the stemming algorithm</returns>
        public static Dictionary<string,string> GetStemDict(FileInfo wordstemfile)
		{
			if ( wordstemfile == null ) 
			{
				return new Dictionary<string,string>();
			}
			Dictionary<string,string> result = new Dictionary<string,string>();
			try 
			{
				StreamReader lnr = new StreamReader(wordstemfile.FullName);
				string line;
				string[] wordstem;
				while ((line = lnr.ReadLine()) != null)
				{
					wordstem = line.Split(new char[]{'\t'},2);
					result.Add(wordstem[0], wordstem[1]);
			   }
			}
			catch (IOException) 
			{
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="wordfile">File containing the wordlist</param>
		/// <returns></returns>
        public static ICollection<string> GetWordtable(FileInfo wordfile) 
		{
			if ( wordfile == null ) 
			{
				return new List<string>();
			}
            ICollection<string> result = null;
			try 
			{
				StreamReader lnr = new StreamReader(wordfile.FullName);
				String word = null;
				String[] stopwords = new String[100];
				int wordcount = 0;
				while ( ( word = lnr.ReadLine() ) != null ) 
				{
					wordcount++;
					if ( wordcount == stopwords.Length ) 
					{
						String[] tmp = new String[stopwords.Length + 50];
						Array.Copy( stopwords, 0, tmp, 0, wordcount );
						stopwords = tmp;
					}
					stopwords[wordcount-1] = word;
				}
				result = MakeWordTable( stopwords, wordcount );
			}
				// On error, use an empty table
			catch (IOException) 
			{
				result = new List<string>();
			}
			return result;
		}

		/// <summary>
		/// Builds the wordlist table.
		/// </summary>
		/// <param name="words">Word that where read</param>
		/// <param name="length">Amount of words that where read into <tt>words</tt></param>
		/// <returns></returns>
        private static ICollection<string> MakeWordTable(String[] words, int length) 
		{
            List<string> table = new List<string>(length);
			for ( int i = 0; i < length; i++ ) 
			{
				table.Add(words[i]);
			}
			return table;
		}
	}
}
