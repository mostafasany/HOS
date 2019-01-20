	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[FNS_Article]''))
		CREATE FULLTEXT INDEX ON [FNS_Article]([Title], [Body])
		KEY INDEX [' + [nop_getprimarykey_indexname] ('FNS_Article') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)