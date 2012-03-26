﻿using System;
using System.Data;

namespace NHibernate.Dialect
{
	[Obsolete("Please use SybaseSQLAnywhere10Dialect instead. This dialect will be removed in a future release.")]
	public class SybaseASA10Dialect : SybaseASA9Dialect
	{
		public SybaseASA10Dialect()
		{
			RegisterColumnType(DbType.StringFixedLength, 255, "NCHAR($l)");
			RegisterColumnType(DbType.String, 1073741823, "LONG NVARCHAR");
			RegisterColumnType(DbType.String, 255, "NVARCHAR($l)");
			RegisterColumnType(DbType.String, "LONG NVARCHAR");
		}
	}
}