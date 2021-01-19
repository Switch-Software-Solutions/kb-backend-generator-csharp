USE [TestGenerador]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](500) NOT NULL,
	[EventDate] [datetime2](7) NOT NULL,
	[EventType] [nvarchar](50) NOT NULL,
	[TableName] [nvarchar](50) NOT NULL,
	[RecordValues] [nvarchar](max) NOT NULL,
	[TimeStamp] [timestamp] NOT NULL,
	[HashValue] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Auto]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Auto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Marca] [nvarchar](50) NOT NULL,
	[DuenioId] [int] NOT NULL,
 CONSTRAINT [PK_Auto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Persona]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persona](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Edad] [int] NOT NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecoveryKey]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecoveryKey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](max) NULL,
	[Expires] [datetime2](7) NOT NULL,
	[RemoteIpAddress] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RecoveryKey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshToken]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](max) NULL,
	[Expires] [datetime2](7) NOT NULL,
	[UserId] [int] NOT NULL,
	[RemoteIpAddress] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Seguro]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seguro](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cobertura] [int] NOT NULL,
	[Descripcion] [nvarchar](50) NULL,
 CONSTRAINT [PK_Seguro] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Seguro_Auto]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seguro_Auto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SeguroId] [int] NOT NULL,
	[AutoId] [int] NOT NULL,
 CONSTRAINT [PK_Seguro_Auto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 1/19/2021 9:50:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Nick] [nvarchar](30) NOT NULL,
	[Email] [nvarchar](60) NOT NULL,
	[Salt] [nvarchar](50) NULL,
	[PasswordHash] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AuditLog] ON 

