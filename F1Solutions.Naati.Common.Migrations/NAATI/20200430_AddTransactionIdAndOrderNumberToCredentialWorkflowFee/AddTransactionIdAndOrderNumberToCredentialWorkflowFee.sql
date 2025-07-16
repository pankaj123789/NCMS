ALTER TABLE [tblCredentialWorkflowFee] ADD TransactionId varchar(50) NULL;
ALTER TABLE [tblCredentialWorkflowFee] ADD OrderNumber varchar(50) NULL;
GO 

UPDATE [tblCredentialWorkflowFee] 
SET [TransactionId] = TRIM((SELECT SUBSTRING(PaymentReference ,LEN(PaymentReference)-CHARINDEX('-',REVERSE(PaymentReference)) + 2  , LEN(PaymentReference))))
WHERE PaymentReference is not null;

UPDATE [tblCredentialWorkflowFee] 
SET OrderNumber = CONCAT (e.NAATINumber,'-APP',ca.CredentialApplicationId)
FROM tblCredentialWorkflowFee wf
JOIN tblCredentialApplication ca
ON wf.CredentialApplicationId = ca.CredentialApplicationId
JOIN tblPerson p
ON ca.PersonId = p.PersonId
JOIN tblEntity e
ON p.EntityId = e.EntityId
WHERE ca.CredentialApplicationTypeId IN (3, 7, 13) AND wf.PaymentReference IS NOT NULL

--UPDATE [tblCredentialWorkflowFee] 
--SET OrderNumber = CONCAT (e.NAATINumber,'-',wf.InvoiceNumber)
--FROM tblCredentialWorkflowFee wf
--JOIN tblCredentialApplication ca
--ON wf.CredentialApplicationId = ca.CredentialApplicationId
--JOIN tblPerson p
--ON ca.PersonId = p.PersonId
--JOIN tblEntity e
--ON p.EntityId = e.EntityId
--WHERE ca.CredentialApplicationTypeId NOT IN (3, 7, 13) AND InvoiceNumber IS NOT NULL AND wf.PaymentReference IS NOT NULL