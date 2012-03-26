using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// POCO for D
	/// </summary>
	[Serializable]
	public class D
	{
		#region Fields

		private Int64 _id;
		private Double _amount;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for class D
		/// </summary>
		public D()
		{
			// try to induce an infinite loop in the lazy-loading machinery
			Amount = 100.0f;
			Double a = Amount;
		}

		/// <summary>
		/// Constructor for class D
		/// </summary>
		/// <param name="id">Initial id value</param>
		/// <param name="amount">Initial amount value</param>
		public D(Int64 id, Double amount)
		{
			_id = id;
			_amount = amount;
		}

		/// <summary>
		/// Minimal constructor for class D
		/// </summary>
		/// <param name="id">Initial id value</param>
		public D(Int64 id)
		{
			_id = id;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get/set for id
		/// </summary>
		public virtual Int64 Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Get/set for Amount
		/// </summary>
		public virtual Double Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}

		#endregion
	}
}