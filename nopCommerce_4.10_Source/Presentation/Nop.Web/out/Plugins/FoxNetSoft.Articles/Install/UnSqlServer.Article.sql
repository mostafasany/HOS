IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetRelatedProductsByArticleId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetRelatedProductsByArticleId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetArticlesByArticleGroupId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetArticlesByArticleGroupId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetAllArticleReadGroup]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetAllArticleReadGroup]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetRelatedArticlesByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetRelatedArticlesByProductId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_SearchArticle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_SearchArticle]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetArticlesDisplayedOnHomePage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetArticlesDisplayedOnHomePage]
GO
