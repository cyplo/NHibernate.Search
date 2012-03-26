﻿using System;

namespace NHibernate.Test.NHSpecificTest.NH2761
{
	public class C
	{
		public C()
		{
			this.As = new Iesi.Collections.Generic.IESI.HashedSet<A>();
			this.Bs = new Iesi.Collections.Generic.IESI.HashedSet<B>();
		}

		public Int32 Id
		{
			get;
			set;
		}

		public String CProperty
		{
			get;
			set;
		}

		public Iesi.Collections.Generic.IESI.ISet<B> Bs
		{
			get;
			set;
		}

		public Iesi.Collections.Generic.IESI.ISet<A> As
		{
			get;
			set;
		}

		public override Int32 GetHashCode()
		{
			return (this.Id.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return (false);
			}

			if (this.GetType() != obj.GetType())
			{
				return (false);
			}

			C other = obj as C;

			if (Object.Equals(other.Id, this.Id) == false)
			{
				return (false);
			}

			if (Object.Equals(other.CProperty, this.CProperty) == false)
			{
				return (false);
			}

			return (true);
		}
	}
}