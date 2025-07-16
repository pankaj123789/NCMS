namespace F1Solutions.Naati.Common.Migrations.NAATI._20200519_AddPolicyIdToProductSpecAndWorkflowFee
{
    [NaatiMigration(202005191125)]
    public class AddPolicyIdToProductSpecAndWorkflowFee : NaatiMigration
    {
        public override void Up()
        {
            //alter CredentialFeeProduct table 

            Alter.Table("tblCredentialFeeProduct").AddColumn("CredentialApplicationRefundPolicyId").AsInt32().Nullable();            

            Update.Table("tblCredentialFeeProduct").Set(new { CredentialApplicationRefundPolicyId = 1 }).AllRows();

            Alter.Table("tblCredentialFeeProduct").AlterColumn("CredentialApplicationRefundPolicyId").AsInt32().NotNullable()
                .ForeignKey("FK_CredentialFeeProduct_CredentialApplicationRefundPolicy", "tblCredentialApplicationRefundPolicy", "CredentialApplicationRefundPolicyId");

            //alter CredentialWorkflowFee table

            Alter.Table("tblCredentialWorkflowFee").AddColumn("CredentialApplicationRefundPolicyId").AsInt32().Nullable();

            Update.Table("tblCredentialWorkflowFee").Set(new { CredentialApplicationRefundPolicyId = 1 }).AllRows();

            Update.Table("tblCredentialWorkflowFee").Set(new { CredentialApplicationRefundPolicyId = 2 }).Where(new { ProductSpecificationId = 42 });

            Execute.Sql(@"UPDATE wf
                        SET wf.CredentialApplicationRefundPolicyId = 3
                        From tblCredentialWorkflowFee wf
                        inner join tblCredentialRequest cr on  wf.CredentialRequestId  = cr.CredentialRequestId 
                        WHERE wf.ProductSpecificationId IN (4,6,7,8,10,11,12,13,14,15,42,91,92)
                        AND ((Cr.CredentialTypeId  Between 1 and 13 ) or Cr.CredentialTypeId = 15 or (cr.CredentialTypeId Between 19 and 32 ))");

            Execute.Sql(@"UPDATE wf
                        SET wf.CredentialApplicationRefundPolicyId = 2
                        From tblCredentialWorkflowFee wf
                        INNER JOIN tblCredentialRequest cr ON  wf.CredentialRequestId  = cr.CredentialRequestId 
                        WHERE wf.ProductSpecificationId IN (13,14)
                        AND cr.CredentialTypeId IN (16,17)");

            Alter.Table("tblCredentialWorkflowFee").AlterColumn("CredentialApplicationRefundPolicyId").AsInt32().NotNullable()
                .ForeignKey("FK_CredentialWorkflowFee_CredentialApplicationRefundPolicy", "tblCredentialApplicationRefundPolicy", "CredentialApplicationRefundPolicyId");

        }
    }
}
