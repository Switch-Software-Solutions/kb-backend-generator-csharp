USE [Auth]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 1/19/2021 9:45:07 AM ******/
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
/****** Object:  Table [dbo].[RecoveryKey]    Script Date: 1/19/2021 9:45:08 AM ******/
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
/****** Object:  Table [dbo].[RefreshToken]    Script Date: 1/19/2021 9:45:08 AM ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 1/19/2021 9:45:08 AM ******/
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
SET IDENTITY_INSERT [dbo].[RefreshToken] ON 
GO
INSERT [dbo].[RefreshToken] ([Id], [Token], [Expires], [UserId], [RemoteIpAddress], [Created]) VALUES (1, N'JTj8Zny4V0NrJKrDmjHbRLs3CS46/bMjy9MA9+s+UKc=', CAST(N'2022-01-13T14:09:35.4815577' AS DateTime2), 1, N'::1', CAST(N'2021-01-13T14:09:35.4820864' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[RefreshToken] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([Id], [Created], [Nick], [Email], [Salt], [PasswordHash]) VALUES (1, CAST(N'2020-11-27T00:00:00.0000000' AS DateTime2), N'TestUser', N'user@example.com', N'E1F53135E559C253', N'jhkjcK7iYvSlmFEmPFoeOKXuDzSCE37VdVl3nDU7MQ4=')
GO
SET IDENTITY_INSERT [dbo].[User] OFF
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
