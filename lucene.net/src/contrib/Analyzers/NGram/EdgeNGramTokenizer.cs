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

using System.IO;
using System.Collections;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Util;

namespace Lucene.Net.Analysis.NGram
{

    /**
     * Tokenizes the input from an edge into n-grams of given size(s).
     * <p>
     * This {@link Tokenizer} create n-grams from the beginning edge or ending edge of a input token.
     * MaxGram can't be larger than 1024 because of limitation.
     * </p>
     */
    public class EdgeNGramTokenizer : Tokenizer
    {
        public static Side DEFAULT_SIDE = Side.FRONT;
        public static int DEFAULT_MAX_GRAM_SIZE = 1;
        public static int DEFAULT_MIN_GRAM_SIZE = 1;

        private TermAttribute termAtt;
        private OffsetAttribute offsetAtt;

        // Replace this with an enum when the Java 1.5 upgrade is made, the impl will be simplified
        /** Specifies which side of the input the n-gram should be generated from */
        public class Side
        {
            private string label;

            /** Get the n-gram from the front of the input */
            public static Side FRONT = new Side("front");

            /** Get the n-gram from the end of the input */
            public static Side BACK = new Side("back");

            // Private ctor
            private Side(string label) { this.label = label; }


            public string getLabel() { return label; }

            // Get the appropriate Side from a string
            public static Side getSide(string sideName)
            {
                if (FRONT.getLabel().Equals(sideName))
                {
                    return FRONT;
                }
                else if (BACK.getLabel().Equals(sideName))
                {
                    return BACK;
                }
                return null;
            }
        }

        private int minGram;
        private int maxGram;
        private int gramSize;
        private Side side;
        private bool started = false;
        private int inLen;
        private string inStr;


        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         *
         * @param input {@link Reader} holding the input to be tokenized
         * @param side the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(TextReader input, Side side, int minGram, int maxGram)
            : base(input)
        {
            init(side, minGram, maxGram);
        }

        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         *
         * @param source {@link AttributeSource} to use
         * @param input {@link Reader} holding the input to be tokenized
         * @param side the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(AttributeSource source, TextReader input, Side side, int minGram, int maxGram)
            : base(source, input)
        {

            init(side, minGram, maxGram);
        }

        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         * 
         * @param factory {@link org.apache.lucene.util.AttributeSource.AttributeFactory} to use
         * @param input {@link Reader} holding the input to be tokenized
         * @param side the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(AttributeFactory factory, TextReader input, Side side, int minGram, int maxGram)
            : base(factory, input)
        {

            init(side, minGram, maxGram);
        }

        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         *
         * @param input {@link Reader} holding the input to be tokenized
         * @param sideLabel the name of the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(TextReader input, string sideLabel, int minGram, int maxGram)
            : this(input, Side.getSide(sideLabel), minGram, maxGram)
        {

        }

        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         *
         * @param source {@link AttributeSource} to use
         * @param input {@link Reader} holding the input to be tokenized
         * @param sideLabel the name of the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(AttributeSource source, TextReader input, string sideLabel, int minGram, int maxGram)
            : this(source, input, Side.getSide(sideLabel), minGram, maxGram)
        {

        }

        /**
         * Creates EdgeNGramTokenizer that can generate n-grams in the sizes of the given range
         * 
         * @param factory {@link org.apache.lucene.util.AttributeSource.AttributeFactory} to use
         * @param input {@link Reader} holding the input to be tokenized
         * @param sideLabel the name of the {@link Side} from which to chop off an n-gram
         * @param minGram the smallest n-gram to generate
         * @param maxGram the largest n-gram to generate
         */
        public EdgeNGramTokenizer(AttributeFactory factory, TextReader input, string sideLabel, int minGram, int maxGram) :
            this(factory, input, Side.getSide(sideLabel), minGram, maxGram)
        {
        }

        private void init(Side side, int minGram, int maxGram)
        {
            if (side == null)
            {
                throw new System.ArgumentException("sideLabel must be either front or back");
            }

            if (minGram < 1)
            {
                throw new System.ArgumentException("minGram must be greater than zero");
            }

            if (minGram > maxGram)
            {
                throw new System.ArgumentException("minGram must not be greater than maxGram");
            }

            this.minGram = minGram;
            this.maxGram = maxGram;
            this.side = side;

            this.termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));
            this.offsetAtt = (OffsetAttribute)AddAttribute(typeof(OffsetAttribute));

        }

        /** Returns the next token in the stream, or null at EOS. */
        public override bool IncrementToken()
        {
            ClearAttributes();
            // if we are just starting, read the whole input
            if (!started)
            {
                started = true;
                char[] chars = new char[1024];
                inStr = input.ReadToEnd().Trim();  // remove any leading or trailing spaces
                inLen = inStr.Length;
                gramSize = minGram;
            }

            // if the remaining input is too short, we can't generate any n-grams
            if (gramSize > inLen)
            {
                return false;
            }

            // if we have hit the end of our n-gram size range, quit
            if (gramSize > maxGram)
            {
                return false;
            }

            // grab gramSize chars from front or back
            int start = side == Side.FRONT ? 0 : inLen - gramSize;
            int end = start + gramSize;
            termAtt.SetTermBuffer(inStr, start, gramSize);
            offsetAtt.SetOffset(CorrectOffset(start), CorrectOffset(end));
            gramSize++;
            return true;
        }

        public override void End()
        {
            // set offset
            int finalOffset = inLen;
            this.offsetAtt.SetOffset(finalOffset, finalOffset);
        }

        /** @deprecated Will be removed in Lucene 3.0. This method is final, as it should
         * not be overridden. Delegates to the backwards compatibility layer. */
        [System.Obsolete("Will be removed in Lucene 3.0. This method is final, as it should not be overridden. Delegates to the backwards compatibility layer.")]
        public override Token Next(Token reusableToken)
        {
            return base.Next(reusableToken);
        }

        /** @deprecated Will be removed in Lucene 3.0. This method is final, as it should
         * not be overridden. Delegates to the backwards compatibility layer. */
        [System.Obsolete("Will be removed in Lucene 3.0. This method is final, as it should not be overridden. Delegates to the backwards compatibility layer.")]
        public override Token Next()
        {
            return base.Next();
        }

        public override void Reset(TextReader input)
        {
            base.Reset(input);
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            started = false;
        }
    }
}