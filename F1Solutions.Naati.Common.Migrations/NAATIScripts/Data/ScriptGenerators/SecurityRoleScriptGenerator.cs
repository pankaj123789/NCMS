using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SecurityRoleScriptGenerator : BaseScriptGenerator
    {
        public SecurityRoleScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblSecurityRole";

        public override IList<string> Columns => new[]
        {
            "SecurityRoleId",
            "Name",
            "DisplayName",
            "Description",
            "System"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "SeniorManagement", "Senior Management", "", "0"});
            CreateOrUpdateTableRow(new[] {"2", "SystemAdministrator", "System Administrator", "", "0" });
            CreateOrUpdateTableRow(new[] {"3", "TestingOperationsAdmin", "Testing Operations Administrator", "", "0" });
            CreateOrUpdateTableRow(new[] {"4", "TestingOperations", "Testing Operations", "", "0" });
            CreateOrUpdateTableRow(new[] {"5", "ResourceAndScheduling", "Resource & Scheduling", "", "0" });
            CreateOrUpdateTableRow(new[] {"6", "RegionalManager", "Regional Manager", "", "0" });
            CreateOrUpdateTableRow(new[] {"7", "RegionalOperations", "Regional Operations", "", "0" });
            CreateOrUpdateTableRow(new[] {"8", "Communications", "Communications", "", "0" });
            CreateOrUpdateTableRow(new[] {"9", "Finance", "Finance", "", "0" });
            CreateOrUpdateTableRow(new[] {"10", "DevelopmentTeam", "Development Team", "", "0" });
            CreateOrUpdateTableRow(new[] {"11", "TestInvigilator", "Test Invigilator", "", "0" });
            CreateOrUpdateTableRow(new[] {"12", "GeneralAccount", "General Account", "", "0" });
            CreateOrUpdateTableRow(new[] {"13", "SystemRole", "System Role", "", "1" });
            CreateOrUpdateTableRow(new[] {"14", "ApiAdministrator", "Api Administrator", "", "0" });
        }
    }
}