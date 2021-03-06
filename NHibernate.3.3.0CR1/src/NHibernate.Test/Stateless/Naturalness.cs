using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Stateless
{
	public abstract class Animal
	{
		public virtual int Id { get; protected set; }
		public virtual string Description { get; set; }
	}

	public class Reptile: Animal
	{
		public virtual float BodyTemperature { get; set; }
	}

	public class Human : Animal
	{
		public virtual string Name { get; set; }
		public virtual string NickName { get; set; }
		public virtual DateTime Birthdate { get; set; }
	}

	public class Family<T> where T: Animal
	{
		private IESI.ISet<T> childs;

		public virtual int Id { get; protected set; }
		public virtual T Father { get; set; }
		public virtual T Mother { get; set; }
		public virtual IESI.ISet<T> Childs
		{
			get { return childs; }
			set { childs = value; }
		}
	}
}