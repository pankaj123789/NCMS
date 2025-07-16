using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class GlCodeScriptGenerator : BaseScriptGenerator
    {
        public GlCodeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblGlCode";
        public override IList<string> Columns => new[] {
                                                           "GlCodeId",
                                                           "Code",
                                                           "AccountName",
                                                           "Description",
                                                           "ExternalReferenceAccountId"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "43131", "Certified Conference Interpreter", "", "1290a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "2", "43132", "Certified Advanced Translator", "", "1390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "3", "43133", "Specialist interpreter - Health", "", "1490a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "4", "43134", "Specialist interpreter - Legal", "", "1590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "5", "43135", "Specialist interpreter", "", "1690a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "6", "43136", "Certified Interpreter", "", "1790a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "7", "43137", "Certified Translator", "", "1890a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "8", "43138", "Certified Provisional Interpreter", "", "1990a77a-9c7f-eb11-b855-000d3a6a4ff0" });
           
            CreateOrUpdateTableRow(new[] { "10", "43141", "Certified Conference Interpreter", "", "1c90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "11", "43142", "Certified Advanced Translator NZ", "", "1d90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "12", "43143", "Specialist interpreter - Health NZ", "", "1e90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "13", "43144", "Specialist interpreter - Legal NZ", "", "1f90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "14", "43145", "Specialist interpreter NZ", "", "2090a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "15", "43146", "Certified Interpreter NZ", "", "2190a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "16", "43147", "Certified Translator NZ", "", "2290a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "17", "43148", "Certified Provisional Interpreter", "", "2390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
           
            CreateOrUpdateTableRow(new[] { "19", "43151", "Certified Conference Interp OS", "", "2590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "20", "43152", "Certified Advanced Translator OS", "", "2690a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "21", "43153", "Specialist interpreter - Health OS", "", "2790a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "22", "43154", "Specialist interpreter - Legal OS", "", "2890a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "23", "43155", "Specialist interpreter OS", "", "2990a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "24", "43156", "Certified Interpreter OS", "", "2a90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "25", "43157", "Certified Translator OS", "", "2b90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "26", "43158", "Certified Provisional Interpreter", "", "2c90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            
            CreateOrUpdateTableRow(new[] { "28", "43161", "Viewings - Translation test", "", "a3387b4a-b465-eb11-bb53-000d3a398e0a" }); //missing in Wiise
            CreateOrUpdateTableRow(new[] { "29", "43201", "Application Recognised PC", "", "3190a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "30", "43202", "Application for transition", "", "3290a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "31", "43203", "Application for endorsed Qualification status", "", "3390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "32", "43204", "Application for Re-cert", "", "3490a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "33", "44110", "Approval Application Fees", "", "5290a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "34", "44111", "Re-Approval Application Fees", "", "5390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "35", "44112", "Course Monitoring Fees", "", "5490a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "36", "44113", "Other Course Approval", "", "5590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "37", "44114", "Approved Course Mgmnt Workshop", "", "4090a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "38", "44410", "Fee Adjustments", "", "5290a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "39", "44411", "Re-issue Letter", "", "5390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "40", "44412", "Reshedule Fees", "", "5490a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "41", "47211", "CCL revenue", "", "6590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "42", "47212", "Community language aide", "", "6690a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "43", "42301", "Certificate", "", "a3387b4a-b465-eb11-bb53-000d3a398e0a" });//missing in Wiise
            CreateOrUpdateTableRow(new[] { "44", "42302", "ID Cards", "", "a3387b4a-b465-eb11-bb53-000d3a398e0a" });//missing in Wiise
            CreateOrUpdateTableRow(new[] { "45", "42303", "Stamps", "", "a3387b4a-b465-eb11-bb53-000d3a398e0a" });//missing in Wiise
            CreateOrUpdateTableRow(new[] { "46", "43111", "Practice Tests", "", "0a90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "47", "44211", "Publications", "", "4890a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "48", "44413", "Retained Fees", "", "5590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "49", "43122", "Ethical Competency", "", "0f90a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "50", "43123", "Intercultural Competency", "", "1090a77a-9c7f-eb11-b855-000d3a6a4ff0" });

            CreateOrUpdateTableRow(new[] { "51", "61113", "Examiner and contract payments - CI", "", "577f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "52", "61114", "Examiner and contract payments - CSI", "", "5f7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "53", "61115", "Examiner and contract payments - CCI", "", "6c7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "54", "61116", "Examiner and contract payments - CT", "", "797f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "55", "61117", "Examiner and contract payments - CPI", "", "867f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "56", "61118", "Examiner and contract payments - CAT", "", "8a7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "57", "61119", "Examiner and contract payments - CLA", "", "8c7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "58", "61120", "Examiner and contract payments - CCL", "", "8e7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "59", "61121", "Screening tests - Intercultural competency", "", "957f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "60", "61122", "Screening tests - Ethical competency", "", "9a7f0dde-a47f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "61", "42212", "Aus Course Assessments", "", "3a90a77a-9c7f-eb11-b855-000d3a6a4ff0" });

            CreateOrUpdateTableRow(new[] { "62", "44213", "Certificates", "", "4390a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "63", "44214", "ID Cards", "", "4490a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "64", "42215", "Stamps", "", "4590a77a-9c7f-eb11-b855-000d3a6a4ff0" });
            CreateOrUpdateTableRow(new[] { "65", "43301", "Scholarship – NAATI funded", "", "6b251340-9745-f011-be59-000d3a6a361d" });

        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("9");
            DeleteTableRow("18");
            DeleteTableRow("27");
        }
    }
}