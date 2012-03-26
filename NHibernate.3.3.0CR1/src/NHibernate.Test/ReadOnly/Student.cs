using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.ReadOnly
{
	public class Student
	{
		private long studentNumber;
		private string name;
		private Course preferredCourse;
		private IESI.ISet<Enrolment> enrolments = new IESI.HashedSet<Enrolment>();
		
		public virtual long StudentNumber
		{
			get { return studentNumber; }
			set { studentNumber = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual Course PreferredCourse
		{
			get { return preferredCourse; }
			set { preferredCourse = value; }
		}
		
		public virtual IESI.ISet<Enrolment> Enrolments
		{
			get { return enrolments; }
			set { enrolments = value; }
		}
	}
}
