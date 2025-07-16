using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class TestMaterialDomainScriptGenerator : BaseScriptGenerator
    {
        public TestMaterialDomainScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblTestMaterialDomain";
        public override IList<string> Columns => new[] {
            "TestMaterialDomainId",
            "DisplayName"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Unspecified" });
            CreateOrUpdateTableRow(new[] { "2", "Community" });
            CreateOrUpdateTableRow(new[] { "3", "Consumer Affairs" });
            CreateOrUpdateTableRow(new[] { "4", "Education" });
            CreateOrUpdateTableRow(new[] { "5", "Employment" });
            CreateOrUpdateTableRow(new[] { "6", "Finance" });
            CreateOrUpdateTableRow(new[] { "7", "Health" });
            CreateOrUpdateTableRow(new[] { "8", "Housing" });
            CreateOrUpdateTableRow(new[] { "9", "Immigration/Settlement" });
            CreateOrUpdateTableRow(new[] { "10", "Insurance" });

            CreateOrUpdateTableRow(new[] { "11", "Legal" });
            CreateOrUpdateTableRow(new[] { "12", "Social Services" });
            CreateOrUpdateTableRow(new[] { "13", "ASLIA – Confidentiality" });
            CreateOrUpdateTableRow(new[] { "14", "ASLIA – Professional Conduct" });
            CreateOrUpdateTableRow(new[] { "15", "ASLIA – Scope of Practice" });
            CreateOrUpdateTableRow(new[] { "16", "ASLIA – Integrity of Service" });
            CreateOrUpdateTableRow(new[] { "17", "ASLIA – Qualifications to Practice" });
            CreateOrUpdateTableRow(new[] { "18", "ASLIA – Faithfulness of Interpretation" });
            CreateOrUpdateTableRow(new[] { "19", "ASLIA – Accountability for Professional Competence" });
            CreateOrUpdateTableRow(new[] { "20", "ASLIA – Ongoing Professional Development" });

            CreateOrUpdateTableRow(new[] { "21", "ASLIA – Non-Discrimination" });
            CreateOrUpdateTableRow(new[] { "22", "ASLIA – Communication Preferences" });
            CreateOrUpdateTableRow(new[] { "23", "ASLIA – Deaf Interpreters" });
            CreateOrUpdateTableRow(new[] { "24", "ASLIA – Professional Relationships" });
            CreateOrUpdateTableRow(new[] { "25", "ASLIA – Impartiality" });
            CreateOrUpdateTableRow(new[] { "26", "ASLIA – Respect for Colleagues" });
            CreateOrUpdateTableRow(new[] { "27", "ASLIA – Support for Professional Associations" });
            CreateOrUpdateTableRow(new[] { "28", "ASLIA – Business Practices" });
            CreateOrUpdateTableRow(new[] { "29", "ASLIA – Accurate Representation of Credentials" });
            CreateOrUpdateTableRow(new[] { "30", "ASLIA – Reimbursement for Services" });

            CreateOrUpdateTableRow(new[] { "31", "AUSIT – Professional Conduct" });
            CreateOrUpdateTableRow(new[] { "32", "AUSIT – Confidentiality" });
            CreateOrUpdateTableRow(new[] { "33", "AUSIT – Competence" });
            CreateOrUpdateTableRow(new[] { "34", "AUSIT – Impartiality" });
            CreateOrUpdateTableRow(new[] { "35", "AUSIT – Accuracy" });
            CreateOrUpdateTableRow(new[] { "36", "AUSIT – Clarity of Role Boundaries" });
            CreateOrUpdateTableRow(new[] { "37", "AUSIT – Maintaining Professional Relationships" });
            CreateOrUpdateTableRow(new[] { "38", "AUSIT – Professional Development" });
            CreateOrUpdateTableRow(new[] { "39", "AUSIT – Professional Solidarity" });
            CreateOrUpdateTableRow(new[] { "40", "AUSIT – Completeness in interpreting" });
            
            CreateOrUpdateTableRow(new[] { "41", "Business" });
            CreateOrUpdateTableRow(new[] { "42", "Industries" });
            CreateOrUpdateTableRow(new[] { "43", "Culture" });
            CreateOrUpdateTableRow(new[] { "44", "Society" });
            CreateOrUpdateTableRow(new[] { "45", "Environment" });
            CreateOrUpdateTableRow(new[] { "46", "Tourism" });
            CreateOrUpdateTableRow(new[] { "47", "Science" });
            CreateOrUpdateTableRow(new[] { "48", "Technology" });
            CreateOrUpdateTableRow(new[] { "49", "Government" });
            CreateOrUpdateTableRow(new[] { "50", "Diplomacy" });

            CreateOrUpdateTableRow(new[] { "51", "Politics" });
            CreateOrUpdateTableRow(new[] { "52", "Commerce" });
            CreateOrUpdateTableRow(new[] { "53", "Economics" });

            CreateOrUpdateTableRow(new[] { "54", "Intercultural Competency" });
            CreateOrUpdateTableRow(new[] { "69", "Geriatrics" });
            CreateOrUpdateTableRow(new[] { "70", "Obstetrics" });

            CreateOrUpdateTableRow(new[] { "71", "Anaesthesiology" });
            CreateOrUpdateTableRow(new[] { "72", "Surgery" });
            CreateOrUpdateTableRow(new[] { "73", "Ophthalmology" });
            CreateOrUpdateTableRow(new[] { "74", "Paediatrics" });
            CreateOrUpdateTableRow(new[] { "76", "Mental Health" });
            CreateOrUpdateTableRow(new[] { "77", "Oncology" });
            CreateOrUpdateTableRow(new[] { "78", "Alternative/Traditional medicine" });
            CreateOrUpdateTableRow(new[] { "79", "Dementia" });
            CreateOrUpdateTableRow(new[] { "80", "Psychiatry" });

            CreateOrUpdateTableRow(new[] { "81", "Psychology" });
            CreateOrUpdateTableRow(new[] { "82", "Speech Pathology" });
            CreateOrUpdateTableRow(new[] { "83", "Anatomy" });
            CreateOrUpdateTableRow(new[] { "84", "Physiology" });
            CreateOrUpdateTableRow(new[] { "85", "Pharmacology" });
            CreateOrUpdateTableRow(new[] { "86", "Civil law" });
            CreateOrUpdateTableRow(new[] { "87", "Criminal law" });
            CreateOrUpdateTableRow(new[] { "88", "Family law" });
            CreateOrUpdateTableRow(new[] { "89", "Commercial law" });
            CreateOrUpdateTableRow(new[] { "90", "JCCD Guidelines" });

            CreateOrUpdateTableRow(new[] { "91", "Migrant and Refugee working with interpreters’ guidelines" });
            CreateOrUpdateTableRow(new[] { "92", "Mental Health Interpreting Guidelines" });

            CreateOrUpdateTableRow(new[] { "93", "Customer Service" });

            //#180936
            CreateOrUpdateTableRow(new[] { "94", "Linguistics" });
            CreateOrUpdateTableRow(new[] { "95", "Social sciences" });
            CreateOrUpdateTableRow(new[] { "96", "Humanities" });
            CreateOrUpdateTableRow(new[] { "97", "Public policy" });
            CreateOrUpdateTableRow(new[] { "98", "Human rights" });
            CreateOrUpdateTableRow(new[] { "99", "Disability" });
            CreateOrUpdateTableRow(new[] { "100", "Deaf community" });

            CreateOrUpdateTableRow(new[] { "101", "Emergency Briefing" });
        }
    }
}
