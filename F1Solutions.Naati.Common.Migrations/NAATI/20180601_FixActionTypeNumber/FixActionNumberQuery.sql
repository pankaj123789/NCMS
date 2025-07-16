SET IDENTITY_INSERT [dbo].tblCredentialWorkflowActionType ON 

INSERT INTO tblCredentialWorkflowActionType (CredentialWorkflowActionTypeId,Name, DisplayName )
VALUES
(14, 'SubmitApplicationWithCreditCard','Submit Application with CreditCard'),
(15,'ApplicationInvoicePaid','Application Invoice Paid')
SET IDENTITY_INSERT [dbo].tblCredentialWorkflowActionType OFF

UPDATE tblCredentialWorkflowFee SET  OnPaymentCreatedCredentialWorkflowActionTypeId = 14 where OnPaymentCreatedCredentialWorkflowActionTypeId = 1038
UPDATE tblCredentialWorkflowFee SET  OnPaymentCreatedCredentialWorkflowActionTypeId = 15 where OnPaymentCreatedCredentialWorkflowActionTypeId = 1037

UPDATE tblCredentialWorkflowActionEmailTemplate set CredentialWorkflowActionTypeId = 14 where CredentialWorkflowActionEmailTemplateId = 118 
UPDATE tblCredentialWorkflowActionEmailTemplate set CredentialWorkflowActionTypeId = 15 where CredentialWorkflowActionEmailTemplateId = 119

DELETE  FROM tblCredentialWorkflowActionType WHERE CredentialWorkflowActionTypeId in (1037,1038)