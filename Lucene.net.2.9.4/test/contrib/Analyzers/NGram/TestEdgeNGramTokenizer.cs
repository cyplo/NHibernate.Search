﻿/**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Collections;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Util;
using NUnit.Framework;

namespace Lucene.Net.Analysis.NGram
{

    /**
     * Tests {@link EdgeNGramTokenizer} for correctness.
     */
    [TestFixture]
    public class TestEdgeNGramTokenizer : BaseTokenStreamTestCase
    {
        private StringReader input;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            input = new StringReader("abcde");
        }

        [Test]
        public void TestInvalidInput()
        {
            bool gotException = false;
            try
            {
                new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 0, 0);
            }
            catch (System.ArgumentException e)
            {
                gotException = true;
            }
            Assert.IsTrue(gotException);
        }

        [Test]
        public void TestInvalidInput2()
        {
            bool gotException = false;
            try
            {
                new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 2, 1);
            }
            catch (System.ArgumentException e)
            {
                gotException = true;
            }
            Assert.IsTrue(gotException);
        }

        [Test]
        public void TestInvalidInput3()
        {
            bool gotException = false;
            try
            {
                new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, -1, 2);
            }
            catch (System.ArgumentException e)
            {
                gotException = true;
            }
            Assert.IsTrue(gotException);
        }

        [Test]
        public void TestFrontUnigram()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 1, 1);
            AssertTokenStreamContents(tokenizer, new String[] { "a" }, new int[] { 0 }, new int[] { 1 }, 5 /* abcde */);
        }

        [Test]
        public void TestBackUnigram()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.BACK, 1, 1);
            AssertTokenStreamContents(tokenizer, new String[] { "e" }, new int[] { 4 }, new int[] { 5 }, 5 /* abcde */);
        }

        [Test]
        public void TestOversizedNgrams()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 6, 6);
            AssertTokenStreamContents(tokenizer, new String[0], new int[0], new int[0], 5 /* abcde */);
        }

        [Test]
        public void TestFrontRangeOfNgrams()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 1, 3);
            AssertTokenStreamContents(tokenizer, new String[] { "a", "ab", "abc" }, new int[] { 0, 0, 0 }, new int[] { 1, 2, 3 }, 5 /* abcde */);
        }

        [Test]
        public void TestBackRangeOfNgrams()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.BACK, 1, 3);
            AssertTokenStreamContents(tokenizer, new String[] { "e", "de", "cde" }, new int[] { 4, 3, 2 }, new int[] { 5, 5, 5 }, 5 /* abcde */);
        }

        [Test]
        public void TestReset()
        {
            EdgeNGramTokenizer tokenizer = new EdgeNGramTokenizer(input, EdgeNGramTokenizer.Side.FRONT, 1, 3);
            AssertTokenStreamContents(tokenizer, new String[] { "a", "ab", "abc" }, new int[] { 0, 0, 0 }, new int[] { 1, 2, 3 }, 5 /* abcde */);
            tokenizer.Reset(new StringReader("abcde"));
            AssertTokenStreamContents(tokenizer, new String[] { "a", "ab", "abc" }, new int[] { 0, 0, 0 }, new int[] { 1, 2, 3 }, 5 /* abcde */);
        }
    }
}