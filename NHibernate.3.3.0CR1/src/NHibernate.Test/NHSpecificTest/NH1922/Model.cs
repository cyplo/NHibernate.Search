using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1922
{
    public class Customer 
    {
			public virtual int ID { get; protected set; }
        public virtual DateTime ValidUntil { get; set; }
    }
}
