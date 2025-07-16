ALTER PROCEDURE [dbo].[NH_EntityInsert]
	--@NAATINumberType int,
	@EntityTypeId int, 
	@ABN varchar (50), 
	@UseEmail bit, 
	@GSTApplies bit,
	@WebsiteInPD bit,
	@WebsiteURL varchar (200), 	
	@Note varchar (8000), 
	@AccountNumber varchar(255),
	@NaatiNumber int
AS

BEGIN
	INSERT INTO tblEntity ( NAATINumber, WebsiteURL, ABN, Note, UseEmail, WebsiteInPD, GSTApplies,EntityTypeId, AccountNumber) 
	VALUES ((case @NaatiNumber when 0 then dbo.fnGetNextNAATINumber(@EntityTypeId) else @NaatiNumber end) , @WebsiteURL, @ABN, @Note, @UseEmail, @WebsiteInPD, @GSTApplies, @EntityTypeId, @AccountNumber)

	SELECT SCOPE_IDENTITY()

END