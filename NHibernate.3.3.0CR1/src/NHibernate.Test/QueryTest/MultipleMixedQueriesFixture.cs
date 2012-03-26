using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Driver;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	public class MultipleMixedQueriesFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
		}

		[TestFixtureSetUp]
		public void CheckMultiQuerySupport()
		{
			TestFixtureSetUp();
			IDriver driver = sessions.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
			{
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
			}
		}

		[Test]
		public void NH_1085_WillIgnoreParametersIfDoesNotAppearInQuery()
		{
			using (ISession s = sessions.OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids)").AddEntity(typeof (Item)))
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids2)").AddEntity(typeof (Item)))
					.SetParameterList("ids", new[] {50})
					.SetParameterList("ids2", new[] {50});
				multiQuery.List();
			}
		}

		[Test]
		public void NH_1085_WillGiveReasonableErrorIfBadParameterName()
		{
			using (ISession s = sessions.OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids)").AddEntity(typeof(Item)))
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids2)").AddEntity(typeof(Item)));
				var e = Assert.Throws<QueryException>(() => multiQuery.List());
				Assert.That(e.Message, Is.EqualTo("Not all named parameters have been set: ['ids'] [select * from ITEM where Id in (:ids)]"));
			}
		}

		[Test]
		public void CanGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMutiQueryAndAssert();

			Hashtable cacheHashtable = MultipleQueriesFixture.GetHashTableUsedAsQueryCache(sessions);
			IList cachedListEntry = (IList)new ArrayList(cacheHashtable.Values)[0];
			IList cachedQuery = (IList)cachedListEntry[1];

			IList firstQueryResults = (IList)cachedQuery[0];
			firstQueryResults.Clear();
			firstQueryResults.Add(3);
			firstQueryResults.Add(4);

			IList secondQueryResults = (IList)cachedQuery[1];
			secondQueryResults[0] = 2L;

			using (ISession s = sessions.OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				IList results = multiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(2, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(2L, count);
			}

			RemoveAllItems();
		}

		[Test]
		public void CanSpecifyParameterOnMultiQueryWhenItIsNotUsedInAllQueries()
		{
			using (ISession s = OpenSession())
			{
				s.CreateMultiQuery()
					.Add("from Item")
					.Add(s.CreateSQLQuery("select * from ITEM where Id > :id").AddEntity(typeof(Item)))
					.SetParameter("id", 5)
					.List();
			}
		}

		[Test]
		public void CanSpecifyParameterOnMultiQueryWhenItIsNotUsedInAllQueries_MoreThanOneParameter()
		{
			using (ISession s = OpenSession())
			{
				s.CreateMultiQuery()
					.Add("from Item")
					.Add(s.CreateSQLQuery("select * from ITEM where Id = :id or Id = :id2").AddEntity(typeof(Item)))
					.Add("from Item i where i.Id = :id2")
					.SetParameter("id", 5)
					.SetInt32("id2", 5)
					.List();
			}
		}

		[Test]
		public void TwoMultiQueriesWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (ISession s = OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateSQLQuery("select count(*) as count from ITEM where Id > ?").AddScalar("count", NHibernateUtil.Int64)
							.SetInt32(0, 50));

				multiQuery.SetCacheable(true);
				IList results = multiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			using (ISession s = OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
							 .SetInt32(0, 50)
							 .SetFirstResult(20))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				IList results = multiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(79, items.Count,
								"Should have gotten different result here, because the paging is different");
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParameters()
		{
			Hashtable cacheHashtable = MultipleQueriesFixture.GetHashTableUsedAsQueryCache(sessions);
			cacheHashtable.Clear();

			CreateItems();

			DoMutiQueryAndAssert();

			Assert.AreEqual(1, cacheHashtable.Count);

			RemoveAllItems();
		}

		private void DoMutiQueryAndAssert()
		{
			using (ISession s = OpenSession())
			{
				IMultiQuery multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				IList results = multiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		private void CreateItems()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				for (int i = 0; i < 150; i++)
				{
					Item item = new Item();
					item.Id = i;
					s.Save(item);
				}
				t.Commit();
			}
		}

		[Test]
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			using (ISession s = OpenSession())
			{
				for (int i = 0; i < 150; i++)
				{
					Item item = new Item();
					item.Id = i;
					s.Save(item);
				}
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IQuery getItems = s.CreateSQLQuery("select * from ITEM where Id > :id").AddEntity(typeof(Item)).SetFirstResult(10);
				IQuery countItems = s.CreateQuery("select count(*) from Item i where i.Id > :id");

				IList results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.SetInt32("id", 50)
					.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
		}

		private void RemoveAllItems()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}

		[Test]
		public void CanUseSetParameterList()
		{
			using (ISession s = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IList results = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:items)").AddEntity(typeof(Item)))
					.Add("select count(*) from Item i where i.id in (:items)")
					.SetParameterList("items", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })
					.List();

				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				long count = (long)counts[0];
				Assert.AreEqual(1L, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}

		[Test]
		public void CanExecuteMultiplyQueriesInSingleRoundTrip()
		{
			using (ISession s = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IQuery getItems = s.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
				IQuery countItems = s.CreateQuery("select count(*) from Item");

				IList results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.List();
				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				long count = (long)counts[0];
				Assert.AreEqual(1L, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}

		[Test]
		public void CanAddIQueryWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (ISession session = OpenSession())
			{
				IMultiQuery multiQuery = session.CreateMultiQuery();

				IQuery firstQuery = session.CreateSQLQuery("select * from ITEM where Id < :id").AddEntity(typeof(Item))
					.SetInt32("id", 50);

				IQuery secondQuery = session.CreateQuery("from Item");

				multiQuery.Add("first", firstQuery).Add("second", secondQuery);

				IList secondResult = (IList)multiQuery.GetResult("second");
				IList firstResult = (IList)multiQuery.GetResult("first");

				Assert.Greater(secondResult.Count, firstResult.Count);
			}

			RemoveAllItems();
		}

		[Test]
		public void CanNotAddQueryWithKeyThatAlreadyExists()
		{
			using (ISession session = OpenSession())
			{
				IMultiQuery multiQuery = session.CreateMultiQuery();

				IQuery firstQuery = session.CreateSQLQuery("select * from ITEM where Id < :id").AddEntity(typeof(Item))
					.SetInt32("id", 50);

				try
				{
					IQuery secondQuery = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
					multiQuery.Add("first", firstQuery).Add("second", secondQuery);
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		[Test]
		public void CanNotRetrieveQueryResultWithUnknownKey()
		{
			using (ISession session = OpenSession())
			{
				IMultiQuery multiQuery = session.CreateMultiQuery();

				multiQuery.Add("firstQuery", session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item)));

				try
				{
					multiQuery.GetResult("unknownKey");
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		[Test]
		public void ExecutingQueryThroughMultiQueryTransformsResults()
		{
			CreateItems();

			using (ISession session = OpenSession())
			{
				ResultTransformerStub transformer = new ResultTransformerStub();
				IQuery query = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item))
					.SetResultTransformer(transformer);
				session.CreateMultiQuery()
					.Add(query)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}

			RemoveAllItems();
		}

		[Test]
		public void ExecutingQueryThroughMultiQueryTransformsResults_When_setting_on_multi_query_directly()
		{
			CreateItems();

			using (ISession session = OpenSession())
			{
				ResultTransformerStub transformer = new ResultTransformerStub();
				IQuery query = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
				session.CreateMultiQuery()
					.Add(query)
					.SetResultTransformer(transformer)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}

			RemoveAllItems();
		}

		[Test]
		public void CanGetResultsInAGenericList()
		{
			using (ISession s = OpenSession())
			{
				IQuery getItems = s.CreateQuery("from Item");
				IQuery countItems = s.CreateSQLQuery("select count(*) as count from ITEM").AddScalar("count", NHibernateUtil.Int64);

				IList results = s.CreateMultiQuery()
					.Add(getItems)
					.Add<long>(countItems)
					.List();

				Assert.That(results[0], Is.InstanceOf<ArrayList>());
				Assert.That(results[1], Is.InstanceOf<List<long>>());
			}
		}
	}
}