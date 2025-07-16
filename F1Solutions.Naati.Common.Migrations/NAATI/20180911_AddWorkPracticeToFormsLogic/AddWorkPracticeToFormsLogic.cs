
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180911_AddWorkPracticeToFormsLogic
{
    [NaatiMigration(201809111603)]
    public class AddWorkPracticeToFormsLogic : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialApplicationFormQuestionLogic").AddColumn("PdPointsMet").AsBoolean().Nullable();
            Alter.Table("tblCredentialApplicationFormQuestionLogic").AddColumn("WorkPracticeMet").AsBoolean()
                .Nullable();
        }
    }
}
