using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class SubMulti : Multi
	{
		private float _amount;
		private SubMulti _parent;
		private IList _children;
		private IList _moreChildren;

		public float Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}

		public SubMulti Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public IList Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public IList MoreChildren
		{
			get { return _moreChildren; }
			set { _moreChildren = value; }
		}
	}
}