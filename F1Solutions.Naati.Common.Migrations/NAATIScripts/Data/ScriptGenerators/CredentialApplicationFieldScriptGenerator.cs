using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationFieldScriptGenerator : BaseScriptGenerator
    {

        public CredentialApplicationFieldScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblCredentialApplicationField";
        public override IList<string> Columns => new[] {
            "CredentialApplicationFieldId",
            "CredentialApplicationTypeId",
            "Reference",
            "Name",
            "Section",
            "DataTypeId",
            "DefaultValue",
            "PerCredentialRequest",
            "Description",
            "Mandatory",
            "DisplayOrder",
            "Reportable",
            "Disabled",
            "CredentialApplicationFieldCategoryId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", null, "Transition All Credentials", "Credentials (mandatory)", "1", null, "0", "Does the applicant wish to transition all of the current credentials?", "1", null, "1","0", "1" });
            CreateOrUpdateTableRow(new[] { "2", "1", null, "All Credentials Have Expiry Dates", "Credentials (mandatory)", "1", null, "0", "Do all of the credentials have an expiry date?", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "3", "1", null, "Products Claimed", "Products and Online Directory (mandatory)", "1", null, "0", "Is the applicant claiming all products that they are entitled to?", "1", null, "1", "0", "2" });
            CreateOrUpdateTableRow(new[] { "4", "1", null, "List in Online Directory", "Products and Online Directory (mandatory)", "1", null, "0", "Does the applicant wish to be listed on the online directory?", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "5", "1", null, "Reference Letter Provided", "Work Practice Evidence (for credentials without an expiry)", "1", null, "0", "Has the applicant provided a Reference letter from an employer or service provider?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "6", "1", null, "Work Practice Record Provided", "Work Practice Evidence (for credentials without an expiry)", "1", null, "0", "Has the applicant provided a completed work practice record?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "7", "1", null, "Proof Of Income Provided", "Work Practice Evidence (for credentials without an expiry)", "1", null, "0", "Has the applicant provided Proof of Income from an accountant?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "8", "1", null, "Statement Provided", "Work Practice Evidence (for credentials without an expiry)", "1", null, "0", "If the applicant did not provide any work practice evidence, did they supply a brief statement about why this was not attached?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "9", "1", null, "Reference Letter Provided", "Chuchotage (for Professional Interpreters only)", "1", null, "0", "Has the applicant provided a Reference letter from an employer or service provider for evidence of chuchotage?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "10", "1", null, "Evidence of Training/Study Provided", "Chuchotage (for Professional Interpreters only)", "1", null, "0", "Has the applicant provided evidence of training/formal unit of study (as part of a qualification)?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "11", "1", null, "Evidence of Completed PD Provided", "Chuchotage (for Professional Interpreters only)", "1", null, "0", "Has the applicant provided evidence of a completed professional development session?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "12", "1", null, "Award Certified Provisional Interpreter", "Chuchotage (for Professional Interpreters only)", "1", null, "0", "Does the applicant wish to be awarded Certified Provisional Interpreter during the interim period (until Professional Development evidence is received by NAATI)?", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "13", "2", null, "Training", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "14", "2", null, "Auslan Proficiency", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "15", "2", null, "English Proficiency", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "16", "2", null, "Ethical Competency", "Ethics and Intercultural", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "17", "2", null, "Intercultural Competency", "Ethics and Intercultural", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "18", "2", null, "To Work in the Industry", "Reason for NAATI Credential (Mandatory)", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "19", "2", null, "Skills Assessment", "Reason for NAATI Credential (Mandatory)", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "20", "2", null, "Obtain CCL Points", "Reason for NAATI Credential (Mandatory)", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "21", "2", null, "Endorsed Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "22", "2", null, "Non Endorsed Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "23", "2", null, "Approved Course", "Qualifications", "1", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "24", "2", null, "Qualification Name", "Endorsed Qualification Details", "2", null, "0", "", "0", "7", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "25", "2", null, "Institution Name", "Endorsed Qualification Details", "2", null, "0", "", "0", "6", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "26", "2", null, "Student ID", "Endorsed Qualification Details", "2", null, "0", "", "0", "8", "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "27", "2", null, "Qualification Start Date", "Endorsed Qualification Details", "8", null, "0", "", "0", "4", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "28", "2", null, "Qualification Completion Date", "Endorsed Qualification Details", "9", null, "0", "", "0", "5", "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "29", "2", null, "More than 3 Years", "Endorsed Qualification Details", "1", null, "0", "", "0", "9", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "30", "2", null, "AIIC", "Membership (for Certified Advanced Translator and Certified Conference Interpreter only)", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "31", "2", null, "AITC", "Membership (for Certified Advanced Translator and Certified Conference Interpreter only)", "1", null, "0", "`", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "32", "3", null, "Obtain CCL Points", "General", "1", null, "0", "", "1", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "33", "3", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "34", "3", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "35", "3", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "36", "3", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "37", "2", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "38", "2", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "39", "2", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "40", "2", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "41", "2", null, "Qualification Name", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "42", "2", null, "Institution", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "43", "2", null, "Qualification Completion Date", "Non-Endorsed Qualification Details", "3", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "44", "2", null, "Student ID", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "45", "2", null, "Qualification Country", "Non-Endorsed Qualification Details", "4", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "46", "2", null, "Recognised Work Practice", "Requirements", "1", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "47", "2", null, "Non Endorsed Advanced Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "48", "6", null, "Endorsed Qualification", "Qualifications", "1", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "49", "6", null, "Higher Level", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "50", "6", null, "New Skill", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "51", "6", null, "Work Practice (higher level)", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "52", "6", null, "Work Practice (new skill)", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "53", "6", null, "Training", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "54", "6", null, "Professional Development", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "55", "6", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "56", "6", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "57", "6", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "58", "6", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "59", "7", null, "Obtain CCL Points", "General", "1", null, "0", "", "1", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "60", "7", null, "Payment Option", "General", "5", null, "0", "", "0", null, "0", "0", "1" });


            CreateOrUpdateTableRow(new[] { "61", "8", null, "Evidence of Professional Development", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "62", "8", null, "Evidence of Work Practice", "Requirements", "1", null, "0", "", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "63", "8", null, "ID Card", "Products", "1", null, "0", "", "0", null, "1", "0", "2" });
            CreateOrUpdateTableRow(new[] { "64", "8", null, "Stamp", "Products", "1", null, "0", "", "0", null, "1", "0", "2" });

            CreateOrUpdateTableRow(new[] { "65", "9", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "66", "9", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "67", "9", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "68", "9", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "69", "8", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "70", "8", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "71", "8", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "72", "8", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });
			//-------------------------------------
			CreateOrUpdateTableRow(new[] { "73", "10", null, "Ethical Competency", "Ethics and Intercultural", "1", null, "0", "", "0", null, "1", "0", "1" });
			CreateOrUpdateTableRow(new[] { "74", "10", null, "Intercultural Competency", "Ethics and Intercultural", "1", null, "0", "", "0", null, "1", "0", "1" });

			CreateOrUpdateTableRow(new[] { "75", "10", null, "Endorsed Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });
			CreateOrUpdateTableRow(new[] { "76", "10", null, "Non Endorsed Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });
			CreateOrUpdateTableRow(new[] { "77", "10", null, "Non Endorsed Advanced Qualification", "Qualifications", "1", null, "0", "", "0", null, "1", "0", "1" });
			CreateOrUpdateTableRow(new[] { "78", "10", null, "Approved Course", "Qualifications", "1", null, "0", "", "0", null, "0", "0", "1" });

			CreateOrUpdateTableRow(new[] { "79", "10", null, "Qualification Name", "Endorsed Qualification Details", "2", null, "0", "", "0", "7", "1", "1", "1" });
			CreateOrUpdateTableRow(new[] { "80", "10", null, "Institution Name", "Endorsed Qualification Details", "2", null, "0", "", "0", "6", "1", "1", "1" });
			CreateOrUpdateTableRow(new[] { "81", "10", null, "Student ID", "Endorsed Qualification Details", "2", null, "0", "", "0", "8", "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "82", "10", null, "Qualification Start Date", "Endorsed Qualification Details", "8", null, "0", "", "0", "4", "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "83", "10", null, "Qualification Completion Date", "Endorsed Qualification Details", "9", null, "0", "", "0", "5", "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "84", "10", null, "More than 3 Years", "Endorsed Qualification Details", "1", null, "0", "", "0", "9", "0", "0", "1" });

			CreateOrUpdateTableRow(new[] { "85", "10", null, "Qualification Name", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "86", "10", null, "Institution", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "87", "10", null, "Student ID", "Non-Endorsed Qualification Details", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "88", "10", null, "Qualification Completion Date", "Non-Endorsed Qualification Details", "3", null, "0", "", "0", null, "0", "0", "1" });

			CreateOrUpdateTableRow(new[] { "89", "10", null, "Employer Name", "Employment", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "90", "10", null, "Contact Person", "Employment", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "91", "10", null, "Contact Person Email", "Employment", "6", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "92", "10", null, "Employment Start Date", "Employment", "3", null, "0", "", "0", null, "0", "0", "1" });

			CreateOrUpdateTableRow(new[] { "93", "10", null, "Sponsored Application", "Sponsor Organisation", "1", null, "0", "", "1", null, "1", "0", "1" });
			CreateOrUpdateTableRow(new[] { "94", "10", null, "Organisation Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "95", "10", null, "Contact Person Name", "Sponsor Organisation", "2", null, "0", "", "0", null, "0", "0", "1" });
			CreateOrUpdateTableRow(new[] { "96", "10", null, "Contact Person Email", "Sponsor Organisation", "6", null, "0", "", "0", null, "0", "0", "1" });
          
            CreateOrUpdateTableRow(new[] { "97", "6", null, "Qualification Name", "Endorsed Qualification Details", "2", null, "0", null, "0", "7", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "98", "6", null, "Institution Name", "Endorsed Qualification Details", "2", null, "0", null, "0", "6", "1", "1", "1" });
            CreateOrUpdateTableRow(new[] { "99", "6", null, "Student ID", "Endorsed Qualification Details", "2", null, "0", null, "0", "8", "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "100", "6", null, "Qualification Start Date", "Endorsed Qualification Details", "8", null, "0", null, "0", "4", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "101", "6", null, "Qualification Completion Date", "Endorsed Qualification Details", "9", null, "0", null, "0", "5", "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "102", "6", null, "More than 3 Years", "Endorsed Qualification Details", "1", null, "0", null, "0", "9", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "103", "6", null, "AIIC", "Membership (for Certified Advanced Translator and Certified Conference Interpreter only)", "1", null, "0", null, "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "104", "6", null, "AITC", "Membership (for Certified Advanced Translator and Certified Conference Interpreter only)", "1", null, "0", "`", "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "105", "6", null, "Bachelor Degree in Health", "Membership (for Certified Specialist Interpreter - Health only)", "1", null, "0", null, "0", null, "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "106", "6", null, "Bachelor Degree in Law", "Membership (for Certified Specialist Interpreter - Legal only)", "1", null, "0", null, "0", null, "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "107", "13", null, "Obtain CCL Points", "General", "1", null, "0", "", "1", null, "0", "0", "1" });

            CreateOrUpdateTableRow(new[] { "108", "2", null, "Qualification", "Endorsed Qualification Details", "7", null, "0", null, "0", "3", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "109", "6", null, "Qualification", "Endorsed Qualification Details", "7", null, "0", null, "0", "3", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "110", "10", null, "Qualification", "Endorsed Qualification Details", "7", null, "0", null, "0", "3", "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "111", "2", null, "Institution", "Endorsed Qualification Details", "10", null, "0", null, "0", "1", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "112", "6", null, "Institution", "Endorsed Qualification Details", "10", null, "0", null, "0", "1", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "113", "10", null, "Institution", "Endorsed Qualification Details", "10", null, "0", null, "0", "1", "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "114", "2", null, "Location", "Endorsed Qualification Details", "11", null, "0", null, "0", "2", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "115", "6", null, "Location", "Endorsed Qualification Details", "11", null, "0", null, "0", "2", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "116", "10", null, "Location", "Endorsed Qualification Details", "11", null, "0", null, "0", "2", "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "117", "2", null, "EndorsedQualificationId", "Endorsed Qualification Details", "12", null, "0", null, "0", "10", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "118", "6", null, "EndorsedQualificationId", "Endorsed Qualification Details", "12", null, "0", null, "0", "10", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "119", "10", null, "EndorsedQualificationId", "Endorsed Qualification Details", "12", null, "0", null, "0", "10", "1", "0", "1" });

            CreateOrUpdateTableRow(new[] { "120", "2", null, "Preferred Form of Written", "Additional Information", "13", null, "0", null, "0", "0", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "121", "6", null, "Preferred Form of Written", "Additional Information", "13", null, "0", null, "0", "0", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "122", "10", null, "Preferred Form of Written", "Additional Information", "13", null, "0", null, "0", "0", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "123", "17", null, "Preferred Form of Written", "Additional Information", "13", null, "0", null, "0", "0", "1", "0", "1" });
        }
    }
}
