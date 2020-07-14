1- Modificar la connection string
	Program -> const string CONNECTION_STRING = @"Password=Nc123123;Persist Security Info=True;User ID=sa;Initial Catalog=sw.bancos;Data Source=localhost\SQLEXPRESS";

2- Modificar la carpeta raiz donde se guardarán los archivos generados
	Program -> Main -> Commons.ROOT_PATH = @"D:\Temp\MSSQL2KNEX\visus2\";

3.1- Asegurarse de que exista el siguiente sp en la base de datos 

USE [NOMBRE DE LA BASE DE DATOS]
GO
/****** Object:  StoredProcedure [dbo].[spS_TablesMetadata]    Script Date: 1/30/2019 11:06:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spS_TablesMetadata]

AS 

DECLARE @INFO TABLE
(
	TABLE_CATALOG NVARCHAR(100),
	TABLE_SCHEMA NVARCHAR(100),
	TABLE_NAME NVARCHAR(100),
	COLUMN_NAME NVARCHAR(100),
	IS_NULLABLE NVARCHAR(100),
	DATA_TYPE NVARCHAR(100),
	CHARACTER_MAXIMUM_LENGTH INT,
	NUMERIC_SCALE INT
)

DECLARE @DEPENDENCIES TABLE
(
	FK_NAME NVARCHAR(100),
	PARENT_TABLE_SCHEMA NVARCHAR(100),
	PARENT_TABLE_NAME NVARCHAR(100),
	PARENT_TABLE_COLUMN NVARCHAR(100),
	REFERENCED_TABLE_SCHEMA NVARCHAR(100),
	REFERENCED_TABLE_NAME NVARCHAR(100),
	REFERENCED_TABLE_COLUMN NVARCHAR(100)
)

INSERT INTO @INFO
SELECT 
		TABLE_CATALOG, 
        TABLE_SCHEMA,
        TABLE_NAME,
        COLUMN_NAME ,
        IS_NULLABLE ,
        DATA_TYPE ,
        CHARACTER_MAXIMUM_LENGTH ,
        NUMERIC_SCALE
			
FROM
					INFORMATION_SCHEMA.COLUMNS C
WHERE
					C.TABLE_NAME NOT LIKE 'sysdiagrams%'

INSERT INTO @DEPENDENCIES
SELECT
					fk.name 'FK_NAME',
					RC.CONSTRAINT_SCHEMA AS 'PARENT_TABLE.CATALOG',
					tp.name 'PARENT_TABLE_NAME',
					cp.name 'PARENT_TABLE_COLUMN', 
					RC.UNIQUE_CONSTRAINT_SCHEMA AS 'REFERENCED_TABLE.CATALOG',
					tr.name 'REFERENCED_TABLE_NAME',
					cr.name 'REFERENCED_TABLE_COLUMN'
FROM 
					sys.foreign_keys fk
			INNER JOIN 
					sys.tables tp ON fk.parent_object_id = tp.object_id
			INNER JOIN 
					sys.tables tr ON fk.referenced_object_id = tr.object_id
			INNER JOIN 
					sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
			INNER JOIN 
					sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
			INNER JOIN 
					sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
			INNER JOIN
					INFORMATION_SCHEMA.TABLE_CONSTRAINTS CST ON CST.CONSTRAINT_NAME = FK.name
			INNER JOIN
					INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON CST.CONSTRAINT_NAME = RC.CONSTRAINT_NAME

SELECT 
					*
FROM
					@INFO I
			LEFT JOIN
					@DEPENDENCIES D ON I.TABLE_NAME = D.PARENT_TABLE_NAME
				AND
					I.COLUMN_NAME = D.PARENT_TABLE_COLUMN

3.2- Asegurarse que existe la tabla

USE [NOMBRE DE LA BASE DE DATOS]
GO

/****** Object:  Table [dbo].[logs]    Script Date: 1/30/2019 6:26:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[logs](
	[id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[usuario] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_logs] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



4- Ejecutar el programa
	- Esto generará los archivos necesarios para los ABM de las tablas existentes en la base de datos

5- Crear la carptea del proyecto
	- Ej.	mkdir D:\Proyectos\MiProyecto

6- Crear la api del proyecto
	- Ej.	cd  D:\Proyectos\MiProyecto
			express MiProyecto-api
			cd D:\Proyectos\MiProyecto\MiProyecto-api
			npm install
	- Para lanzar la api: npm start en la carpeta de la api

7- Crear la web del proyecto
	- Ej.	cd  D:\Proyectos\MiProyecto
			vue create MiProyecto-web
			cd D:\Proyectos\MiProyecto\MiProyecto-web
	- Para lanzar la web: npm run serve en la carpeta de la web

8- Copiar los archivos generados en las respectivas carpetas
	- api
		- business
		- data-access
		- db
		- routes
	- web
		- src

9- Install en api
	- dotenv
		- npm install dotenv
	- knex
		- npm install knex
		- npm install mssql@4.1.0 --save 
			- Se instala la versión 4.1.0 porque es la compatible con knex
	- cors
		- npm install cors	
	- nodemon
		- npm install nodemon
		- En el archivo package-json cambiar el valor de start por:
			"start": "set DEBUG=MiProyecto:* & set DEBUG=knex:tx & nodemon ./bin/www"

10- Install en web
	- axios
		npm install --save axios
	- bootstrap-vue
		npm install --save bootstrap-vue
	nue-router
		npm install --save vue-router
	vuelidate
		npm install --save vuelidate /lib/validators vuelidate

