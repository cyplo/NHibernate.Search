using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2366
{
	public class One
	{
		private int id;
		private string value;
		private IESI.ISet<Two> twos = new IESI.HashedSet<Two>();
		
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Value
		{
			get { return value; }
			set { this.value = value; }
		}
		
		public virtual IESI.ISet<Two> Twos
		{
			get { return twos; }
			set { twos = value; }
		}
		
		public One()
		{
		}
	}
	
	public class Two
	{
		private int id;
		private string value;
		
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Value
		{
			get { return value; }
			set { this.value = value; }
		}
		
		public Two()
		{
		}
	}
}
