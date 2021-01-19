using System.Collections.Generic;
using System.Linq;
using T4CodeGenerator.MSSQL2CSHARP.Database;

namespace T4CodeGenerator.MSSQL2CSHARP
{
    public static class Configuration
    {
        public static string ProjectName { get; set; }
        public static string BasePath { get; set; }
        public static string DatabasePassword { get; set; }
        public static string DatabaseUserName { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseServer { get; set; }
        public static string DataTypeMapping { get; set; }
        public static bool SaveOnDisk { get; set; }

        private static MSSQL _MSSQL;
        public static MSSQL MSSQL
        {
            get
            {
                if (_MSSQL == null) 
                {
                    string CONNECTION_STRING = 
                        $@"Password={DatabasePassword};
                        Persist Security Info=True;
                        User Id={DatabaseUserName};
                        Initial Catalog={DatabaseName};
                        Data Source={DatabaseServer}";

                    _MSSQL = new MSSQL(CONNECTION_STRING, DataTypeMapping);
                }
                return _MSSQL;
            }
        }

        public static string TableName { get; set; }
        public static string ParentTableName { get; set; }
        public static Dictionary<string, List<string>> RelatedEntities { get; set; }

        public static List<string> PurgeExcludedClasses(List<string> classesList)
        {
            return classesList.Except(ExcludedClasses).ToList();
        }

        public static List<string> ExcludedClasses = new List<string>()
        {
            "__EFMigrationsHistory",
            "RecoveryKey",
            "RefreshToken",
            "AuditLog"
        };
    }
}
