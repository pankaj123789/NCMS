using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class StoredFileDeletePolicyDocumentTypeScriptGenerator : BaseScriptGenerator
    {
        public StoredFileDeletePolicyDocumentTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }
        public override string TableName => "tblStoredFileDeletePolicyDocumentType";

        public override IList<string> Columns => new[] {
            "StoredFileDeletePolicyDocumentTypeId",
            "DocumentTypeId",
            "StoredFileDeletePolicyId",
            "Description",
            "EntityType"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "25", "6", "Application documents", "tblCredentialApplicationAttachment"});
            CreateOrUpdateTableRow(new[] { "2", "26", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "3", "21", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "4", "13", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "5", "43", "5", "Material request round document", "tblMaterialRequestRoundAttachment"});
            CreateOrUpdateTableRow(new[] { "6", "40", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "7", "33", null, "Test specification document", "tblTestSpecificationAttachment" });
            CreateOrUpdateTableRow(new[] { "8", "34", null, "Test specification document", "tblTestSpecificationAttachment" });
            CreateOrUpdateTableRow(new[] { "9", "30", null, "Test material document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "10", "30", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });

            CreateOrUpdateTableRow(new[] { "11", "18", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "12",  "6", null, "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "13",  "4", "2", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "14", "14", "6", "Material request round document", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "15", "15", "6", "Application Documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "16", "31", null, "Test specification document", "tblTestSpecificationAttachment" });
            CreateOrUpdateTableRow(new[] { "17", "31", "5", "Test material document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "18", "39","7", "Person documents", "tblPersonAttachment" });
            CreateOrUpdateTableRow(new[] { "19", "39", "7", "Note attachment", "tblNoteAttachment" });
            CreateOrUpdateTableRow(new[] { "20", "39", "7", "Application documents", "tblCredentialApplicationAttachment" });

            CreateOrUpdateTableRow(new[] { "21", "8", "3", "Note attachment", "tblNoteAttachment" });
            CreateOrUpdateTableRow(new[] { "22", "8", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "23", "8", "6", "Note attachment", "tblNoteAttachment" });
            CreateOrUpdateTableRow(new[] { "24", "7", "6", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "25", "45", null, "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "26", "9", "6", "Note attachment", "tblNoteAttachment" });
            CreateOrUpdateTableRow(new[] { "27", "9", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "28", "3", "8", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "29", "39", "7", "Note attachment", "tblNoteAttachment" });
            CreateOrUpdateTableRow(new[] { "30", "39", "7", "Application documents", "tblCredentialApplicationAttachment" });

            CreateOrUpdateTableRow(new[] { "31", "8", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "32", "17", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "33", "36", "2", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "34", "28", "9", "Professional development activity document", "tblProfessionalDevelopmentActivityAttachment" });
            CreateOrUpdateTableRow(new[] { "35", "28", "9", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "36", "32", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "37", "24", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "38", "44", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "39", "6", "2", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "40", "35", null, "Test material document", "tblMaterialRequestRoundAttachment" });

            CreateOrUpdateTableRow(new[] { "41", "35", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "42", "42", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "43", "47", "5", "Material request round document", "tblMaterialRequestRoundAttachment" });
            CreateOrUpdateTableRow(new[] { "44", "5", "2", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "45", "12", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "46", "11", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "47", "41", "1", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "48", "2", "2", "Test documents", "tblTestSittingDocument" });
            CreateOrUpdateTableRow(new[] { "49", "27", "3", "Work practice activity document", "tblWorkPracticeAttachment" });
            CreateOrUpdateTableRow(new[] { "50", "27", "1", "Application documents", "tblCredentialApplicationAttachment" });

            CreateOrUpdateTableRow(new[] { "51", "38", "3", "Person Documents", "tblPersonAttachment" });
            CreateOrUpdateTableRow(new[] { "52", "10", "1", "Application documents", "tblCredentialApplicationAttachment" });

            // TFS 205856
            CreateOrUpdateTableRow(new[] { "53", "16", "6", "Application documents", "tblCredentialApplicationAttachment" });
            CreateOrUpdateTableRow(new[] { "54", "37", "6", "Test documents", "tblTestSittingDocument" });

            // These can be replaced with CreateOrUpdate if new entries need to be added
            DeleteTableRow("55");
        }
    }
}
