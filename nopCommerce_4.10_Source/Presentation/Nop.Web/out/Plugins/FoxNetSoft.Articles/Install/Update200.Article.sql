IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='AllowComments')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [AllowComments] bit NOT NULL CONSTRAINT DF_FNS_Article_AllowComments DEFAULT 0
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='CommentCount')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [CommentCount] int NOT NULL CONSTRAINT DF_FNS_Article_CommentCount DEFAULT 0
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='Tags')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [Tags] nvarchar(MAX) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleComment]'))
BEGIN
	CREATE TABLE [dbo].[FNS_ArticleComment](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CustomerId] [int] NOT NULL,
		[CommentText] [nvarchar](max) NULL,
		[ArticleId] [int] NOT NULL,
		[CreatedOnUtc] [datetime] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys where name  like '%ArticleComment_Article%' or name  like '%ArticleComment_FNS_Article_Article%')
BEGIN
	ALTER TABLE [dbo].[FNS_ArticleComment]  WITH CHECK ADD  CONSTRAINT [FK_FNS_ArticleComment_FNS_Article_ArticleId] FOREIGN KEY([ArticleId])
	REFERENCES [dbo].[FNS_Article] ([Id])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[FNS_ArticleComment] CHECK CONSTRAINT [FK_FNS_ArticleComment_FNS_Article_ArticleId]

	ALTER TABLE [dbo].[FNS_ArticleComment]  WITH CHECK ADD  CONSTRAINT [FK_FNS_ArticleComment_FNS_Article_ArticleId] FOREIGN KEY([CustomerId])
	REFERENCES [dbo].[Customer] ([Id])
	ON DELETE CASCADE
end
GO
--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleGroup]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [FNS_ArticleGroup]
	ADD [SubjectToAcl] bit NOT NULL CONSTRAINT DF_FNS_ArticleGroup_SubjectToAcl DEFAULT 0
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='SubjectToAcl')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [SubjectToAcl] bit NOT NULL CONSTRAINT DF_FNS_Article_SubjectToAcl DEFAULT 0
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='ShowOnHomePage')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [ShowOnHomePage] bit NOT NULL CONSTRAINT DF_FNS_Article_ShowOnHomePage DEFAULT 0
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleManufacturer]'))
BEGIN
	CREATE TABLE [dbo].[FNS_ArticleManufacturer](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ArticleId] [int] NOT NULL,
		[ManufacturerId] [int] NOT NULL,
		[DisplayOrder] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [dbo].[FNS_ArticleManufacturer]  WITH CHECK ADD  CONSTRAINT [ArticleManufacturer_Article] FOREIGN KEY([ArticleId])
	REFERENCES [dbo].[FNS_Article] ([Id])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[FNS_ArticleManufacturer] CHECK CONSTRAINT [ArticleManufacturer_Article]
END
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleGroup]') and NAME='SubjectToAcl')
BEGIN
	update A
	set SubjectToAcl=case 
		when [acl].EntityId is null then 0
		else 1
		end
	from [FNS_ArticleGroup] A left join 
		(select distinct EntityId FROM [AclRecord] WITH (NOLOCK)
		WHERE EntityName = 'ArticleGroup') as [acl] on A.Id=[acl].EntityId
	where A.SubjectToAcl!=case 
		when [acl].EntityId is null then 0
		else 1
		end
END
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='SubjectToAcl')
BEGIN
	update A
	set SubjectToAcl=case 
		when [acl].EntityId is null then 0
		else 1
		end
	from [FNS_Article] A left join 
		(select distinct EntityId FROM [AclRecord] WITH (NOLOCK)
		WHERE EntityName = 'Article') as [acl] on A.Id=[acl].EntityId
	where A.SubjectToAcl!=case 
		when [acl].EntityId is null then 0
		else 1
		end
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleGroup]') and NAME='LimitedToStores')
BEGIN
	update A
	set LimitedToStores=case 
		when [SM].EntityId is null then 0
		else 1
		end
	from [FNS_ArticleGroup] A left join 
		(select distinct EntityId FROM [StoreMapping] WITH (NOLOCK)
		WHERE EntityName = 'ArticleGroup') as [SM] on A.Id=[SM].EntityId
	where A.LimitedToStores!=case 
		when [SM].EntityId is null then 0
		else 1
		end
END
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='LimitedToStores')
BEGIN
	update A
	set LimitedToStores=case 
		when [SM].EntityId is null then 0
		else 1
		end
	from [FNS_Article] A left join 
		(select distinct EntityId FROM [StoreMapping] WITH (NOLOCK)
		WHERE EntityName = 'Article') as [SM] on A.Id=[SM].EntityId
	where A.LimitedToStores!=case 
		when [SM].EntityId is null then 0
		else 1
		end
END
GO
