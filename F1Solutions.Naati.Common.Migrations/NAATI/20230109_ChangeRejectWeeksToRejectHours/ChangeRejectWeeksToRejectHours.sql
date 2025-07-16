IF COL_LENGTH('dbo.tblCredentialType', 'TestSessionBookingRejectWeeks') IS NOT NULL
BEGIN
	UPDATE tblCredentialType
	SET TestSessionBookingRejectWeeks = TestSessionBookingRejectWeeks * 168

	EXEC sp_rename 'dbo.tblCredentialType.TestSessionBookingRejectWeeks', 'TestSessionBookingRejectHours', 'COLUMN'
END