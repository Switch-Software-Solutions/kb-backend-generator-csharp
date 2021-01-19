using System;
using System.Linq;

namespace T4CodeGenerator.MSSQL2CSHARP.Database
{
	public class SchemaTable
	{
		// Estructura esquema-nombre de la tabla
		public string TABLE_SCHEMA { get; set; }
		public string TABLE_NAME { get; set; }

		public SchemaTable(string TABLE_SCHEMA, string TABLE_NAME)
		{
			this.TABLE_SCHEMA = TABLE_SCHEMA;
			this.TABLE_NAME = TABLE_NAME;
		}

		public static string SchemaTableConcat(string schema, string connector, string table)
		{
			string result = string.Empty;

			if (schema.ToUpper() == "DBO")
			{
				result = table;
			}
			else
			{
				result = schema + connector + table;
			}

			return result;
		}

		public static string SchemaTableReplaceConnector(string schema_table, string oldConnector, string newConnector)
		{
			string result = string.Empty;

			string schema = schema_table.Split(new string[] { oldConnector }, StringSplitOptions.None).First();
			string table = schema_table.Split(new string[] { oldConnector }, StringSplitOptions.None).Last();

			if (schema.ToUpper() == "DBO")
			{
				result = table;
			}
			else
			{
				result = schema + newConnector + table;
			}

			return result;
		}
	}
}

