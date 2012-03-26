using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.ReadOnly
{
	[Serializable]
	public class Container
	{
		private long id;
		private string name;
		//private Owner noProxyOwner;
		private Owner proxyOwner;
		private Owner nonLazyOwner;
		//private Info noProxyInfo;
		private Info proxyInfo;
		private Info nonLazyInfo;
		private IESI.ISet<DataPoint> lazyDataPoints = new IESI.HashedSet<DataPoint>();
		private IESI.ISet<DataPoint> nonLazyJoinDataPoints = new IESI.HashedSet<DataPoint>();
		private IESI.ISet<DataPoint> nonLazySelectDataPoints = new IESI.HashedSet<DataPoint>();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
//		public virtual Owner NoProxyOwner
//		{
//			get { return noProxyOwner; }
//			set { noProxyOwner = value; }
//		}
		
		public virtual Owner ProxyOwner
		{
			get { return proxyOwner; }
			set { proxyOwner = value; }
		}
		
		public virtual Owner NonLazyOwner
		{
			get { return nonLazyOwner; }
			set { nonLazyOwner = value; }
		}

//		public virtual Info NoProxyInfo
//		{
//			get { return noProxyInfo; }
//			set { noProxyInfo = value; }
//		}

		public virtual Info ProxyInfo
		{
			get { return proxyInfo; }
			set { proxyInfo = value; }
		}

		public virtual Info NonLazyInfo
		{
			get { return nonLazyInfo; }
			set { nonLazyInfo = value; }
		}
		
		public virtual IESI.ISet<DataPoint> LazyDataPoints
		{
			get { return lazyDataPoints; }
			set { this.lazyDataPoints = value; }
		}

		public virtual IESI.ISet<DataPoint> NonLazyJoinDataPoints
		{
			get { return nonLazyJoinDataPoints; }
			set { this.nonLazyJoinDataPoints = value; }
		}
		
		public virtual IESI.ISet<DataPoint> NonLazySelectDataPoints
		{
			get { return nonLazySelectDataPoints; }
			set { this.nonLazySelectDataPoints = value; }
		}
		
		public Container()
		{
		}
		
		public Container(string name)
		{
			this.name = name;
		}
	}
}
