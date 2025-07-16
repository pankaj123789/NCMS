using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProductCategoryScriptGenerator : BaseScriptGenerator
    {
        public ProductCategoryScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tluProductCategory";
        public override IList<string> Columns => new[] {
                                                           "ProductCategoryId",
                                                           "Name",
                                                           "DisplayName",
                                                           "Code",
                                                           "ProductTypeId"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "AssessmentFee", "Assessment Fee", "", null });
            CreateOrUpdateTableRow(new[] { "2", "TestingFee", "Testing Fee", "", null });
            CreateOrUpdateTableRow(new[] { "3", "AdminFee", "Admin Fee", "", null });
            CreateOrUpdateTableRow(new[] { "4", "Publications", "Publications", "", null });
            CreateOrUpdateTableRow(new[] { "5", "Products", "Products", "", null });

            CreateOrUpdateTableRow(new[] { "6", "Marking", "Marking", "", "5" });
            CreateOrUpdateTableRow(new[] { "7", "MarkingReview", "Marking Review", "", "5" });
            CreateOrUpdateTableRow(new[] { "8",  "ApplicationFee", "Application Fee", "", null });
            CreateOrUpdateTableRow(new[] { "9", "SupplementaryTestFee", "Supplementary Test Fee", "", null });
            CreateOrUpdateTableRow(new[] { "10", "ReviewFee", "Review Fee", "", null });
            CreateOrUpdateTableRow(new[] { "11", "MaterialCreationFee", "Material Creation", "", null });
            CreateOrUpdateTableRow(new[] { "12", "Scholarship", "Scholarship", "", null });
        }
    }
}