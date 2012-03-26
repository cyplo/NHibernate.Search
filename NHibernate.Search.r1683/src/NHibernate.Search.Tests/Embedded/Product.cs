using System.Collections;
using IESI = Iesi.Collections.Generic;
using NHibernate.Search.Attributes;
using System.Collections.Generic;

namespace NHibernate.Search.Tests.Embedded
{
    [Indexed]
    public class Product
    {
        [DocumentId]
        private int id;
        [Field(Index.Tokenized)]
        private string name;
        [IndexedEmbedded]
        private IESI.ISet<Author> authors = new IESI.HashedSet<Author>();
        [IndexedEmbedded]
        private IDictionary<string, Order> orders = new Dictionary<string, Order>();

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual IESI.ISet<Author> Authors
        {
            get { return authors; }
            set { authors = value; }
        }

        public virtual IDictionary<string, Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }
    }
}
