
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190618_AddMaterialRequest
{
    [NaatiMigration(201907011125)]
    public class AddMaterialRequest : NaatiMigration
    {
        public override void Up()
        {
            CreateMaterialRequestStatusType();
            CreateMaterialRequestTable();
            CreateMaterialRequestRoundStatusType();
            CreateMaterialRequestRound();
            CreateMaterialRequestPanelMembershipType();
            CreateMaterialRequestPanelMembership();
            CreateMaterialRequestNote();
            CreateMaterialRequestPublicNote();
            CreateMaterialRequestAttachment();
            AddTestMaterialDomain();
            AddMaterialType();
            AddMaterialTypeToTestMaterial();
            AddMaterialLinkType();
            AddMaterialLink();
            AddUserToEmailMessage();
            UpdateEmailTable();
            AddMaterialRequestTaskType();
            AddMaterialRequestPanelMembershipTask();
            AddMaxMaterialRequestHours();
            ModifyProductCategory();
            AddMaterialRequestPayRoll();
            CreateMaterialRequestRoundLink();
        }

        private void CreateMaterialRequestRoundLink()
        {
            Create.Table("tblMaterialRequestRoundLink")
                .WithColumn("MaterialRequestRoundLinkId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestRoundId").AsInt32()
                .WithColumn("Link").AsString(500)
                .WithColumn("UserId").AsInt32().Nullable()
                .WithColumn("PersonId").AsInt32().Nullable()
                .WithColumn("NcmsAvailable").AsBoolean();

            Create.ForeignKey("FK_MaterialRequestRoundLink_MaterialRequestRound")
                .FromTable("tblMaterialRequestRoundLink")
                .ForeignColumn("MaterialRequestRoundId")
                .ToTable("tblMaterialRequestRound")
                .PrimaryColumn("MaterialRequestRoundId");

            Create.ForeignKey("FK_MaterialRequestRoundLink_User")
                .FromTable("tblMaterialRequestRoundLink")
                .ForeignColumn("UserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_MaterialRequestRoundLink_Person")
                .FromTable("tblMaterialRequestRoundLink")
                .ForeignColumn("PersonId")
                .ToTable("tblPerson")
                .PrimaryColumn("PersonId");
        }
        private void AddTestMaterialDomain()
        {
            Create.Table("tblTestMaterialDomain")
                .WithColumn("TestMaterialDomainId").AsInt32().Identity().PrimaryKey()
                .WithColumn("DisplayName").AsString(255);

            Insert.IntoTable("tblTestMaterialDomain").Row(new { DisplayName = "None" });

            Create.Table("tblCredentialTypeTestMaterialDomain")
                .WithColumn("CredentialTypeTestMaterialDomainId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("TestMaterialDomainId").AsInt32();

            Create.ForeignKey("FK_CredentialTypeTestMaterialDomain_CredentialType")
                .FromTable("tblCredentialTypeTestMaterialDomain")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialTypeTestMaterialDomain_TestMaterialDomain")
                .FromTable("tblCredentialTypeTestMaterialDomain")
                .ForeignColumn("TestMaterialDomainId")
                .ToTable("tblTestMaterialDomain")
                .PrimaryColumn("TestMaterialDomainId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialTypeTestMaterialDomain] 
                ADD  CONSTRAINT [U_tblCredentialTypeTestMaterialDomain]
                UNIQUE NONCLUSTERED ([CredentialTypeId] , [TestMaterialDomainId])");
        }
        

        private void ModifyProductCategory()
        {
            Create.Column("DisplayName").OnTable("tluProductCategory").AsString(255).Nullable();
            Update.Table("tluProductCategory").Set(new { DisplayName = string.Empty }).AllRows();
            Alter.Column("DisplayName").OnTable("tluProductCategory").AsString(255).NotNullable();
        }
        private void AddMaxMaterialRequestHours()
        {
            Create.Column("DefaultMaterialRequestHours").OnTable("tblTestComponentType").AsDouble().Nullable();
            Create.Column("DefaultMaterialRequestDueDays").OnTable("tblTestComponentType").AsInt32().Nullable();
            Update.Table("tblTestComponentType").Set(new { DefaultMaterialRequestHours = 0.0, DefaultMaterialRequestDueDays = 12 }).AllRows();
            Alter.Column("DefaultMaterialRequestHours").OnTable("tblTestComponentType").AsDouble().NotNullable();
            Alter.Column("DefaultMaterialRequestDueDays").OnTable("tblTestComponentType").AsDouble().NotNullable();
        }
        private void AddMaterialRequestPanelMembershipTask()
        {
            Create.Table("tblMaterialRequestPanelMembershipTask")
            .WithColumn("MaterialRequestPanelMembershipTaskId").AsInt32().Identity().PrimaryKey()
            .WithColumn("MaterialRequestPanelMembershipId").AsInt32()
            .WithColumn("MaterialRequestTaskTypeId").AsInt32()
            .WithColumn("HoursSpent").AsDouble();

            Create.ForeignKey("FK_MaterialRequestPanelMembershipTask_MaterialRequestTaskType")
                .FromTable("tblMaterialRequestPanelMembershipTask")
                .ForeignColumn("MaterialRequestTaskTypeId")
                .ToTable("tblMaterialRequestTaskType")
                .PrimaryColumn("MaterialRequestTaskTypeId");

            Create.ForeignKey("FK_MaterialRequestPanelMembershipTask_MaterialRequestPanelMembership")
                .FromTable("tblMaterialRequestPanelMembershipTask")
                .ForeignColumn("MaterialRequestPanelMembershipId")
                .ToTable("tblMaterialRequestPanelMembership")
                .PrimaryColumn("MaterialRequestPanelMembershipId");
        }

        private void AddMaterialRequestTaskType()
        {
            Create.Table("tblMaterialRequestTaskType")
                .WithColumn("MaterialRequestTaskTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);
        }

        private void UpdateEmailTable()
        {
            Execute.Sql("UPDATE TBLUSER SET EMAIL = 'DefaultEmail@invalidEmail.com.au' where Email is null");
            Alter.Column("Email").OnTable("tblUser").AsString(200).NotNullable();
        }

        private void AddUserToEmailMessage()
        {
            Create.Column("RecipientUserId").OnTable("tblEmailMessage").AsInt32().Nullable();

            Create.ForeignKey("FK_EmailMessage_UserId")
                .FromTable("tblEmailMessage")
                .ForeignColumn("RecipientUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Alter.Column("RecipientEntityId").OnTable("tblEmailMessage").AsInt32().Nullable();
        }

        private void CreateMaterialRequestStatusType()
        {
            Create.Table("tblMaterialRequestStatusType")
                .WithColumn("MaterialRequestStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);
        }
        private void CreateMaterialRequestTable()
        {
            Create.Table("tblMaterialRequest")
                .WithColumn("MaterialRequestId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PanelId").AsInt32()
                .WithColumn("OutputMaterialId").AsInt32()
                .WithColumn("MaterialRequestStatusTypeId").AsInt32()
                .WithColumn("ProductSpecificationId").AsInt32()
                .WithColumn("SourceMaterialId").AsInt32().Nullable()
                .WithColumn("CreatedDate").AsDateTime()
                .WithColumn("CreatedByUserId").AsInt32()
                .WithColumn("StatusChangeDate").AsDateTime()
                .WithColumn("StatusChangeUserId").AsInt32()
                .WithColumn("OwnedByUserId").AsInt32().Nullable()
                .WithColumn("MaxBillableHours").AsDouble();

            Create.ForeignKey("FK_MaterialRequest_TestMaterial")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("OutputMaterialId")
                .ToTable("tblTestMaterial")
                .PrimaryColumn("TestMaterialId");

            Create.ForeignKey("FK_MaterialRequest_SourceTestMaterial")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("SourceMaterialId")
                .ToTable("tblTestMaterial")
                .PrimaryColumn("TestMaterialId");

            Create.ForeignKey("FK_MaterialRequest_Panel")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("PanelId")
                .ToTable("tblPanel")
                .PrimaryColumn("PanelId");

            Create.ForeignKey("FK_MaterialRequest_MaterialRequestStatusType")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("MaterialRequestStatusTypeId")
                .ToTable("tblMaterialRequestStatusType")
                .PrimaryColumn("MaterialRequestStatusTypeId");

            Create.ForeignKey("FK_MaterialRequest_UserCreated")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("CreatedByUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_MaterialRequest_UserStatusChange")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("StatusChangeUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_MaterialRequest_OwnedBy")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("OwnedByUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_MaterialRequest_ProductSpecification")
                .FromTable("tblMaterialRequest")
                .ForeignColumn("ProductSpecificationId")
                .ToTable("tblProductSpecification")
                .PrimaryColumn("ProductSpecificationId");

        }


        private void CreateMaterialRequestRoundStatusType()
        {
            Create.Table("tblMaterialRequestRoundStatusType")
                .WithColumn("MaterialRequestRoundStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);
        }
        private void CreateMaterialRequestRound()
        {
            Create.Table("tblMaterialRequestRound")
                .WithColumn("MaterialRequestRoundId").AsInt32().Identity().PrimaryKey()
                .WithColumn("RoundNumber").AsInt32()
                .WithColumn("MaterialRequestId").AsInt32()
                .WithColumn("DueDate").AsDate()
                .WithColumn("RequestedDate").AsDateTime()
                .WithColumn("SubmittedDate").AsDateTime().Nullable()
                .WithColumn("StatusChangeDate").AsDateTime()
                .WithColumn("ModifiedUserId").AsInt32()
                .WithColumn("MaterialRequestRoundStatusTypeId").AsInt32();
                


            Create.ForeignKey("FK_MaterialRequestRound_MaterialRequest")
                .FromTable("tblMaterialRequestRound")
                .ForeignColumn("MaterialRequestId")
                .ToTable("tblMaterialRequest")
                .PrimaryColumn("MaterialRequestId");

            Create.ForeignKey("FK_MaterialRequestRound_MaterialRequestRoundStatusType")
                .FromTable("tblMaterialRequestRound")
                .ForeignColumn("MaterialRequestRoundStatusTypeId")
                .ToTable("tblMaterialRequestRoundStatusType")
                .PrimaryColumn("MaterialRequestRoundStatusTypeId");

            Create.ForeignKey("FK_MaterialRequestRound_ModifiedUser")
                .FromTable("tblMaterialRequestRound")
                .ForeignColumn("ModifiedUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");
        }

        private void CreateMaterialRequestPanelMembershipType()
        {
            Create.Table("tblMaterialRequestPanelMembershipType")
                .WithColumn("MaterialRequestPanelMembershipTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);
        }

        private void CreateMaterialRequestPanelMembership()
        {
            Create.Table("tblMaterialRequestPanelMembership")
                .WithColumn("MaterialRequestPanelMembershipId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32()
                .WithColumn("PanelMembershipId").AsInt32()
                .WithColumn("MaterialRequestPanelMembershipTypeId").AsInt32();


            Create.ForeignKey("FK_MaterialRequestPanelMembership_PanelMembership")
                .FromTable("tblMaterialRequestPanelMembership")
                .ForeignColumn("PanelMembershipId")
                .ToTable("tblPanelMembership")
                .PrimaryColumn("PanelMembershipId");

            Create.ForeignKey("FK_MaterialRequestPanelMembership_MaterialRequestPanelMembershipType")
                .FromTable("tblMaterialRequestPanelMembership")
                .ForeignColumn("MaterialRequestPanelMembershipTypeId")
                .ToTable("tblMaterialRequestPanelMembershipType")
                .PrimaryColumn("MaterialRequestPanelMembershipTypeId");

            Create.ForeignKey("FK_MaterialRequestPanelMembership_MaterialRequest")
                .FromTable("tblMaterialRequestPanelMembership")
                .ForeignColumn("MaterialRequestId")
                .ToTable("tblMaterialRequest")
                .PrimaryColumn("MaterialRequestId");
        }

        private void AddMaterialLinkType()
        {
            Create.Table("tblTestMaterialLinkType")
                .WithColumn("TestMaterialLinkTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);
        }


        private void AddMaterialLink()
        {
            Create.Table("tblTestMaterialLink")
                .WithColumn("TestMaterialLinkId").AsInt32().Identity().PrimaryKey()
                .WithColumn("FromTestMaterialId").AsInt32()
                .WithColumn("ToTestMaterialId").AsInt32()
                .WithColumn("TestMaterialLinkTypeId").AsInt32();

            Create.ForeignKey("FK_TestMaterialLink_FromTestMaterial")
                .FromTable("tblTestMaterialLink")
                .ForeignColumn("FromTestMaterialId")
                .ToTable("tblTestMaterial")
                .PrimaryColumn("TestMaterialId");

            Create.ForeignKey("FK_TestMaterialLink_ToTestMaterial")
                .FromTable("tblTestMaterialLink")
                .ForeignColumn("ToTestMaterialId")
                .ToTable("tblTestMaterial")
                .PrimaryColumn("TestMaterialId");


            Create.ForeignKey("FK_TestMaterialLink_TestMaterialLinkType")
                .FromTable("tblTestMaterialLink")
                .ForeignColumn("TestMaterialLinkTypeId")
                .ToTable("tblTestMaterialLinkType")
                .PrimaryColumn("TestMaterialLinkTypeId");
        }
        private void AddMaterialType()
        {
            Create.Table("tblTestMaterialType")
                .WithColumn("TestMaterialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(255)
                .WithColumn("DisplayName").AsString(255);

            Insert.IntoTable("tblTestMaterialType").Row(new { Name = "Test", DisplayName = "Test" });
        }

        private void AddMaterialTypeToTestMaterial()
        {
            Alter.Table("tblTestMaterial").AddColumn("TestMaterialTypeId").AsInt32().Nullable();
            Alter.Table("tblTestMaterial").AddColumn("TestMaterialDomainId").AsInt32().Nullable();

            Update.Table("tblTestMaterial").Set(new { TestMaterialTypeId = 1 }).AllRows();
            Update.Table("tblTestMaterial").Set(new { TestMaterialDomainId = 1 }).AllRows();

            Alter.Column("TestMaterialTypeId").OnTable("tblTestMaterial").AsInt32().NotNullable();
            Alter.Column("TestMaterialDomainId").OnTable("tblTestMaterial").AsInt32().NotNullable();

            Create.ForeignKey("FK_TestMaterial_TestMaterialType")
                .FromTable("tblTestMaterial")
                .ForeignColumn("TestMaterialTypeId")
                .ToTable("tblTestMaterialType")
                .PrimaryColumn("TestMaterialTypeId");

            Create.ForeignKey("FK_TestMaterial_TestMaterialDomain")
                .FromTable("tblTestMaterial")
                .ForeignColumn("TestMaterialDomainId")
                .ToTable("tblTestMaterialDomain")
                .PrimaryColumn("TestMaterialDomainId");
        }

        private void CreateMaterialRequestNote()
        {
            Create.Table("tblMaterialRequestNote")
                .WithColumn("MaterialRequestNoteId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32()
                .WithColumn("NoteId").AsInt32();

            Create.ForeignKey("FK_MaterialRequestNote_MaterialRequest")
                .FromTable("tblMaterialRequestNote")
                .ForeignColumn("MaterialRequestId")
                .ToTable("tblMaterialRequest")
                .PrimaryColumn("MaterialRequestId");

            Create.ForeignKey("FK_MaterialRequestNote_Note")
                .FromTable("tblMaterialRequestNote")
                .ForeignColumn("NoteId")
                .ToTable("tblNote")
                .PrimaryColumn("NoteId");
        }

        private void CreateMaterialRequestPublicNote()
        {
            Create.Table("tblMaterialRequestPublicNote")
                .WithColumn("MaterialRequestPublicNoteId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32()
                .WithColumn("NoteId").AsInt32();

            Create.ForeignKey("FK_MaterialRequestPublicNote_MaterialRequest")
                .FromTable("tblMaterialRequestPublicNote")
                .ForeignColumn("MaterialRequestId")
                .ToTable("tblMaterialRequest")
                .PrimaryColumn("MaterialRequestId");

            Create.ForeignKey("FK_MaterialRequestPublicNote_Note")
                .FromTable("tblMaterialRequestPublicNote")
                .ForeignColumn("NoteId")
                .ToTable("tblNote")
                .PrimaryColumn("NoteId");

        }

        private void CreateMaterialRequestAttachment()
        {
            Create.Table("tblMaterialRequestRoundAttachment")
                .WithColumn("MaterialRequestRoundAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestRoundId").AsInt32()
                .WithColumn("StoredFileId").AsInt32()
                .WithColumn("Description").AsString(255)
                .WithColumn("ExaminersAvailable").AsBoolean()
                .WithColumn("NcmsAvailable").AsBoolean();


            Create.ForeignKey("FK_MaterialRequestRoundAttachment_StoredFile")
                .FromTable("tblMaterialRequestRoundAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");

            Create.ForeignKey("FK_MaterialRequestRoundAttachment_MaterialRequestRound")
                .FromTable("tblMaterialRequestRoundAttachment")
                .ForeignColumn("MaterialRequestRoundId")
                .ToTable("tblMaterialRequestRound")
                .PrimaryColumn("MaterialRequestRoundId");
        }

        private void AddMaterialRequestPayRoll()
        {
            Create.Table("tblMaterialRequestPayroll")
                .WithColumn("MaterialRequestPayrollId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestPanelMembershipId").AsInt32()
                .WithColumn("ApprovedByUserId").AsInt32().Nullable()
                .WithColumn("ApprovedDate").AsDateTime().Nullable()
                .WithColumn("PaidByUserId").AsInt32().Nullable()
                .WithColumn("PaymentDate").AsDateTime().Nullable()
                .WithColumn("Amount").AsDecimal(10,4).Nullable()
                .WithColumn("PaymentReference").AsString(255).Nullable();

            Create.ForeignKey("FK_MaterialRequestPayroll_MaterialRequestPanelMembership")
                .FromTable("tblMaterialRequestPayroll")
                .ForeignColumn("MaterialRequestPanelMembershipId")
                .ToTable("tblMaterialRequestPanelMembership")
                .PrimaryColumn("MaterialRequestPanelMembershipId");
            
            Create.ForeignKey("FK_MaterialRequestPayroll_ApprovedByUser")
                .FromTable("tblMaterialRequestPayroll")
                .ForeignColumn("ApprovedByUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Create.ForeignKey("FK_MaterialRequestPayroll_PaidByUser")
                .FromTable("tblMaterialRequestPayroll")
                .ForeignColumn("PaidByUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");
        
        }
    
    }
}
