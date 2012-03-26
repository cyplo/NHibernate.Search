using System;
using System.Collections.Generic;
using IESI = Iesi.Collections.Generic;


namespace NHibernate.Event
{
	/// <summary>Defines an event class for the auto-flushing of a session. </summary>
	[Serializable]
	public class AutoFlushEvent : FlushEvent
	{
		private IESI.ISet<string> querySpaces;
		private bool flushRequired;

		public AutoFlushEvent(IESI.ISet<string> querySpaces, IEventSource source)
			: base(source)
		{
			this.querySpaces = querySpaces;
		}

		public IESI.ISet<string> QuerySpaces
		{
			get { return querySpaces; }
			set { querySpaces = value; }
		}

		public bool FlushRequired
		{
			get { return flushRequired; }
			set { flushRequired = value; }
		}
	}
}