using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;

namespace CodeGenerator.Core.Database
{
	public class InformationSchemaColumns
	{
		public string TABLE_CATALOG { get; set; }
		public string TABLE_SCHEMA { get; set; }
		public string TABLE_NAME { get; set; }
		public string COLUMN_NAME { get; set; }
		public bool IS_NULLABLE { get; set; }
		public string DATA_TYPE { get; set; }
		public int CHARACTER_MAXIMUM_LENGTH { get; set; }
		public int NUMERIC_SCALE { get; set; }

		public string FK_NAME { get; set; }
		public string PARENT_TABLE_SCHEMA { get; set; }
		public string PARENT_TABLE_NAME { get; set; }
		public string PARENT_TABLE_COLUMN { get; set; }
		public string REFERENCED_TABLE_SCHEMA { get; set; }
		public string REFERENCED_TABLE_NAME { get; set; }
		public string REFERENCED_TABLE_COLUMN { get; set; }
		public bool PRIMARY_KEY { get; set; }
		public bool IS_IDENTITY { get; set; }

		public InformationSchemaColumns()
		{ }

		public InformationSchemaColumns(SqlDataReader reader)
		{
			try
			{
				TABLE_CATALOG = reader.GetString(0);
				TABLE_SCHEMA = reader.GetString(1);
				TABLE_NAME = reader.GetString(2);
				COLUMN_NAME = reader.GetString(3);
				IS_NULLABLE = reader.GetString(4) == "YES" ? true : false;
				DATA_TYPE = reader.GetString(5);

				CHARACTER_MAXIMUM_LENGTH = reader.GetValue(6) is System.DBNull ? 0 : (int)reader.GetValue(6);

				NUMERIC_SCALE = reader.GetValue(7) is System.DBNull ? 0 : (int)reader.GetValue(7);

				FK_NAME = reader.GetValue(8) is System.DBNull ? string.Empty : reader.GetString(8);
				PARENT_TABLE_SCHEMA = reader.GetValue(9) is System.DBNull ? string.Empty : reader.GetString(9);
				PARENT_TABLE_NAME = reader.GetValue(10) is System.DBNull ? string.Empty : reader.GetString(10);
				PARENT_TABLE_COLUMN = reader.GetValue(11) is System.DBNull ? string.Empty : reader.GetString(11);
				REFERENCED_TABLE_SCHEMA = reader.GetValue(12) is System.DBNull ? string.Empty : reader.GetString(12);
				REFERENCED_TABLE_NAME = reader.GetValue(13) is System.DBNull ? string.Empty : reader.GetString(13);
				REFERENCED_TABLE_COLUMN = reader.GetValue(14) is System.DBNull ? string.Empty : reader.GetString(14);

				PRIMARY_KEY = reader.GetString(15) == "YES" ? true : false;
				IS_IDENTITY = reader.GetString(15) == "YES" ? true : false;
			}
			catch (Exception)
			{
				throw;
			}
		}

		//public static IEnumerable<IGrouping<string, string>> DEPENDENT_TABLE_NAME(List<InformationSchemaColumns> _InformationSchemaColumns, string tableName) 
		//{
		//	var result = from isc in _InformationSchemaColumns
		//	where isc.REFERENCED_TABLE_NAME != string.Empty && isc.REFERENCED_TABLE_NAME == tableName
		//	group isc.TABLE_NAME by isc.TABLE_NAME;

		//	return result;
		//}

	}
}

