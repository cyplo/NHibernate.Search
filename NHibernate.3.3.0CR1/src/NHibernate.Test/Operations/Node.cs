using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Operations
{
	public class Node
	{
		private ISet<Node> cascadingChildren = new IESI.HashedSet<Node>();
		private ISet<Node> children = new IESI.HashedSet<Node>();
		private DateTime created = DateTime.Now;

		public virtual string Name { get; set; }

		public virtual string Description { get; set; }

		public virtual DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		public virtual Node Parent { get; set; }

		public virtual ISet<Node> Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual ISet<Node> CascadingChildren
		{
			get { return cascadingChildren; }
			set { cascadingChildren = value; }
		}

		public virtual Node AddChild(Node child)
		{
			children.Add(child);
			child.Parent = this;
			return this;
		}
	}
}