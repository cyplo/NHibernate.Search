using System;
using System.Collections.Generic;
using System.Text;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2280
{
    public class Organisation
    {
        public virtual Guid OrganisationId { get; set; }

        public virtual IESI.ISet<OrganisationCode> Codes { get; protected internal set; }

        public virtual string LegalName { get; set; }
        public virtual string Abn { get; set; }
        public virtual string Acn { get; set; }
    }
}
