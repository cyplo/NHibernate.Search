using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using NHibernate.Search.Engine;
using Directory=Lucene.Net.Store.Directory;

namespace NHibernate.Search.Store
{
    public class FSDirectoryProvider : IDirectoryProvider
    {
		private static IInternalLogger log = LoggerProvider.LoggerFor(typeof(FSDirectoryProvider));
        private FSDirectory directory;
        private String indexName;

        public Directory Directory
        {
            get { return directory; }
        }

        public void Initialize(String directoryProviderName, IDictionary<string, string> properties, ISearchFactoryImplementor searchFactory)
        {
            DirectoryInfo indexDir = DirectoryProviderHelper.DetermineIndexDir(directoryProviderName, (IDictionary) properties);
            try
            {
                bool create = !IndexReader.IndexExists(indexDir.FullName);
                indexName = indexDir.FullName;
                directory = FSDirectory.GetDirectory(indexName, create);
                if (create)
                {
                    IndexWriter iw = new IndexWriter(directory, new StandardAnalyzer(), create);
                    iw.Close();
                }
                //searchFactory.RegisterDirectoryProviderForLocks(this);
            }
            catch (IOException e)
            {
                throw new HibernateException("Unable to initialize index: " + directoryProviderName, e);
            }
        }

        public void Start()
        {
            // All the work is done in initialize
        }

        public override bool Equals(Object obj)
        {
            // this code is actually broken since the value change after initialize call
            // but from a practical POV this is fine since we only call this method
            // after initialize call
            if (obj == this) return true;
            if (obj == null || !(obj is FSDirectoryProvider)) return false;
            return indexName.Equals(((FSDirectoryProvider) obj).indexName);
        }

        public override int GetHashCode()
        {
            // this code is actually broken since the value change after initialize call
            // but from a practical POV this is fine since we only call this method
            // after initialize call
            const int hash = 11;
            return 37*hash + indexName.GetHashCode();
        }
    }
}