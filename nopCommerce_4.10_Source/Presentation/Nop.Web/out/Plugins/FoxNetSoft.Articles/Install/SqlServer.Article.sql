IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetRelatedProductsByArticleId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetRelatedProductsByArticleId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FNS_Article_GetRelatedProductsByArticleId]
(
	@articleId int ,	--Article identifier
	@AllowedCustomerRoleIds	nvarchar(MAX),	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@ProductsPerPage			int, --Products Per Page
	@storeId			int -- Store mapping
)
AS
BEGIN
SET NOCOUNT ON
--Gets Products for only read.

create table #ProductID (Id int,SubjectToAcl bit,LimitedToStores bit, DisplayOrder int)

--filter by customer role IDs (access control list)
SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
CREATE TABLE #FilteredCustomerRoleIds
(
	CustomerRoleId int not null
)


if exists(select G.CategoryId from FNS_ArticleCategory G WITH (NOLOCK) where G.ArticleId=@articleId)
begin
	create table #CategoryId (Id int not null,SubjectToAcl bit,LimitedToStores bit, DisplayOrder int)
	--Category
	;with cte_tree (Id,ParentCategoryId,SubjectToAcl,LimitedToStores,DisplayOrder) as 
	(
		select C.Id,C.ParentCategoryId,C.SubjectToAcl,C.LimitedToStores,G.DisplayOrder
		from Category C,(SELECT TOP 1 WITH TIES G.CategoryId,G.DisplayOrder
				FROM FNS_ArticleCategory G WITH (NOLOCK)
				WHERE G.ArticleId=@articleId
				ORDER BY ROW_NUMBER() OVER(PARTITION BY G.CategoryId ORDER BY G.DisplayOrder)) G
		where C.Id=G.CategoryId
			and C.Deleted=0 and C.Published=1
		union all
		select T.Id,T.ParentCategoryId,T.SubjectToAcl,T.LimitedToStores,C.DisplayOrder
		from cte_tree C,Category T WITH (NOLOCK)
		where C.Id=T.ParentCategoryId and T.Deleted=0 and T.Published=1
	)
	
	insert into #CategoryId (Id,SubjectToAcl,LimitedToStores,DisplayOrder)
	select Id,SubjectToAcl,LimitedToStores,DisplayOrder
	from cte_tree

    --ACL Category
	if exists(select Id from #CategoryId where SubjectToAcl=1)		
	begin
		INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
		SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

		delete #CategoryId
		from #CategoryId C
		where C.SubjectToAcl=1 and not EXISTS (
								SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
									WHERE
										[fcr].CustomerRoleId IN (
											SELECT [acl].CustomerRoleId
											FROM [AclRecord] acl  WITH (NOLOCK)
											WHERE [acl].EntityId = C.Id AND [acl].EntityName = 'Category'
										)
								)
	end
	--StoreMapping Category
	if exists(select Id from #CategoryId where LimitedToStores=1)		
	begin
		delete #CategoryId
		from #CategoryId C
		where C.LimitedToStores=1 and not Exists(select Top 1 Id from StoreMapping SM WITH (NOLOCK)
					where SM.EntityName = 'Category' and SM.StoreId=@storeId and SM.EntityId=C.Id)
	end

	insert into #ProductID (Id,SubjectToAcl,LimitedToStores,DisplayOrder)
	select P.Id,P.SubjectToAcl,P.LimitedToStores,G.DisplayOrder
	from Product_Category_Mapping PM WITH (NOLOCK),Product P WITH (NOLOCK),
		(SELECT TOP 1 WITH TIES G.Id,G.DisplayOrder
				FROM #CategoryId G WITH (NOLOCK)
				ORDER BY ROW_NUMBER() OVER(PARTITION BY G.Id ORDER BY G.DisplayOrder)) G
	where PM.CategoryId=G.Id
		and PM.ProductId=P.Id and P.Published=1 and P.Deleted=0
	drop table #CategoryId
end

--Manufacturer
if exists(select * from FNS_ArticleManufacturer WITH (NOLOCK) where ArticleId=@articleId)
begin
	create table #ManufacturerId (Id int not null,SubjectToAcl bit,LimitedToStores bit, DisplayOrder int)

	insert into #ManufacturerId (Id,SubjectToAcl,LimitedToStores,DisplayOrder)
	select M.Id,M.SubjectToAcl,M.LimitedToStores,AM.DisplayOrder
	from Manufacturer M WITH (NOLOCK), FNS_ArticleManufacturer AM WITH (NOLOCK)
	WHERE AM.ArticleId=@articleId and AM.ManufacturerId=M.Id

    --ACL Manufacturer
	if exists(select Id from #ManufacturerId where SubjectToAcl=1)		
	begin
		INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
		SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

		delete #ManufacturerId
		from #ManufacturerId C
		where C.SubjectToAcl=1 and not EXISTS (
								SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
									WHERE
										[fcr].CustomerRoleId IN (
											SELECT [acl].CustomerRoleId
											FROM [AclRecord] acl  WITH (NOLOCK)
											WHERE [acl].EntityId = C.Id AND [acl].EntityName = 'Manufacturer'
										)
								)
	end
	--StoreMapping Manufacturer
	if exists(select Id from #ManufacturerId where LimitedToStores=1)		
	begin
		delete #ManufacturerId
		from #ManufacturerId C
		where C.LimitedToStores=1 and not Exists(select Top 1 Id from StoreMapping SM WITH (NOLOCK)
					where SM.EntityName = 'Manufacturer' and SM.StoreId=@storeId and SM.EntityId=C.Id)
	end

	insert into #ProductID (Id,SubjectToAcl,LimitedToStores,DisplayOrder)
	select P.Id,P.SubjectToAcl,P.LimitedToStores,G.DisplayOrder
	from Product_Manufacturer_Mapping PM WITH (NOLOCK),Product P WITH (NOLOCK),
		(SELECT TOP 1 WITH TIES G.Id,G.DisplayOrder
				FROM #ManufacturerId G WITH (NOLOCK)
				ORDER BY ROW_NUMBER() OVER(PARTITION BY G.Id ORDER BY G.DisplayOrder)) G
	where PM.ManufacturerId=G.Id
		and PM.ProductId=P.Id and P.Published=1 and P.Deleted=0

	drop table #ManufacturerId
end

--Products
insert into #ProductID (Id,SubjectToAcl,LimitedToStores,DisplayOrder)
select P.Id,P.SubjectToAcl,P.LimitedToStores,PM.DisplayOrder
from FNS_ArticleProduct PM WITH (NOLOCK),Product P WITH (NOLOCK)
where PM.ArticleId=@articleId
	and PM.ProductId=P.Id 
	and PM.ProductId not in (select Id from #ProductID)
	and P.Published=1 and P.Deleted=0
	and (P.AvailableStartDateTimeUtc is null or P.AvailableStartDateTimeUtc<=getdate())
	and (P.AvailableEndDateTimeUtc is null or P.AvailableEndDateTimeUtc>=getdate())

if exists(select * from #ProductID)			
begin
	--ACL Product
	if exists(select Id from #ProductID where SubjectToAcl=1)		
	begin	
		if not exists(select * from #FilteredCustomerRoleIds)
		begin
			INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
			SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	
		end
							
		delete #ProductID
		from #ProductID C
		where C.SubjectToAcl!=0 and not EXISTS (
								SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
									WHERE
										[fcr].CustomerRoleId IN (
											SELECT [acl].CustomerRoleId
											FROM [AclRecord] acl  WITH (NOLOCK)
											WHERE [acl].EntityId = C.Id AND [acl].EntityName = 'Product'
										)
									)
	end
	--StoreMapping Product
	if exists(select Id from #ProductID where LimitedToStores=1)		
	begin								
		delete #ProductID
		from #ProductID C
		where C.LimitedToStores!=0 and not Exists(select Top 1 Id from StoreMapping SM WITH (NOLOCK)
					where SM.EntityName = 'Product' and SM.StoreId=@storeId and SM.EntityId=C.Id)	
	end
end

drop table #FilteredCustomerRoleIds

select TOP (@ProductsPerPage) P.*
from Product P WITH (NOLOCK), #ProductID PP
where P.Id=PP.Id
order By PP.DisplayOrder

drop table #ProductID
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetArticlesByArticleGroupId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetArticlesByArticleGroupId]
GO
/****** Object:  StoredProcedure [FoxNetSoft_Category_GetProductCategoriesByCategoryId]    Script Date: 09/07/2013 12:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FNS_Article_GetArticlesByArticleGroupId]
(
	@articleGroupId int ,	--ArticleGroup identifier
	@AllowedCustomerRoleIds	nvarchar(MAX),	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@PageIndex			int , --Page index
	@PageSize			int, --Page size
	@storeId			int, -- Store mapping
	@OrderBy			int, --0 article date, 5 - creation date, 10 - Name: A to Z, 20 - Name: Z to A, 30 - updation date
	@TotalRecords		int OUTPUT
)
              
AS
BEGIN
SET NOCOUNT ON

--paging
DECLARE @PageLowerBound int
DECLARE @PageUpperBound int
DECLARE @RowsToReturn int
SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
SET @PageLowerBound = @PageSize * @PageIndex
SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	create table #GroupId (Id int not null)

	--ArticleGroup Tree
	;with cte_tree (Id,ParentGroupId) as 
	(
		select C.Id,C.ParentGroupId
		from FNS_ArticleGroup C
		where (C.Id=@articleGroupId or (@articleGroupId=0 and C.ParentGroupId=0))
			and C.Deleted=0 and C.Published=1
		union all
		select T.Id,T.ParentGroupId
		from cte_tree C,FNS_ArticleGroup T WITH (NOLOCK)
		where C.Id=T.ParentGroupId and T.Deleted=0 and T.Published=1
	)
	
	insert into #GroupId (Id)			
	select Id
	from cte_tree

CREATE TABLE #DisplayOrderTmp 
(
	[Id] int IDENTITY (1, 1) NOT NULL,
	[ArticleId] int NOT NULL
)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

	insert into #DisplayOrderTmp (ArticleId)
	select A.Id  
	from FNS_ArticleGroup_Mapping G  WITH (NOLOCK),FNS_Article A  WITH (NOLOCK)
	where G.GroupId in (select Id from #GroupId)
		and G.ArticleId=A.Id
		and A.Deleted=0
		and A.Published=1
		and (A.SubjectToAcl = 0 OR EXISTS (
						SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
							WHERE
								[fcr].CustomerRoleId IN (
									SELECT [acl].CustomerRoleId
									FROM [AclRecord] acl  WITH (NOLOCK)
									WHERE [acl].EntityId = A.Id AND [acl].EntityName = 'Article'
								)
							))
		and (A.LimitedToStores=0 or Exists(select Top 1 Id from StoreMapping SM   WITH (NOLOCK)
			where SM.EntityName = 'Article' and SM.StoreId=@storeId and SM.EntityId=A.Id))
	ORDER BY
		CASE WHEN @OrderBy = 0
		THEN A.DateUtc END desc,
		CASE WHEN @OrderBy = 5
		THEN A.CreatedOnUtc END desc,
		CASE WHEN @OrderBy = 10
		THEN A.Title END,
		CASE WHEN @OrderBy = 20
		THEN A.Title END desc,
		CASE WHEN @OrderBy = 30
		THEN A.UpdatedOnUtc END desc

	drop table #FilteredCustomerRoleIds
	drop table #GroupId
					
	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)
	INSERT INTO #PageIndex (ArticleId)
	SELECT ArticleId
	FROM #DisplayOrderTmp
	GROUP BY ArticleId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return categories
	SELECT TOP (@RowsToReturn)
		C.*
	FROM
		#PageIndex [pi]
		INNER JOIN FNS_Article C  WITH (NOLOCK) on C.Id = [pi].[ArticleId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END

GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetAllArticleReadGroup]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetAllArticleReadGroup]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FNS_Article_GetAllArticleReadGroup]
(
	@AllowedCustomerRoleIds	nvarchar(MAX),	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@storeId int,	--storeId
	@LanguageId int=0, --LanguageId
	@calccount			bit =1, --A value indicating whether to calculate Articles in group
	@calccountsub			bit =1, --A value indicating whether to calculate Articles in subgroup
	@getarticles		bit=0  --A value indicating whether to get Articles to show in Navigation panel
)
AS
BEGIN
SET NOCOUNT ON

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

create table #tmpArticleGroup (Id int not null,Name nvarchar(200),ParentGroupId int,ind nvarchar(max),[Level] int,NumberOfArticles int,DisplayOrder int,SeName nvarchar(400) null)

			;with T as (select *,ROW_NUMBER() over (order by DisplayOrder) as rn from FNS_ArticleGroup)
			,cte_tree (Id,ParentId,ind,[Level]) as 
			(
			select T.Id,T.ParentGroupId,cast(str(T.rn, 8) as nvarchar(max)) as ind,0 as [Level]
			from T WITH (NOLOCK)
			where ParentGroupId=0 and Published=1 and Deleted=0
			union all
			select T.Id,T.ParentGroupId,cast(C.ind + str(T.rn, 8) as nvarchar(max)),1+C.[Level] as [Level]
			from cte_tree C,T WITH (NOLOCK) 
			where C.Id=T.ParentGroupId and T.Published=1 and T.Deleted=0
			)

			insert into #tmpArticleGroup (Id,Name,ParentGroupId,ind,[Level],NumberOfArticles,DisplayOrder)
			select T.Id,T.Name,T.ParentGroupId,C.ind,C.[Level],CAST( 0 as int) as NumberOfArticles,T.DisplayOrder
			from FNS_ArticleGroup T WITH (NOLOCK),cte_tree C
			where T.Id=C.Id
					and (T.SubjectToAcl = 0 OR EXISTS (
									SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
										WHERE
											[fcr].CustomerRoleId IN (
												SELECT [acl].CustomerRoleId
												FROM [AclRecord] acl  WITH (NOLOCK)
												WHERE [acl].EntityId = T.Id AND [acl].EntityName = 'ArticleGroup'
											)
										))
					and (T.LimitedToStores=0 or Exists(select Top 1 Id from StoreMapping sm   WITH (NOLOCK)
						where sm.EntityName = 'ArticleGroup' and sm.StoreId=@storeId and sm.EntityId=T.Id))
			order by C.ind

--calculate articles
if @calccount=1
begin
	update G
	set NumberOfArticles=R.NumberOfArticles
	from #tmpArticleGroup G,
		(select M.GroupId,COUNT(*) as NumberOfArticles
		from FNS_Article A WITH (NOLOCK),FNS_ArticleGroup_Mapping M WITH (NOLOCK)
		where A.Published=1 and A.Deleted=0 and A.Id=M.ArticleId
			and (A.SubjectToAcl = 0 OR EXISTS (
							SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
								WHERE
									[fcr].CustomerRoleId IN (
										SELECT [acl].CustomerRoleId
										FROM [AclRecord] acl  WITH (NOLOCK)
										WHERE [acl].EntityId = A.Id AND [acl].EntityName = 'Article'
									)
								))
			and (A.LimitedToStores=0 or Exists(select Top 1 Id from StoreMapping sm   WITH (NOLOCK)
				where sm.EntityName = 'Article' and sm.StoreId=@storeId and sm.EntityId=A.Id))
		group by M.GroupId) R
	where G.Id=R.GroupId

	if @calccountsub=1
	begin
		create table #tmpArticleGroupCNT (Id int not null,NumberOfArticles int)

		;with cte_tree2 (Id,ParentGroupId,NumberOfArticles) as 
		(
			select T.Id,T.ParentGroupId,T.NumberOfArticles
			from #tmpArticleGroup T
			where T.Id not in (select ParentGroupId from #tmpArticleGroup WITH (NOLOCK))
			union all
			select T.Id,T.ParentGroupId,T.NumberOfArticles+C.NumberOfArticles as NumberOfArticles
			from cte_tree2 C,#tmpArticleGroup T
			where C.ParentGroupId=T.Id
		)
		
		insert into #tmpArticleGroupCNT (Id, NumberOfArticles)	
		select Id, max(NumberOfArticles) as NumberOfArticles
		from cte_tree2
		group by Id
			
		update G
		SET NumberOfArticles=N.NumberOfArticles
		from #tmpArticleGroup G,#tmpArticleGroupCNT N
		where G.Id=N.Id

		drop table #tmpArticleGroupCNT			
	end
end

update G
set SeName=ltrim(rtrim(Substring(US.Slug,1,400)))
from #tmpArticleGroup G, (select TOP 1 WITH TIES U.EntityId,U.Slug
	from UrlRecord U WITH (NOLOCK)
	where U.EntityId in (select Id from #tmpArticleGroup) 
		and U.EntityName='ArticleGroup' and U.LanguageId=0 and U.IsActive=1
	ORDER BY ROW_NUMBER() OVER(PARTITION BY U.EntityId ORDER BY U.Id DESC)) US
where G.Id=US.EntityId

if @LanguageId>0
begin
	update G
	set Name=ISNULL(L.LocaleValue,G.name)
	from #tmpArticleGroup G left join LocalizedProperty L on G.Id=L.EntityId 
		and L.LocaleKeyGroup='ArticleGroup' 
		and L.LocaleKey='Name' 
		and L.LanguageId=@LanguageId

	update G
	set SeName=ltrim(rtrim(Substring(US.Slug,1,400)))
	from #tmpArticleGroup G, (select TOP 1 WITH TIES U.EntityId,U.Slug
		from UrlRecord U WITH (NOLOCK)
		where U.EntityId in (select Id from #tmpArticleGroup) 
			and U.EntityName='ArticleGroup' and U.LanguageId=@LanguageId and U.IsActive=1
			and ltrim(rtrim(U.Slug))!=''
		ORDER BY ROW_NUMBER() OVER(PARTITION BY U.EntityId ORDER BY U.Id DESC)) US
	where G.Id=US.EntityId
end	

create table #tmpArticles (Id int not null,Name nvarchar(200),ParentGroupId int,DisplayOrder int,SeName nvarchar(400) null)
if @getarticles=1
begin
	insert into #tmpArticles (Id,Name,ParentGroupId,DisplayOrder)
	select A.Id,A.Title as Name,M.GroupId,M.DisplayOrder
	from #tmpArticleGroup G,FNS_Article A WITH (NOLOCK),FNS_ArticleGroup_Mapping M WITH (NOLOCK)
	where A.Published=1 and A.Deleted=0 and A.Id=M.ArticleId and M.GroupId=G.Id
		and (A.SubjectToAcl = 0 OR EXISTS (
							SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
								WHERE
									[fcr].CustomerRoleId IN (
										SELECT [acl].CustomerRoleId
										FROM [AclRecord] acl  WITH (NOLOCK)
										WHERE [acl].EntityId = A.Id AND [acl].EntityName = 'Article'
									)
							))
		and (A.LimitedToStores=0 or Exists(select Top 1 Id from StoreMapping sm   WITH (NOLOCK)
				where sm.EntityName = 'Article' and sm.StoreId=@storeId and sm.EntityId=A.Id))
	order by M.GroupId,M.DisplayOrder
					
	update C
	set SeName=ltrim(rtrim(Substring(US.Slug,1,400)))
	from #tmpArticles C, (select TOP 1 WITH TIES U.EntityId,U.Slug
		from UrlRecord U WITH (NOLOCK)
		where U.EntityId in (select Id from #tmpArticles) 
			and U.EntityName='Article' and U.LanguageId=0 and U.IsActive=1
		ORDER BY ROW_NUMBER() OVER(PARTITION BY U.EntityId ORDER BY U.Id DESC)) US
	where C.Id=US.EntityId

	if @LanguageId>0
	begin
		update G
		set Name=ISNULL(L.LocaleValue,G.name)
		from #tmpArticles G left join LocalizedProperty L on G.Id=L.EntityId 
			and L.LocaleKeyGroup='Article' 
			and L.LocaleKey='Title' 
			and L.LanguageId=@LanguageId

		update C
		set SeName=ltrim(rtrim(Substring(US.Slug,1,400)))
		from #tmpArticles C, (select TOP 1 WITH TIES U.EntityId,U.Slug
			from UrlRecord U WITH (NOLOCK)
			where U.EntityId in (select Id from #tmpArticles) 
				and U.EntityName='Article' and U.LanguageId=@LanguageId and U.IsActive=1
				and ltrim(rtrim(U.Slug))!=''
			ORDER BY ROW_NUMBER() OVER(PARTITION BY U.EntityId ORDER BY U.Id DESC)) US
		where C.Id=US.EntityId
	end	
end

select G.Id,G.Name,G.ParentGroupId,G.NumberOfArticles,convert(bit,1) as IsGroup,G.SeName,G.[Level], G.DisplayOrder
from #tmpArticleGroup G
union all
select A.Id,A.Name,A.ParentGroupId,0 as NumberOfArticles,convert(bit,0) as IsGroup,A.SeName, 999999 as [Level], A.DisplayOrder
from #tmpArticles A
order by [Level], DisplayOrder

drop table #tmpArticleGroup	
drop table #tmpArticles	
drop table #FilteredCustomerRoleIds
end

GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetRelatedArticlesByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetRelatedArticlesByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FNS_Article_GetRelatedArticlesByProductId]
(
	@productId int ,	--Product identifier
	@ArticlesPerPage			int, --Articles Per Page
	@storeId			int -- Store mapping
)
AS
BEGIN
SET NOCOUNT ON
--Gets Articles for only read.

create table #ArticleId (Id int,LimitedToStores bit)

if exists(select G.CategoryId from Product_Category_Mapping G WITH (NOLOCK) where G.ProductId=@productId)
begin
	create table #CategoryId (Id int not null,LimitedToStores bit)

	--Category
	;with cte_tree (Id,ParentCategoryId,LimitedToStores) as 
	(
		select C.Id,C.ParentCategoryId,C.LimitedToStores
		from Category C
		where C.Id in (select G.CategoryId from Product_Category_Mapping G WITH (NOLOCK) where G.ProductId=@productId)
			and C.Deleted=0 and C.Published=1
		union all
		select T.Id,T.ParentCategoryId,T.LimitedToStores
		from cte_tree C,Category T WITH (NOLOCK)
		where C.Id=T.ParentCategoryId and T.Deleted=0 and T.Published=1
	)
	
	insert into #CategoryId (Id,LimitedToStores)
	select Id,LimitedToStores
	from cte_tree

	insert into #ArticleId (Id,LimitedToStores)
	select A.Id,A.LimitedToStores
	from FNS_ArticleCategory AG WITH (NOLOCK),FNS_Article A WITH (NOLOCK)
	where AG.CategoryId in (select C.Id from #CategoryId C)
		and AG.ArticleId=A.Id and A.Published=1 and A.Deleted=0
	drop table #CategoryId
end

--Manufacturers
insert into #ArticleId (Id,LimitedToStores)
select A.Id,A.LimitedToStores
from FNS_ArticleManufacturer AM WITH (NOLOCK),FNS_Article A WITH (NOLOCK)
where AM.ManufacturerId in (select PM.ManufacturerId from Product_Manufacturer_Mapping PM WITH (NOLOCK) where PM.ProductId=@productId)
	and AM.ArticleId=A.Id and A.Published=1 and A.Deleted=0

--Articles
insert into #ArticleId (Id,LimitedToStores)
select A.Id,A.LimitedToStores
from FNS_ArticleProduct AP WITH (NOLOCK),FNS_Article A WITH (NOLOCK)
where AP.ProductId=@productId
	and AP.ArticleId=A.Id and A.Published=1 and A.Deleted=0
	and AP.ArticleId not in (select Id from #ArticleId)

if exists(select * from #ArticleId)			
begin
	--StoreMapping Product
	if exists(select Id from #ArticleId where LimitedToStores=1)		
	begin								
		delete #ArticleId
		from #ArticleId C
		where C.LimitedToStores!=0 and not Exists(select Top 1 Id from StoreMapping sm WITH (NOLOCK)
					where sm.EntityName = 'Article' and sm.StoreId=@storeId and sm.EntityId=C.Id)	
	end
end

select TOP (@ArticlesPerPage) P.*
from FNS_Article P WITH (NOLOCK)
where P.Id in (select Id from #ArticleId)
order by DateUtc desc
drop table #ArticleId
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_SearchArticle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_SearchArticle]
GO
/****** Object:  StoredProcedure [ProductLoadAllPaged]    Script Date: 11/13/2013 09:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FNS_Article_SearchArticle]
(
	@articleGroupIds nvarchar(MAX) = null,	--a list of ArticleGroup identifiers IDs (comma-separated list). e.g. 1,2,3
	@PageIndex			int , --Page index
	@PageSize			int, --Page size
	@AllowedCustomerRoleIds	nvarchar(MAX),	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@storeId			int, -- Store mapping
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Title: A to Z, 6 - Title: Z to A, 15 - creation date
	@ShowHidden			bit = 0,
	@TotalRecords		int OUTPUT
)
AS
BEGIN
SET NOCOUNT ON
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id('[FNS_Article]'))
	begin
		set @UseFullTextSearch=0
		set @FullTextMode=0
	end
		
	/* Articles that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ArticleId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, LEN(@Keywords) - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, LEN(@Keywords)) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--Article title
		SET @sql = '
		INSERT INTO #KeywordProducts ([ArticleId])
		SELECT p.Id
		FROM FNS_Article p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[title], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[title]) > 0 '

		--localized Article title
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Article''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Title'' '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
	

		IF @SearchDescriptions = 1
		BEGIN
			--article Body
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM FNS_Article p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[Body], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[Body]) > 0 '
			

			--localized product short description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Article''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Body'''
				IF @UseFullTextSearch = 1
					SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
				ELSE
					SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '					
				END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000)', @Keywords
	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @articleGroupIds = isnull(@articleGroupIds, '')	
	CREATE TABLE #FilteredArticleGroupIds
	(
		articleGroupId int not null
	)
	INSERT INTO #FilteredArticleGroupIds (articleGroupId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@articleGroupIds, ',')	
	DECLARE @articleGroupIdsCount int	
	SET @articleGroupIdsCount = (SELECT COUNT(1) FROM #FilteredArticleGroupIds)
	--print @articleGroupIdsCount

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)

	SET @sql = '
	INSERT INTO #DisplayOrderTmp ([ArticleId])
	SELECT p.Id
	FROM
		FNS_Article p with (NOLOCK)'
	
	IF @articleGroupIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN FNS_ArticleGroup_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ArticleId'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ArticleId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by ArticleGroup
	IF @articleGroupIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.GroupId IN (SELECT articleGroupId FROM #FilteredArticleGroupIds)'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Published = 1'
	END

	--show hidden and ACL
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Article''
				)
			))'
	END
		
	--show hidden and filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Article'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE IF @OrderBy = 10 /* Title: A to Z */
		SET @sql_orderby = ' p.[Title] ASC'
	ELSE IF @OrderBy = 20 /* Name: Z to A  */
		SET @sql_orderby = ' p.[Title] DESC'
	ELSE IF @OrderBy = 30 /* updation date */
		SET @sql_orderby = ' p.[UpdatedOnUtc] DESC'
	ELSE /* default sorting, 0 (article date) */
	BEGIN
		SET @sql_orderby = ' p.[DateUtc] DESC'
		--Title
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Title] ASC'
	END

	SET @sql = @sql + '	ORDER BY' + @sql_orderby
	
	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #FilteredArticleGroupIds

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ArticleId])
	SELECT ArticleId
	FROM #DisplayOrderTmp
	GROUP BY ArticleId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN FNS_Article p with (NOLOCK) on p.Id = [pi].[ArticleId] 
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FNS_Article_GetArticlesDisplayedOnHomePage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FNS_Article_GetArticlesDisplayedOnHomePage]
GO
CREATE PROCEDURE [dbo].[FNS_Article_GetArticlesDisplayedOnHomePage]
(
	@AllowedCustomerRoleIds	nvarchar(MAX),	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
	@PageIndex			int , --Page index
	@PageSize			int, --Page size
	@storeId			int, -- Store mapping
	@OrderBy			int, --0 article date, 5 - creation date, 10 - Name: A to Z, 20 - Name: Z to A, 30 - updation date
	@TotalRecords		int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')	

	insert into #DisplayOrderTmp (ArticleId)
	select A.Id  
	from FNS_Article A  WITH (NOLOCK)
	where A.Deleted=0
		and A.ShowOnHomePage=1
		and A.Published=1
		and (A.SubjectToAcl = 0 OR EXISTS (
						SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
							WHERE
								[fcr].CustomerRoleId IN (
									SELECT [acl].CustomerRoleId
									FROM [AclRecord] acl  WITH (NOLOCK)
									WHERE [acl].EntityId = A.Id AND [acl].EntityName = 'Article'
								)
							))
		and (A.LimitedToStores=0 or Exists(select Top 1 Id from StoreMapping SM   WITH (NOLOCK)
			where SM.EntityName = 'Article' and SM.StoreId=@storeId and SM.EntityId=A.Id))
	ORDER BY
		CASE WHEN @OrderBy = 0
		THEN A.DateUtc END desc,
		CASE WHEN @OrderBy = 5
		THEN A.CreatedOnUtc END desc,
		CASE WHEN @OrderBy = 10
		THEN A.Title END,
		CASE WHEN @OrderBy = 20
		THEN A.Title END desc,
		CASE WHEN @OrderBy = 30
		THEN A.UpdatedOnUtc END desc

	drop table #FilteredCustomerRoleIds
					
	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ArticleId] int NOT NULL
	)
	INSERT INTO #PageIndex (ArticleId)
	SELECT ArticleId
	FROM #DisplayOrderTmp
	GROUP BY ArticleId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return categories
	SELECT TOP (@RowsToReturn)
		C.*
	FROM
		#PageIndex [pi]
		INNER JOIN FNS_Article C  WITH (NOLOCK) on C.Id = [pi].[ArticleId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO


