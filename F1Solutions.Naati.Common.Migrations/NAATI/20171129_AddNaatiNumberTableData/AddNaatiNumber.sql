DECLARE @PersonNaatiNumber INT
DECLARE @InstitutionNaatiNumber INT
DECLARE @MinPersonNaatiNumber INT

DECLARE @MinInstitutionNaatiNumber INT

SET @MinPersonNaatiNumber = 200000
SET @MinInstitutionNaatiNumber = 950000

SELECT @PersonNaatiNumber = MAX(e.NaatiNumber) from tblPerson p inner join tblentity e on e.entityId = p.EntityId
SELECT @InstitutionNaatiNumber = MAX(e.NaatiNumber) from tblInstitution i inner join tblentity e on e.entityId = i.EntityId

IF @PersonNaatiNumber < @MinPersonNaatiNumber OR @PersonNaatiNumber IS NULL
BEGIN
SET @PersonNaatiNumber = @MinPersonNaatiNumber
END

IF @InstitutionNaatiNumber < @MinInstitutionNaatiNumber OR  @InstitutionNaatiNumber IS NULL
BEGIN
SET @InstitutionNaatiNumber = @MinInstitutionNaatiNumber
END


INSERT INTO [dbo].[tblTableData]
           ([TableName]
           ,[NextKey]
           ,[AllocateQuantity]
           ,[DefaultId])
     VALUES
           ('PersonNaatiNumber'
           ,@PersonNaatiNumber
           ,5
           ,0)

INSERT INTO [dbo].[tblTableData]
           ([TableName]
           ,[NextKey]
           ,[AllocateQuantity]
           ,[DefaultId])
     VALUES
           ('InstitutionNaatiNumber'
           ,@InstitutionNaatiNumber
           ,5
           ,0)