
namespace F1Solutions.Naati.Common.Migrations.NAATI._20210520_AddNewCandidatesOnlyToTestSitting
{

        [NaatiMigration(202105200756)]
        public class AddNewCandidatesOnlyToSessionTable : NaatiMigration
        {
            public override void Up()
            {
                Alter.Table("tblTestSession").AddColumn("NewCandidatesOnly").AsBoolean().Nullable();
            }
        }

}
