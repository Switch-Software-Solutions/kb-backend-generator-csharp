using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Database
{
    public static class DatabaseMetadata
    {
        // Se obtiene de la base de datos la metadata 
        public static List<InformationSchemaColumns> GetMetadata(string connectionString)
        {
            List<InformationSchemaColumns> result = new List<InformationSchemaColumns>();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            SqlDataReader reader;

            command.CommandText = GetTablesMetadataStoredPrecedure();

            command.Connection = sqlConnection;

            try
            {
                sqlConnection.Open();

                reader = command.ExecuteReader();

                InformationSchemaColumns info = null;

                while (reader.Read())
                {
                    info = new InformationSchemaColumns(reader);

                    result.Add(info);
                }
                reader.Close();
                command.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            //CompleteDependencieOrder();

            return result;
        }

        private static string GetTablesMetadataStoredPrecedure()
        {
            string result = string.Empty;

            result += "DECLARE @INFO TABLE ";
            result += "( ";
            result += "	TABLE_CATALOG NVARCHAR(100), ";
            result += "	TABLE_SCHEMA NVARCHAR(100), ";
            result += "	TABLE_NAME NVARCHAR(100), ";
            result += "	COLUMN_NAME NVARCHAR(100), ";
            result += "	IS_NULLABLE NVARCHAR(100), ";
            result += "	DATA_TYPE NVARCHAR(100), ";
            result += "	CHARACTER_MAXIMUM_LENGTH INT, ";
            result += "	NUMERIC_SCALE INT ";
            result += ") ";
            result += " ";
            result += "DECLARE @DEPENDENCIES TABLE ";
            result += "( ";
            result += "	FK_NAME NVARCHAR(100), ";
            result += "	PARENT_TABLE_SCHEMA NVARCHAR(100), ";
            result += "	PARENT_TABLE_NAME NVARCHAR(100), ";
            result += "	PARENT_TABLE_COLUMN NVARCHAR(100), ";
            result += "	REFERENCED_TABLE_SCHEMA NVARCHAR(100), ";
            result += "	REFERENCED_TABLE_NAME NVARCHAR(100), ";
            result += "	REFERENCED_TABLE_COLUMN NVARCHAR(100) ";
            result += ") ";
            result += " ";
            result += "DECLARE @PRIMARY_KEYS TABLE ";
            result += "( ";
            result += "	TABLE_NAME NVARCHAR(100), ";
            result += "	COLUMN_NAME NVARCHAR(100), ";
            result += "	IS_IDENTITY NVARCHAR(3) ";
            result += ") ";
            result += " ";
            result += "INSERT INTO @INFO ";
            result += "SELECT  ";
            result += "		TABLE_CATALOG,  ";
            result += "        TABLE_SCHEMA, ";
            result += "        TABLE_NAME, ";
            result += "        COLUMN_NAME , ";
            result += "        IS_NULLABLE , ";
            result += "        DATA_TYPE , ";
            result += "        CHARACTER_MAXIMUM_LENGTH , ";
            result += "        NUMERIC_SCALE ";
            result += "			 ";
            result += "FROM ";
            result += "					INFORMATION_SCHEMA.COLUMNS C ";
            result += "WHERE ";
            result += "					C.TABLE_NAME NOT LIKE 'sysdiagrams%' ";
            result += " ";
            result += "INSERT INTO @DEPENDENCIES ";
            result += "SELECT ";
            result += "					fk.name 'FK_NAME', ";
            result += "					RC.CONSTRAINT_SCHEMA AS 'PARENT_TABLE.CATALOG', ";
            result += "					tp.name 'PARENT_TABLE_NAME', ";
            result += "					cp.name 'PARENT_TABLE_COLUMN',  ";
            result += "					RC.UNIQUE_CONSTRAINT_SCHEMA AS 'REFERENCED_TABLE.CATALOG', ";
            result += "					tr.name 'REFERENCED_TABLE_NAME', ";
            result += "					cr.name 'REFERENCED_TABLE_COLUMN' ";
            result += "FROM  ";
            result += "					sys.foreign_keys fk ";
            result += "			INNER JOIN  ";
            result += "					sys.tables tp ON fk.parent_object_id = tp.object_id ";
            result += "			INNER JOIN  ";
            result += "					sys.tables tr ON fk.referenced_object_id = tr.object_id ";
            result += "			INNER JOIN  ";
            result += "					sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id ";
            result += "			INNER JOIN  ";
            result += "					sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id ";
            result += "			INNER JOIN  ";
            result += "					sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id ";
            result += "			INNER JOIN ";
            result += "					INFORMATION_SCHEMA.TABLE_CONSTRAINTS CST ON CST.CONSTRAINT_NAME = FK.name ";
            result += "			INNER JOIN ";
            result += "					INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON CST.CONSTRAINT_NAME = RC.CONSTRAINT_NAME ";
            result += " ";
            result += "INSERT INTO @PRIMARY_KEYS ";
            result += "SELECT  ";
            result += "				T.TABLE_NAME, ";
            result += "				C.Column_Name,  ";
            result += "				CASE WHEN IC.[NAME] IS NULL THEN 'NO' ELSE 'YES' END IS_IDENTITY ";
            result += "FROM  ";
            result += "				INFORMATION_SCHEMA.TABLE_CONSTRAINTS T ";
            result += "		INNER JOIN ";
            result += "				INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ";
            result += "			ON   ";
            result += "				C.Constraint_Name = T.Constraint_Name ";
            result += "			AND ";
            result += "				C.Table_Name = T.Table_Name ";
            result += "			AND ";
            result += "				Constraint_Type = 'PRIMARY KEY' ";
            result += "		LEFT JOIN ";
            result += "				SYS.IDENTITY_COLUMNS IC ";
            result += "			ON  ";
            result += "				C.TABLE_NAME =  OBJECT_NAME(IC.OBJECT_ID) ";
            result += "			AND ";
            result += "				T.TABLE_NAME =  OBJECT_NAME(IC.OBJECT_ID) ";
            result += "			AND ";
            result += "				C.COLUMN_NAME = IC.[NAME] ";
            result += " ";
            result += " ";
            result += "SELECT  ";
            result += "					I.*, ";
            result += "					D.*, ";
            result += "					CASE WHEN PK.COLUMN_NAME IS NULL THEN 'NO' ELSE 'YES' END IS_PRIMARY_KEY, ";
            result += "					CASE WHEN PK.IS_IDENTITY IS NULL THEN 'NO' ELSE 'YES' END IS_IDENTITY ";
            result += "FROM ";
            result += "					@INFO I ";
            result += "			LEFT JOIN ";
            result += "					@DEPENDENCIES D ON I.TABLE_NAME = D.PARENT_TABLE_NAME ";
            result += "				AND ";
            result += "					I.COLUMN_NAME = D.PARENT_TABLE_COLUMN ";
            result += "			LEFT JOIN ";
            result += "					@PRIMARY_KEYS PK ";
            result += "				ON ";
            result += "					I.TABLE_NAME = PK.TABLE_NAME ";
            result += "				AND ";
            result += "					I.COLUMN_NAME = PK.COLUMN_NAME ";

            return result;
        }
    }
}
