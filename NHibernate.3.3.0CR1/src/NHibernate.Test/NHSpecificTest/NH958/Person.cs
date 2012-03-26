using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH958
{
    public class Person
    {
        private int _id;
        private string _name = null;
        private IList _hobbies = new ArrayList();

        public Person()
        {
        }

        public Person(string name) : this()
        {
            _name = name;
        }

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual IList Hobbies
        {
            get { return _hobbies; }
            set { _hobbies = value; }
        }

        public virtual void AddHobby(Hobby hobby)
        {
            hobby.Person = this;
            _hobbies.Add(hobby);
        }

        public virtual void RemoveHobby(Hobby hobby)
        {
            _hobbies.Remove(hobby);
        }
    }
}