INSERT [dbo].[AuditLog] ([Id], [Token], [EventDate], [EventType], [TableName], [RecordValues], [HashValue]) VALUES (1, N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyQGV4YW1wbGUuY29tIiwianRpIjoiYWFmMDg2YmItODllMi00YWQ5LWE2OTctZjNiZGUxNmFjOTgxIiwidXNlcklkIjoiMSIsImV4cCI6MTYxMDk5NTIwNywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIn0.R4yyqLhibdctn7VK17wWG0CHBAxBBXexzayeiy1X0uc', CAST(N'2021-01-18T15:11:02.1279051' AS DateTime2), N'CREATE', N'Persona', N'{
  "Id": 5,
  "Nombre": "string",
  "Edad": 10,
  "AutoCollection": null
}', N'9f72b01c68fc4c8d8a7cd9455095f8070bd85b80e1630e54eaaccccc85a11479')
INSERT [dbo].[AuditLog] ([Id], [Token], [EventDate], [EventType], [TableName], [RecordValues], [HashValue]) VALUES (2, N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyQGV4YW1wbGUuY29tIiwianRpIjoiZWUwYjhlODQtOWMwZC00ZWM4LWI4N2UtNWI2Y2RkNzExODRiIiwidXNlcklkIjoiMSIsImV4cCI6MTYxMTAwMzI3MywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIn0.LDbNXKc7SPbR9xergG8Tg0jRXCN3sBgKw-vtEQ7Pbzs', CAST(N'2021-01-18T17:25:34.7598000' AS DateTime2), N'CREATE', N'Persona', N'{
  "Id": 6,
  "Nombre": "string222",
  "Edad": 22,
  "AutoCollection": null
}', N'f6df56d132fb64e914d47b4822b5458802f38f56976ae26f36512a3383699623')
SET IDENTITY_INSERT [dbo].[AuditLog] OFF
SET IDENTITY_INSERT [dbo].[Auto] ON 

INSERT [dbo].[Auto] ([Id], [Marca], [DuenioId]) VALUES (1, N'Fiat', 1)
INSERT [dbo].[Auto] ([Id], [Marca], [DuenioId]) VALUES (2, N'Ford', 1)
INSERT [dbo].[Auto] ([Id], [Marca], [DuenioId]) VALUES (3, N'Ford', 2)
INSERT [dbo].[Auto] ([Id], [Marca], [DuenioId]) VALUES (5, N'Toyota', 2)
SET IDENTITY_INSERT [dbo].[Auto] OFF
SET IDENTITY_INSERT [dbo].[Persona] ON 

INSERT [dbo].[Persona] ([Id], [Nombre], [Edad]) VALUES (1, N'Ana', 30)
INSERT [dbo].[Persona] ([Id], [Nombre], [Edad]) VALUES (2, N'Pablo', 42)
INSERT [dbo].[Persona] ([Id], [Nombre], [Edad]) VALUES (4, N'Marcelo', 41)
INSERT [dbo].[Persona] ([Id], [Nombre], [Edad]) VALUES (5, N'string', 10)
INSERT [dbo].[Persona] ([Id], [Nombre], [Edad]) VALUES (6, N'string222', 22)
SET IDENTITY_INSERT [dbo].[Persona] OFF
SET IDENTITY_INSERT [dbo].[RefreshToken] ON 

INSERT [dbo].[RefreshToken] ([Id], [Token], [Expires], [UserId], [RemoteIpAddress], [Created]) VALUES (1, N'yq4yKZ1K/1kk/X1dodLzt3JQ83td7sSmeXmkjs9qVTo=', CAST(N'2022-01-18T18:10:08.3182918' AS DateTime2), 1, N'::1', CAST(N'2021-01-18T18:10:08.3192066' AS DateTime2))
SET IDENTITY_INSERT [dbo].[RefreshToken] OFF
SET IDENTITY_INSERT [dbo].[Seguro] ON 

INSERT [dbo].[Seguro] ([Id], [Cobertura], [Descripcion]) VALUES (1, 10, N'Diez por ciento de cobertura')
INSERT [dbo].[Seguro] ([Id], [Cobertura], [Descripcion]) VALUES (2, 10, N'10/100 en seguro de incendio')
SET IDENTITY_INSERT [dbo].[Seguro] OFF
SET IDENTITY_INSERT [dbo].[Seguro_Auto] ON 

INSERT [dbo].[Seguro_Auto] ([Id], [SeguroId], [AutoId]) VALUES (1, 1, 1)
INSERT [dbo].[Seguro_Auto] ([Id], [SeguroId], [AutoId]) VALUES (2, 1, 3)
SET IDENTITY_INSERT [dbo].[Seguro_Auto] OFF
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [Created], [Nick], [Email], [Salt], [PasswordHash]) VALUES (1, CAST(N'2020-11-27T00:00:00.0000000' AS DateTime2), N'TestUser', N'user@example.com', N'E1F53135E559C253', N'jhkjcK7iYvSlmFEmPFoeOKXuDzSCE37VdVl3nDU7MQ4=')
SET IDENTITY_INSERT [dbo].[User] OFF
ALTER TABLE [dbo].[Auto]  WITH CHECK ADD  CONSTRAINT [FK_Auto_Persona] FOREIGN KEY([DuenioId])
REFERENCES [dbo].[Persona] ([Id])
GO
ALTER TABLE [dbo].[Auto] CHECK CONSTRAINT [FK_Auto_Persona]
GO
ALTER TABLE [dbo].[RecoveryKey]  WITH CHECK ADD  CONSTRAINT [FK_RecoveryKey_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RecoveryKey] CHECK CONSTRAINT [FK_RecoveryKey_User_UserId]
GO
ALTER TABLE [dbo].[RefreshToken]  WITH CHECK ADD  CONSTRAINT [FK_RefreshToken_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshToken] CHECK CONSTRAINT [FK_RefreshToken_User_UserId]
GO
ALTER TABLE [dbo].[Seguro_Auto]  WITH CHECK ADD  CONSTRAINT [FK_Seguro_Auto_Auto] FOREIGN KEY([AutoId])
REFERENCES [dbo].[Auto] ([Id])
GO
ALTER TABLE [dbo].[Seguro_Auto] CHECK CONSTRAINT [FK_Seguro_Auto_Auto]
GO
ALTER TABLE [dbo].[Seguro_Auto]  WITH CHECK ADD  CONSTRAINT [FK_Seguro_Auto_Seguro] FOREIGN KEY([SeguroId])
REFERENCES [dbo].[Seguro] ([Id])
GO
ALTER TABLE [dbo].[Seguro_Auto] CHECK CONSTRAINT [FK_Seguro_Auto_Seguro]
GO
