IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'FT_Audit')
	CREATE FULLTEXT CATALOG FT_Audit;

IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes where object_id = object_id('tblAuditLog'))
	CREATE FULLTEXT INDEX ON tblAuditLog(ChangeDetail) KEY INDEX PK_tblAuditLog ON FT_Audit;