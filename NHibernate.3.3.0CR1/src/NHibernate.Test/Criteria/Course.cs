using System;
using IESI = Iesi.Collections.Generic;

namespace NHibernate.Test.Criteria
{
	public class Course
	{
		private string courseCode;
		private string description;
		private IESI.ISet<CourseMeeting> courseMeetings = new IESI.HashedSet<CourseMeeting>();
		
		public virtual string CourseCode
		{
			get { return courseCode; }
			set { courseCode = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
		
		public virtual IESI.ISet<CourseMeeting> CourseMeetings
		{
			get { return courseMeetings; }
			set { courseMeetings = value; }
		}
	}
}