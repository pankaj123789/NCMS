using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProductTypeScriptGenerator : BaseScriptGenerator
    {
        public ProductTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tluProductType";
        public override IList<string> Columns => new[] {
                                                           "ProductTypeId",
                                                           "Name",
                                                           "DisplayName",
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "IDCards", "ID Cards" });
            CreateOrUpdateTableRow(new[] { "2", "Stamps", "Stamps" });
            CreateOrUpdateTableRow(new[] { "3", "Certificates", "Certificates" });
            CreateOrUpdateTableRow(new[] { "4", "Workshops", "Workshops" });
            CreateOrUpdateTableRow(new[] { "5", "MarkingClaims", "Claims - Marking" });
        }
    }
}