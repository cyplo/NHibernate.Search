using System;
using System.Collections;
using System.Threading;
using NHibernate.Criterion;
using NHibernate.Engine;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessSessionFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"Stateless.Document.hbm.xml"}; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Document");
				s.Delete("from Paper");
			}
		}

		[Test]
		public void CreateUpdateReadDelete()
		{
			Document doc;
			DateTime? initVersion;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ITransaction tx;
				using (tx = ss.BeginTransaction())
				{
					doc = new Document("blah blah blah", "Blahs");
					ss.Insert(doc);
					Assert.IsNotNull(doc.LastModified);
					initVersion = doc.LastModified;
					Assert.IsTrue(initVersion.HasValue);
					tx.Commit();
				}
				Thread.Sleep(100); // Only to be secure that next modification have a different version
				using (tx = ss.BeginTransaction())
				{
					doc.Text = "blah blah blah .... blah";
					ss.Update(doc);
					Assert.IsTrue(doc.LastModified.HasValue);
					Assert.AreNotEqual(initVersion, doc.LastModified);
					tx.Commit();
				}
				using (tx = ss.BeginTransaction())
				{
					doc.Text = "blah blah blah .... blah blay";
					ss.Update(doc);
					tx.Commit();
				}
				var doc2 = ss.Get<Document>("Blahs");
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document) ss.CreateQuery("from Document where text is not null").UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document) ss.CreateSQLQuery("select * from Document").AddEntity(typeof (Document)).UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = ss.CreateCriteria<Document>().UniqueResult<Document>();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document) ss.CreateCriteria(typeof (Document)).UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				using (tx = ss.BeginTransaction())
				{
					ss.Delete(doc);
					tx.Commit();
				}
			}
		}

		[Test]
		public void HqlBulk()
		{
			IStatelessSession ss = sessions.OpenStatelessSession();
			ITransaction tx = ss.BeginTransaction();
			var doc = new Document("blah blah blah", "Blahs");
			ss.Insert(doc);
			var paper = new Paper {Color = "White"};
			ss.Insert(paper);
			tx.Commit();

			tx = ss.BeginTransaction();
			int count =
				ss.CreateQuery("update Document set Name = :newName where Name = :oldName").SetString("newName", "Foos").SetString(
					"oldName", "Blahs").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			count = ss.CreateQuery("update Paper set Color = :newColor").SetString("newColor", "Goldenrod").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			tx.Commit();

			tx = ss.BeginTransaction();
			count = ss.CreateQuery("delete Document").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			count = ss.CreateQuery("delete Paper").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			tx.Commit();
			ss.Close();
		}

		[Test]
		public void InitId()
		{
			Paper paper;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ITransaction tx;
				using (tx = ss.BeginTransaction())
				{
					paper = new Paper {Color = "White"};
					ss.Insert(paper);
					Assert.IsTrue(paper.Id != 0);
					tx.Commit();
				}
				using (tx = ss.BeginTransaction())
				{
					ss.Delete(ss.Get<Paper>(paper.Id));
					tx.Commit();
				}
			}
		}

		[Test]
		public void Refresh()
		{
			Paper paper;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				using (ITransaction tx = ss.BeginTransaction())
				{
					paper = new Paper {Color = "whtie"};
					ss.Insert(paper);
					tx.Commit();
				}
			}
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				using (ITransaction tx = ss.BeginTransaction())
				{
					var p2 = ss.Get<Paper>(paper.Id);
					p2.Color = "White";
					ss.Update(p2);
					tx.Commit();
				}
			}
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				using (ITransaction tx = ss.BeginTransaction())
				{
					Assert.AreEqual("whtie", paper.Color);
					ss.Refresh(paper);
					Assert.AreEqual("White", paper.Color);
					ss.Delete(paper);
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenSetTheBatchSizeThenSetTheBatchSizeOfTheBatcher()
		{
            if (!Dialect.SupportsSqlBatches)
                Assert.Ignore("Dialect does not support sql batches.");

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ss.SetBatchSize(37);
				var impl = (ISessionImplementor)ss;
				impl.Batcher.BatchSize.Should().Be(37);
			}
		}

		[Test]
		public void CanGetImplementor()
		{
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ss.GetSessionImplementation().Should().Be.SameInstanceAs(ss);
			}
		}

		[Test]
		public void HavingDetachedCriteriaThenCanGetExecutableCriteriaFromStatelessSession()
		{
			var dc = DetachedCriteria.For<Paper>();
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ICriteria criteria = null;
				Executing.This(()=> criteria = dc.GetExecutableCriteria(ss)).Should().NotThrow();
				criteria.Executing(c => c.List()).NotThrows();
			}
		}
		
		[Test]
		public void DisposingClosedStatelessSessionShouldNotCauseSessionException()
		{
			try
			{
				IStatelessSession ss = sessions.OpenStatelessSession();
				ss.Close();
				ss.Dispose();
			}
			catch (SessionException)
			{
				Assert.Fail();
			}
		}
	}
}