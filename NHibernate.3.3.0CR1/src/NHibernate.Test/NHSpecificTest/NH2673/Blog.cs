using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2673
{
	public class Blog
	{
		public Blog()
		{
			Posts = new IESI.HashedSet<Post>();
			Comments = new IESI.HashedSet<Comment>();
		}

		public virtual int Id { get; set; }
		public virtual string Author { get; set; }
		public virtual string Name { get; set; }
		public virtual IESI.ISet<Post> Posts { get; set; }
		public virtual IESI.ISet<Comment> Comments { get; set; }
	}

	public class Post
	{
		public virtual int Id { get; protected set; }
		public virtual string Title { get; set; }
		public virtual string Body { get; set; }
	}


	public class Comment
	{
		public virtual int Id { get; protected set; }
		public virtual string Title { get; set; }
		public virtual string Body { get; set; }
	}
}