using System;

namespace NHibernate.Search.Tests.Embedded.DoubleInsert
{
    using Attributes;

    using IESI = Iesi.Collections.Generic;

    [Indexed]
    public class Contact
    {
        [DocumentId]
        private long id;
        [Field(Index = Index.Tokenized, Store = Store.Yes)]
        private string email;
        private DateTime createdOn;
        private DateTime lastUpdatedOn;
        [ContainedIn]
        private IESI.ISet<Address> addresses;
        [ContainedIn]
        private IESI.ISet<Phone> phoneNumbers;
        [Field(Index = Index.Tokenized, Store = Store.Yes)]
        private string notes;

        #region Constructors

        public Contact()
        {
            addresses = new IESI.HashedSet<Address>();
            phoneNumbers = new IESI.HashedSet<Phone>();
        }

        #endregion

        #region Property methods

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public DateTime LastUpdatedOn
        {
            get { return lastUpdatedOn; }
            set { lastUpdatedOn = value; }
        }

        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        public IESI.ISet<Address> Addresses
        {
            get { return addresses; }
        }

        public IESI.ISet<Phone> PhoneNumbers
        {
            get { return phoneNumbers; }
        }

        #endregion

        #region Public methods

        public void AddAddressToContact(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            address.Contact = this;
            Addresses.Add(address);
        }

        public void AddPhoneToContact(Phone phone)
        {
            if (phone == null)
            {
                throw new ArgumentNullException("phone");
            }

            phone.Contact = this;
            PhoneNumbers.Add(phone);
        }

        public void RemoveAddressFromContact(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (Addresses.Contains(address))
            {
                Addresses.Remove(address);
            }
        }

        public void RemovePhoneFromContact(Phone phone)
        {
            if (phone == null)
            {
                throw new ArgumentNullException("phone");
            }

            if (PhoneNumbers.Contains(phone))
            {
                PhoneNumbers.Remove(phone);
            }
        }

        #endregion
    }
}