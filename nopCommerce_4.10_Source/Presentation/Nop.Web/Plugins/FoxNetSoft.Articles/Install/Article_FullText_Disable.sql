	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[FNS_Article]''))
		DROP FULLTEXT INDEX ON [FNS_Article]
	')