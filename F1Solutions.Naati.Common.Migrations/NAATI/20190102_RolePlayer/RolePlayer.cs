namespace F1Solutions.Naati.Common.Migrations.NAATI._20190102_RolePlayer
{
    [NaatiMigration(201901021250)]
    public class RolePlayer : NaatiMigration
    {
        public override void Up()
        {
			CreateRolePlayer();
			CreateRolePlayerStatusType();
			CreateRolePlayerRoleType();
			CreateRolePlayerTestLocation();
			CreateTestSessionRolePlayer();
			CreateTestSessionRolePlayerDetail();
            AddRolePlayerFlagToComponentType();
            AddRehersalDateTimeToTestSession();
        }

		private void CreateTestSessionRolePlayerDetail()
		{
			Create.Table("tblTestSessionRolePlayerDetail")
				.WithColumn("TestSessionRolePlayerDetailId").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("TestSessionRolePlayerId").AsInt32().NotNullable().ForeignKey("tblTestSessionRolePlayer", "TestSessionRolePlayerId")
				.WithColumn("TestComponentId").AsInt32().NotNullable().ForeignKey("tblTestComponent", "TestComponentId")
				.WithColumn("SkillId").AsInt32().NotNullable().ForeignKey("tblSkill", "SkillId")
				.WithColumn("LanguageId").AsInt32().NotNullable().ForeignKey("tblLanguage", "LanguageId")
				.WithColumn("RolePlayerRoleTypeId").AsInt32().NotNullable().ForeignKey("tblRolePlayerRoleType", "RolePlayerRoleTypeId");
		}

		private void CreateTestSessionRolePlayer()
		{
			Create.Table("tblTestSessionRolePlayer")
				.WithColumn("TestSessionRolePlayerId").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("RolePlayerId").AsInt32().NotNullable().ForeignKey("tblRolePlayer", "RolePlayerId")
				.WithColumn("TestSessionId").AsInt32().NotNullable().ForeignKey("tblTestSession", "TestSessionId")
				.WithColumn("RolePlayerStatusTypeId").AsInt32().NotNullable().ForeignKey("tblRolePlayerStatusType", "RolePlayerStatusTypeId")
				.WithColumn("Rehearsed").AsBoolean().NotNullable()
				.WithColumn("Attended").AsBoolean().NotNullable()
		        .WithColumn("Rejected").AsBoolean().NotNullable()
		        .WithColumn("StatusChangeDate").AsDateTime().NotNullable()
		        .WithColumn("StatusChangeUserId").AsInt32().NotNullable().ForeignKey("tblUser", "UserId");
        }

		private void CreateRolePlayerRoleType()
		{
			Create.Table("tblRolePlayerRoleType")
				.WithColumn("RolePlayerRoleTypeId").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Name").AsString(50).NotNullable()
				.WithColumn("DisplayName").AsString(50).NotNullable();
		}

		private void CreateRolePlayerStatusType()
		{
			Create.Table("tblRolePlayerStatusType")
				.WithColumn("RolePlayerStatusTypeId").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Name").AsString(50).NotNullable()
				.WithColumn("DisplayName").AsString(50).NotNullable();
		}

		private void CreateRolePlayerTestLocation()
		{
			Create.Table("tblRolePlayerTestLocation")
				.WithColumn("RolePlayerTestLocationId").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("RolePlayerId").AsInt32().NotNullable().ForeignKey("tblRolePlayer", "RolePlayerId")
				.WithColumn("TestLocationId").AsInt32().NotNullable().ForeignKey("tblTestLocation", "TestLocationId");
		}

		private void CreateRolePlayer()
		{
		    Create.Table("tblRolePlayer")
		        .WithColumn("RolePlayerId").AsInt32().NotNullable().Identity().PrimaryKey()
		        .WithColumn("PersonId").AsInt32().NotNullable().ForeignKey("tblPerson", "PersonId")
		        .WithColumn("SessionLimit").AsInt32().NotNullable()
		        .WithColumn("Rating").AsDecimal(3, 1).Nullable()
		        .WithColumn("Senior").AsBoolean().NotNullable();

		    Execute.Sql(@"
                ALTER TABLE [dbo].[tblRolePlayer] 
                ADD  CONSTRAINT [U_RolePlayer]
                UNIQUE NONCLUSTERED ([PersonId])");
        }

        private void AddRolePlayerFlagToComponentType()
        {
            Create.Column("RoleplayersRequired").OnTable("tblTestComponentType").AsBoolean().Nullable();
            Update.Table("tblTestComponentType").Set(new { RoleplayersRequired = false }).AllRows();
            Alter.Column("RoleplayersRequired").OnTable("tblTestComponentType").AsBoolean().NotNullable();
        }

        private void AddRehersalDateTimeToTestSession()
        {
            Create.Column("RehearsalDateTime").OnTable("tblTestSession").AsDateTime().Nullable();
            Create.Column("RehearsalNotes").OnTable("tblTestSession").AsString(int.MaxValue).Nullable();
        }

    }
}
