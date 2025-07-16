select count(*) as 'count',
/*e.EmailTemplateId as 'EmailTemplateId',
e.Name as 'EmailTemplateName', */
d.name as 'SystemActionTypeName', 
h.Name as 'SystemActionEventTypeName',
g.Name as 'CredentialApplicationTypeName',
i.Name as 'EmailTemplateDetailTypeName'
from
tblSystemActionEmailTemplate a inner join
tblCredentialWorkflowActionEmailTemplate b on b.SystemActionEmailTemplateId = a.SystemActionEmailTemplateId inner join
tblSystemActionEmailTemplateDetail c on c.SystemActionEmailTemplateId = a.SystemActionEmailTemplateId inner join
tblSystemActionType d on d.SystemActionTypeId = a.SystemActionTypeId inner join
tblEmailTemplate e on e.EmailTemplateId = a.EmailTemplateId inner join
tblCredentialApplicationType g on g.CredentialApplicationTypeId = b.CredentialApplicationTypeId inner join
tblSystemActionEventType h on  h.SystemActionEventTypeId = a.SystemActionEventTypeId inner join
tblEmailTemplateDetailType i on i.EmailTemplateDetailTypeId = c.EmailTemplateDetailTypeId
/*where d.Name like 'FinishChecking'
and h.name like 'InvoiceCreatedToUntrustedSponsor'
and g.Name like 'Ethics'*/
group by /*e.EmailTemplateId, e.Name,*/ d.Name, h.Name, g.Name, i.Name
having count(*) > 1