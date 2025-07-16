using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class DocumentTypeCategoryScriptGenerator : BaseScriptGenerator
    {
        public DocumentTypeCategoryScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblDocumentTypeCategory";
        public override IList<string> Columns => new[] {
                                                           "DocumentTypeCategoryId",
                                                           "Name",
                                                           "DisplayName",
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Tests", "Tests" });
            CreateOrUpdateTableRow(new[] { "2", "Applications", "Applications" });
            CreateOrUpdateTableRow(new[] { "3", "General", "General"});
            CreateOrUpdateTableRow(new[] { "4", "TestMaterial", "Test Material"});
            CreateOrUpdateTableRow(new[] { "5", "TestSpecification", "Test Specification" });
            CreateOrUpdateTableRow(new[] { "6", "Person", "Person" });
            CreateOrUpdateTableRow(new[] { "7", "TestMaterialRequest", "Test Material Request" });
            CreateOrUpdateTableRow(new[] { "8", "MaterialRequestSubmission", "Material Request Submission" });
        }
    }
}