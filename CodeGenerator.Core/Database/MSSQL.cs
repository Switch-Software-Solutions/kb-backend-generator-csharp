﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CodeGenerator.Core.Database
{
	public class MSSQL
	{
		private string connectionString;

		// Información de la base de datos
		public List<InformationSchemaColumns> Metadata { get; private set; }

		// Mantiene el orden de las dependecias (la tabla referenciada debe de agregarse primero)
		public List<string> DependencieOrder { get; private set; }

		// Mantiene el orden de las dependecias (la tabla referenciada debe de agregarse primero)
		public List<string> Schemas { get; private set; }

		public MSSQL(string connectionString)
		{
			this.connectionString = connectionString;

			Metadata = new List<InformationSchemaColumns>();
			DependencieOrder = new List<string>();
			Schemas = new List<string>();

			GetMetadata();

			GetDependencieOrder();

			GetSchemas();
		}

		private void GetDependencieOrder()
		{
			List<InformationSchemaColumns> dependencies = Metadata.Where(m => m.FK_NAME != "").ToList();

			List<string> roots = Metadata.Where(r => !Metadata.Any(m => m.REFERENCED_TABLE_NAME == r.TABLE_NAME)).Select(r => r.TABLE_SCHEMA + "." + r.TABLE_NAME).Distinct().ToList();

			foreach (string root in roots)
			{
				AddIfNotExist(DependencieOrder, GetDependencieOrderRecursive(dependencies, root));
			}
		}

		public static List<string> tables = new List<string>();

		private List<string> GetDependencieOrderRecursive(List<InformationSchemaColumns> dependencies, string currentTable)
		{
			tables.Add(currentTable);

			List<string> result = new List<string>();

			List<InformationSchemaColumns> currentDependencies = dependencies.Where(t => t.PARENT_TABLE_SCHEMA + "." + t.PARENT_TABLE_NAME == currentTable).ToList();

			if (!currentDependencies.Any())
			{
				result.Add(currentTable);
			}
			else
			{
				foreach (InformationSchemaColumns info in currentDependencies)
				{
					AddIfNotExist(result, GetDependencieOrderRecursive(dependencies, info.REFERENCED_TABLE_SCHEMA + "." + info.REFERENCED_TABLE_NAME));
				}
				AddIfNotExist(result, currentTable);
			}

			return result;
		}

		private void AddIfNotExist(List<string> items, List<string> newItems)
		{
			foreach (string newItem in newItems)
			{
				if (!items.Any(i => i == newItem))
				{
					items.Add(newItem);
				}
			}
		}

		private void AddIfNotExist(List<string> items, string newItem)
		{
			if (!items.Any(i => i == newItem))
			{
				items.Add(newItem);
			}
		}

		// Se obtiene de la base de datos la metadata 
		private void GetMetadata()
		{
			Metadata = DatabaseMetadata.GetMetadata(connectionString);
			//SqlConnection sqlConnection = new SqlConnection(connectionString);
			//SqlCommand command = new SqlCommand();
			//SqlDataReader reader;

			//command.CommandText = System.IO.File.ReadAllText("spS_TablesMetadata.txt");

			//command.Connection = sqlConnection;

			//try
			//{
			//	sqlConnection.Open();

			//	reader = command.ExecuteReader();

			//	InformationSchemaColumns info = null;

			//	while (reader.Read())
			//	{
			//		info = new InformationSchemaColumns(reader);

			//		Metadata.Add(info);
			//	}
			//	reader.Close();
			//	command.Dispose();
			//}
			//catch (Exception)
			//{
			//	throw;
			//}
			//finally
			//{
			//	sqlConnection.Close();
			//}

			////CompleteDependencieOrder();
		}

		// Se obtiene de la base de datos la metadata 
		public List<List<string[]>> GetTableData(string tableName)
		{
			List<List<string[]>> result = new List<List<string[]>>();

			SqlConnection sqlConnection = new SqlConnection(connectionString);
			SqlCommand command = new SqlCommand();
			SqlDataReader reader;

			command.CommandText = "SELECT * FROM [" + tableName + "]";
			command.CommandType = CommandType.Text;
			command.Connection = sqlConnection;

			try
			{
				sqlConnection.Open();

				reader = command.ExecuteReader();

				while (reader.Read())
				{
					List<string[]> row = new List<string[]>();

					for (int i = 0; i < reader.FieldCount; i++)
					{
						row.Add(new string[3] { reader.GetName(i), reader.GetValue(i).ToString(), reader.GetValue(i).GetType().ToString() });
					}

					result.Add(row);
				}
				reader.Close();
				command.Dispose();

				return result;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		// Se maneja la referencia a otra tabla
		private void HandleOrderDependencie(InformationSchemaColumns info)
		{
			string parent_table_full_name = SchemaTable.SchemaTableConcat(info.PARENT_TABLE_SCHEMA, ".", info.PARENT_TABLE_NAME);
			string referenced_table_full_name = SchemaTable.SchemaTableConcat(info.REFERENCED_TABLE_SCHEMA, ".", info.REFERENCED_TABLE_NAME);

			if (info.FK_NAME != string.Empty)
			{
				int indexOfReferencedeTable = DependencieOrder.IndexOf(referenced_table_full_name);
				int indexOfParentTable = DependencieOrder.IndexOf(parent_table_full_name);

				if (indexOfReferencedeTable == -1)
				{
					if (indexOfParentTable == -1)
					{ // NINGUNA EXISTE 
						DependencieOrder.Insert(0, parent_table_full_name);
						DependencieOrder.Insert(0, referenced_table_full_name);
					}
					else
					{ // NO EXISTE REFERENCIADA Y EXISTE PADRE
						DependencieOrder.Insert(indexOfParentTable, referenced_table_full_name);
					}
				}
				else if (indexOfParentTable == -1)
				{
					// NO EXISTE REFERENCIADA Y EXISTE PADRE
					DependencieOrder.Insert(indexOfReferencedeTable + 1, parent_table_full_name);
				}
				else
				{
					// SI EXISTEN LAS DOS YA DEBERÍAN ESTAR BIEN ORDENADAS
					if (indexOfParentTable < indexOfReferencedeTable)
					{
						DependencieOrder.Insert(indexOfReferencedeTable + 1, DependencieOrder[indexOfParentTable]);
						DependencieOrder.RemoveAt(indexOfParentTable);
					}
				}
			}
		}

		// Aquellas tablas que no tienen dependencia y no dependen de ninguna otra tabla se las inserta al comienzo
		private void CompleteDependencieOrder()
		{
			foreach (InformationSchemaColumns info in Metadata)
			{
				if (!DependencieOrder.Contains(info.TABLE_SCHEMA + "." + info.TABLE_NAME))
				{
					DependencieOrder.Insert(0, info.TABLE_SCHEMA + "." + info.TABLE_NAME);
				}
			}
		}

		private void GetSchemas()
		{
			if (Metadata.Any())
			{
				string currentSchemaName = Metadata.First().TABLE_SCHEMA;

				foreach (string schemaName in Metadata.GroupBy(m => m.TABLE_SCHEMA).Select(m => m.Key))
				{
					Schemas.Add(schemaName);
				}
			}
		}

		// Se obtiene la línea para la creación del campo a partir del tipo de datos
		public static string HandledDataType(InformationSchemaColumns info)
		{
			string result = string.Empty;

			result += "    table.";

			switch (info.DATA_TYPE)
			{
				case "nvarchar":
				case "varchar":
					result += "string('" + info.COLUMN_NAME + "'" + (info.CHARACTER_MAXIMUM_LENGTH > 0 ? ", " + info.CHARACTER_MAXIMUM_LENGTH + ")" : ")") + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				case "tinyint":
				case "numeric":
				case "int":
					if (info.NUMERIC_SCALE == 0)
					{
						result += "integer('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					}
					else
					{
						result += "decimal('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					}
					break;
				case "decimal":
					result += "decimal('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				case "float":
					result += "decimal('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				case "date":
				case "datetime":
					result += "date('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				case "datetime2":
					result += "date('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				case "bit":
					result += "boolean('" + info.COLUMN_NAME + "')" + (info.IS_NULLABLE ? ".nullable();" : ".notNullable()");
					break;
				default:
					throw new Exception("DATA_TYPE(" + info.DATA_TYPE + ")no válido");
			}

			return result;
		}
	}
}

