using System;

namespace MSSQL2KNEX
{
	class Program
    {
		static void Main(string[] args)
		{
			const string CONNECTION_STRING = @"Password=Switch09#bp;Persist Security Info=True;User ID=usr_BHC_SafeTemp;Initial Catalog=BHC_SafeTemp;Data Source=localhost\SQLEXPRESS";
			const string ROOT_PATH = @"D:\Temp\PRUEBASMSSQLTOKNEX";

			CodeGenerator.Core.Database.MSSQL _MSSQL = new CodeGenerator.Core.Database.MSSQL(CONNECTION_STRING);

			CodeGenerator.Core.Engine e;

			string proyectName = "BHC_SafeTemp";

			e = new CodeGenerator.Core.Engine(@"./Templates/Front/ViewDelete.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Front/ViewCreate.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Front/ViewDelete.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Front/ViewList.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Front/ViewUpdate.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Back/Business.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Back/DataAccess.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Back/Route.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			e = new CodeGenerator.Core.Engine(@"./Templates/Database/Migrations.xml", @"./Types/MSSQL2KNEX.xml", proyectName);
			Console.WriteLine(e.Generate(_MSSQL.Metadata));

			Console.ReadKey();
		}
	}
}
