using IESI = Iesi.Collections.Generic;


namespace NHibernate.Test.NHSpecificTest.NH2846
{
	public class Post
	{
		public Post()
		{
			Comments = new IESI.HashedSet<Comment>();
		}

		public virtual int Id { get; set; }

		public virtual string Title { get; set; }

		public virtual Category Category { get; set; }

		public virtual IESI.ISet<Comment> Comments { get; set; }
	}

	public class Category
	{
		public virtual int Id { get; set; }

		public virtual string Title { get; set; }
	}

	public class Comment
	{
		public virtual int Id { get; set; }

		public virtual string Title { get; set; }

		public virtual Post Post { get; set; }
	}
}