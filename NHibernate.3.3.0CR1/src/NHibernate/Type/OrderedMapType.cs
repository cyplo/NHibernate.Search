using System;
using System.Collections.Specialized;

namespace NHibernate.Type
{
	/// <summary>
	/// A <see cref="MapType" /> implemented using a collection that maintains
	/// the order in which elements are inserted into it.
	/// </summary>
	[Serializable]
	public class OrderedMapType : MapType
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="OrderedMapType"/> class.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="propertyRef"></param>
		/// <param name="isEmbeddedInXML"></param>
		public OrderedMapType(string role, string propertyRef, bool isEmbeddedInXML)
			: base(role, propertyRef, isEmbeddedInXML)
		{
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new ListDictionary();
		}
	}
}