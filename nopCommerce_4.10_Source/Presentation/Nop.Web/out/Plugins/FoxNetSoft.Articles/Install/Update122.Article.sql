--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleGroup]') and NAME='Description')
BEGIN
	ALTER TABLE [FNS_ArticleGroup]
	ADD [Description] nvarchar(MAX) NULL
END
GO
--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='DateUtc')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [DateUtc] datetime NOT NULL CONSTRAINT DF_FNS_Article_DateUtc DEFAULT getdate()
	--update FNS_Article set DateUtc=UpdatedOnUtc
END
GO
--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleGroup]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [FNS_ArticleGroup]
	ADD [LimitedToStores] bit NOT NULL CONSTRAINT DF_FNS_ArticleGroup_LimitedToStores DEFAULT 0
END
GO
--add a new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_Article]') and NAME='ArticleTemplateId')
BEGIN
	ALTER TABLE [FNS_Article]
	ADD [ArticleTemplateId] int NOT NULL CONSTRAINT DF_FNS_Article_ArticleTemplateId DEFAULT 1
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[FNS_ArticleTemplate]'))
BEGIN
	CREATE TABLE [dbo].[FNS_ArticleTemplate](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](400) NOT NULL,
		[ViewPath] [nvarchar](400) NOT NULL,
		[DisplayOrder] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	insert into FNS_ArticleTemplate (Name,ViewPath,DisplayOrder)
	values ('Default','ArticleTemplate.Default',1)
end
GO
