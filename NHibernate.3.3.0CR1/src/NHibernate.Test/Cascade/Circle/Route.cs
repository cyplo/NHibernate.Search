﻿using System;
using System.Text;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Cascade.Circle
{
	public class Route
	{
		private long routeId;
		private long version;
		private IESI.ISet<Node> nodes = new IESI.HashedSet<Node>();
		private IESI.ISet<Vehicle> vehicles = new IESI.HashedSet<Vehicle>();
		private string name;
		private string transientField = null;
		
		public virtual long RouteId
		{
			get { return routeId; }
			set { routeId = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual IESI.ISet<Node> Nodes
		{
			get { return nodes; }
			set { nodes = value; }
		}
		
		public virtual IESI.ISet<Vehicle> Vehicles
		{
			get { return vehicles; }
			set { vehicles = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual string TransientField
		{
			get { return transientField; }
			set { transientField = value; }
		}
		
		public override string ToString()
		{
			var buffer = new StringBuilder();
		
			buffer.AppendFormat("Route name: {0}, id: {1}, transientField: {2}", name, routeId, transientField);
		
			foreach(Node node in nodes)
				buffer.AppendFormat("Node: {0}", node.ToString());

			foreach(Vehicle vehicle in vehicles)
				buffer.AppendFormat("Vehicle: {0}", vehicle.ToString());
			
			return buffer.ToString();
		}
	}
}