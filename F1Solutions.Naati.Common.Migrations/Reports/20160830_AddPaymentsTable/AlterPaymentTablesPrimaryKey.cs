using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160830_AddPaymentsTable
{
    [NaatiMigration(201609061140)]
    public class AlterPaymentTablesPrimaryKey : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
DECLARE @paymentTableObjectId INT
DECLARE @primaryKeyName NVARCHAR(128)

SET @paymentTableObjectId =
(
	SELECT [object_id]
	FROM [sys].[tables] t
	LEFT JOIN [sys].[schemas] s
		ON s.[schema_id] = t.[schema_id]
	WHERE t.[name] = 'Payment'
	AND s.[name] = 'Internal'
)

SET @primaryKeyName =
(
	SELECT [name] FROM [sys].[key_constraints]
	WHERE [parent_object_id] = @paymentTableObjectId
	AND [type] = 'PK'
	AND [is_system_named] = 1
	AND [name] LIKE 'PK__Payment__%'
)

IF @primaryKeyName IS NOT NULL
BEGIN
	DECLARE @query NVARCHAR(MAX)
	SET @query = N'ALTER TABLE Internal.Payment DROP CONSTRAINT ' + @primaryKeyName
	EXEC sp_executesql @query
	PRINT 'Hello'
END

SET @primaryKeyName =
(
	SELECT [name] FROM [sys].[key_constraints]
	WHERE [parent_object_id] = @paymentTableObjectId
	AND [type] = 'PK'
)

IF @primaryKeyName IS NULL
BEGIN
	ALTER TABLE Internal.Payment
	ADD CONSTRAINT PK_Payment PRIMARY KEY CLUSTERED 
	(
		PaymentId,
		ModifiedDate
	) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	PRINT 'World'
END");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
