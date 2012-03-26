using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Iesi.Collections;
using IESI = Iesi.Collections.Generic;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Generic
{
	/// <summary>
	/// .NET has no design equivalent for Java's Set so we are going to use the
	/// Iesi.Collections library. This class is internal to NHibernate and shouldn't
	/// be used by user code.
	/// </summary>
	/// <remarks>
	/// The code for the Iesi.Collections library was taken from the article
	/// <a href="http://www.codeproject.com/csharp/sets.asp">Add Support for "Set" Collections
	/// to .NET</a> that was written by JasonSmith.
	/// </remarks>
	[Serializable]
	[DebuggerTypeProxy(typeof (CollectionProxy<>))]
    public class PersistentGenericSet<T> : PersistentSet, IESI.ISet<T>
	{
		// TODO NH: find a way to writeonce (no duplicated code from PersistentSet)

		/* NH considerations:
		 * The implementation of Set<T> in Iesi collections don't have any particular behavior
		 * for strongly typed. BTW we use the same technique used for other collection.
		 */
		protected IESI.ISet<T> gset;
		[NonSerialized] private IList<T> readList;

		public PersistentGenericSet() {}
		public PersistentGenericSet(ISessionImplementor session) : base(session) {}

		public PersistentGenericSet(ISessionImplementor session, IESI.ISet<T> original) : base(session, original as ISet)
		{
			gset = original;
			set = (ISet) original;
		}

		public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
		{
			gset = (IESI.ISet<T>) persister.CollectionType.Instantiate(anticipatedSize);
			set = (ISet) gset;
		}

		public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
		{
			object element = role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
			if (element != null)
			{
				readList.Add((T) element);
			}
			return element;
		}

		public override void BeginRead()
		{
			base.BeginRead();
			readList = new List<T>();
		}

		public override bool EndRead(ICollectionPersister persister)
		{
			gset.AddAll(readList);
			readList = null;
			SetInitialized();
			return true;
		}

		#region IESI.ISet<T> Members

        IESI.ISet<T> IESI.ISet<T>.Union(IESI.ISet<T> a)
		{
			Read();
			return gset.Union(a);
		}

        IESI.ISet<T> IESI.ISet<T>.Intersect(IESI.ISet<T> a)
		{
			Read();
			return gset.Intersect(a);
		}

        IESI.ISet<T> IESI.ISet<T>.Minus(IESI.ISet<T> a)
		{
			Read();
			return gset.Minus(a);
		}

		IESI.ISet<T> IESI.ISet<T>.ExclusiveOr(IESI.ISet<T> a)
		{
			Read();
			return gset.ExclusiveOr(a);
		}

		bool IESI.ISet<T>.ContainsAll(ICollection<T> c)
		{
			Read();
			return gset.ContainsAll(c);
		}

		bool IESI.ISet<T>.Add(T o)
		{
			return Add(o);
		}

		bool IESI.ISet<T>.AddAll(ICollection<T> c)
		{
			if (c.Count > 0)
			{
				Initialize(true);
				if (gset.AddAll(c))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		bool IESI.ISet<T>.RemoveAll(ICollection<T> c)
		{
			if (c.Count > 0)
			{
				Initialize(true);
				if (gset.RemoveAll(c))
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		bool IESI.ISet<T>.RetainAll(ICollection<T> c)
		{
			Initialize(true);
			if (gset.RetainAll(c))
			{
				Dirty();
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		bool ICollection<T>.Contains(T item)
		{
			bool? exists = ReadElementExistence(item);
			return exists == null ? gset.Contains(item) : exists.Value;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			Read();
			gset.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			bool? exists = PutQueueEnabled ? ReadElementExistence(item) : null;
			if (!exists.HasValue)
			{
				Initialize(true);
				bool contained = gset.Remove(item);
				if (contained)
				{
					Dirty();
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (exists.Value)
			{
				QueueOperation(new SimpleRemoveDelayedOperation(this, item));
				return true;
			}
			else
			{
				return false;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			Read();
			return gset.GetEnumerator();
		}

		#endregion

    }
}