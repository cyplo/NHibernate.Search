using System;
using System.Collections.Generic;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	[Serializable]
	public class Contract
	{
		private long id;
		private long version;
		private string customerName;
		private string type;

		private IList<ContractVariation> variations;
		private Contract parent;
		private IESI.ISet<Contract> subcontracts;
		private IESI.ISet<Plan> plans = new IESI.HashedSet<Plan>();
		private IESI.ISet<Party> parties;
		private IESI.ISet<Info> infos;
		
		public Contract()
		{
		}
		
		public Contract(Plan plan, string customerName, string type)
		{
			if (plan != null)
			{
				plans.Add(plan);
				plan.Contracts.Add(this);
			}
			this.customerName = customerName;
			this.type = type;
			variations = new List<ContractVariation>();
			subcontracts = new IESI.HashedSet<Contract>();
			parties = new IESI.HashedSet<Party>();
			infos = new IESI.HashedSet<Info>();
		}
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual string CustomerName
		{
			get { return customerName; }
			set { customerName = value; }
		}
		
		public virtual string Type
		{
			get { return type; }
			set { type = value; }
		}
		
		public virtual IList<ContractVariation> Variations
		{
			get { return variations; }
			set { variations = value; }
		}
		
		public virtual Contract Parent
		{
			get { return parent; }
			set { parent = value; }
		}
		
		public virtual IESI.ISet<Contract> Subcontracts
		{
			get { return subcontracts; }
			set { subcontracts = value; }
		}
		
		public virtual IESI.ISet<Plan> Plans
		{
			get { return plans; }
			set { plans = value; }
		}
		
		public virtual IESI.ISet<Party> Parties
		{
			get { return parties; }
			set { parties = value; }
		}
		
		public virtual IESI.ISet<Info> Infos
		{
			get { return infos; }
			set { infos = value; }
		}
		
		public virtual void AddSubcontract(Contract subcontract)
		{
			subcontracts.Add(subcontract);
			subcontract.Parent = this;
		}
		
		public virtual void AddParty(Party party)
		{
			parties.Add(party);
			party.Contract = this;
		}
	
		public virtual void RemoveParty(Party party)
		{
			parties.Remove(party);
			party.Contract = null;
		}
	}
}
