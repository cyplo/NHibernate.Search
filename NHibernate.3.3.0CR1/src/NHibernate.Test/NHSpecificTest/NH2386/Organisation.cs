using System;
using System.Collections.Generic;
using System.Text;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2386
{
    public class Organisation
    {
        //internal to TGA
        //private int organisationId;
        public virtual Guid OrganisationId { get; set; }
        private IESI.ISet<TradingName> tradingNames;
        private IESI.ISet<ResponsibleLegalPerson> responsibleLegalPersons;

        /// <summary>
        /// 
        /// </summary>
        

         public virtual IESI.ISet<ResponsibleLegalPerson> ResponsibleLegalPersons {
            get {
                if (responsibleLegalPersons == null) {
                    responsibleLegalPersons = new IESI.HashedSet<ResponsibleLegalPerson>();
                }
                return responsibleLegalPersons;
            }
            protected set {
                responsibleLegalPersons = value;
               
            }
        }

        public virtual IESI.ISet<TradingName> TradingNames {
            get {
                if (tradingNames == null) {
                    tradingNames = new IESI.HashedSet<TradingName>();
                }
                return tradingNames;
            }
            protected set {
                tradingNames = value;
               
            }
        }

         protected internal virtual byte[] RowVersion { get; protected set; }

    }

}
