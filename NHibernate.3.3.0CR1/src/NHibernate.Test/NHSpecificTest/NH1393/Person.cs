using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH1393
{
	public class Person
	{
		private int id;
		private int iq;
		private string name;
		private IList pets;
		private int shoeSize;

		public Person()
		{
			pets = new ArrayList();
		}

		public Person(string name, int iq, int shoeSize)
		{
			this.name = name;
			this.iq = iq;
			this.shoeSize = shoeSize;
			pets = new ArrayList();
		}

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

		public virtual int IQ
		{
			get { return iq; }
			set { iq = value; }
		}

		public virtual int ShoeSize
		{
			get { return shoeSize; }
			set { shoeSize = value; }
		}

		public virtual IList Pets
		{
			get { return pets; }
			protected set { pets = value; }
		}
	}

	public class Pet
	{
		private int id;
		private string name;
		private Person owner;
		private string species;
		private double weight;

		public Pet() {}

		public Pet(string name, string species, int weight, Person owner)
		{
			this.name = name;
			this.species = species;
			this.weight = weight;
			this.owner = owner;
		}

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

		public virtual string Species
		{
			get { return species; }
			set { species = value; }
		}

		public virtual double Weight
		{
			get { return weight; }
			set { weight = value; }
		}

		public virtual Person Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}
}