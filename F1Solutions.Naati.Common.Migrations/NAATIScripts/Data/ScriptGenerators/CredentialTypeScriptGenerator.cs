using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialType";

        public override IList<string> Columns => new[]
        {
            "CredentialTypeId",
            "CredentialCategoryId",
            "InternalName",
            "ExternalName",
            "DisplayOrder",
            "Simultaneous",
            "SkillTypeId",
            "Certification",
            "DefaultExpiry",
            "AllowBackDating",
            "Level",
            "TestSessionBookingAvailabilityWeeks",
            "TestSessionBookingClosedWeeks",
            "TestSessionBookingRejectHours",
            "AllowAvailabilityNotice"
        };

        public override void RunScripts()
        {
            ChangeColumValue("DisplayOrder");
            CreateOrUpdateTableRow(new[] { "1", "1", "Recognised Practising Translator", "Recognised Practising Translator", "1", "0", "1", "1", null, "0", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "2", "1", "Certified Translator", "Certified Translator", "3", "0", "2", "1", null, "0", "3", "156", "2", "192", "1" });
            CreateOrUpdateTableRow(new[] { "3", "1", "Certified Advanced Translator", "Certified Advanced Translator", "4", "1", "3", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "4", "1", "Certified Advanced Translator LOTE to LOTE", "Certified Advanced Translator", "5", "1", "4", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "5", "2", "Recognised Practising Interpreter", "Recognised Practising Interpreter", "8", "0", "5", "1", null, "0", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "6", "2", "Certified Provisional Interpreter", "Certified Provisional Interpreter", "11", "0", "6", "1", null, "0", "2", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "7", "2", "Certified Interpreter", "Certified Interpreter", "14", "1", "7", "1", null, "0", "3", "156", "2", "192", "1" });
            CreateOrUpdateTableRow(new[] { "8", "2", "Certified Specialist Health Interpreter ", "Certified Specialist Health Interpreter ", "17", "1", "9", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "9", "2", "Certified Specialist Legal Interpreter", "Certified Specialist Legal Interpreter", "18", "1", "10", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "10", "2", "Certified Conference Interpreter", "Certified Conference Interpreter", "21", "1", "8", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "11", "2", "Certified Conference Interpreter LOTE to LOTE", "Certified Conference Interpreter", "23", "1", "11", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "12", "3", "Recognised Practising Deaf Interpreter", "Recognised Practising Deaf Interpreter", "6", "0", "13", "1", null, "0", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "13", "3", "Certified Provisional Deaf Interpreter", "Certified Provisional Deaf Interpreter", "7", "0", "12", "1", null, "0", "2", "156", "5", "192", "1" });
            //dont change
            CreateOrUpdateTableRow(new[] { "14", "4", "CCL", "Credentialed Community Language Test", "28", "0", "14", "0", "5", "0", "0", "78", "1", "192", "0" });
            CreateOrUpdateTableRow(new[] { "15", "5", "CLA", "Community Language Aide", "29", "0", "15", "0", "5", "0", "0", "52", "1", "192", "0" });
            //dont change
            CreateOrUpdateTableRow(new[] { "16", "6", "Ethics", "Ethical Competency Test", "24", "0", "16", "0", "3", "0", "0", "156", "1", "192", "1" });
            CreateOrUpdateTableRow(new[] { "17", "7", "Intercultural", "Intercultural Competency Test", "25", "0", "17", "0", "3", "0", "0", "156", "1", "192", "1" });
            CreateOrUpdateTableRow(new[] { "18", "8", "Migration", "Migration Assessment", "30", "0", "18", "0", null, "0", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "19", "2", "Recognised Practising Interpreter - Auslan", "Recognised Practising Interpreter", "9", "0", "19", "1", null, "0", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "20", "2", "Certified Provisional Interpreter - Auslan", "Certified Provisional Interpreter", "12", "0", "20", "1", null, "0", "2", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "21", "2", "Certified Interpreter - Auslan", "Certified Interpreter", "15", "1", "21", "1", null, "0", "3", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "22", "2", "Certified Specialist Health Interpreter - Auslan", "Certified Specialist Health Interpreter ", "19", "1", "23", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "23", "2", "Certified Specialist Legal Interpreter - Auslan", "Certified Specialist Legal Interpreter", "20", "1", "24", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "24", "2", "Certified Conference Interpreter - Auslan", "Certified Conference Interpreter", "22", "1", "22", "1", null, "0", "4", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "25", "2", "Recognised Practising Interpreter - Indigenous", "Recognised Practising Interpreter", "10", "0", "25", "1", null, "1", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "26", "2", "Certified Provisional Interpreter - Indigenous", "Certified Provisional Interpreter", "13", "0", "26", "1", null, "1", "2", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "27", "2", "Certified Interpreter - Indigenous", "Certified Interpreter", "16", "0", "27", "1", null, "1", "3", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "28", "1", "Recognised Practising Translator - Indigenous", "Recognised Practising Translator", "2", "0", "28", "1", null, "0", "1", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "29", "6", "Indigenous Ethics", "Ethical Competency Test", "26", "0", "29", "0", "3", "1", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "30", "7", "Indigenous Intercultural", "Intercultural Competency Test", "27", "0", "30", "0", "3", "1", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "31", "9", "CSLI - Knowledge", "Specialist Legal Interpreter Knowledge Test", "31", "0", "31", "0", "3", "0", "0", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "32", "10", "CSHI - Knowledge", "Specialist Health Interpreter Knowledge Test", "32", "0", "32", "0", "3", "0", "0", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "33", "11", "Paraprofessional Interpreter", "Paraprofessional Interpreter", "33", "0", "33", "0", "3", "0", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "34", "11", "Professional Interpreter", "Professional Interpreter", "34", "0", "34", "0", "3", "0", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "35", "11", "Professional Translator", "Professional Translator", "35", "0", "35", "0", "3", "0", "0", "156", "5", "192", "0" });
            CreateOrUpdateTableRow(new[] { "36", "2", "Cert. Conference Interpreter – English into Auslan", "Cert. Conference Interpreter – English into Auslan", "36", "1", "36", "1", null, "4", "0", "156", "5", "192", "1" });
            CreateOrUpdateTableRow(new[] { "37", "2", "Cert. Conference Interpreter – Auslan into English", "Cert. Conference Interpreter – Auslan into English", "37", "1", "37", "1", null, "4", "0", "156", "5", "192", "1" });
            //practice test
            CreateOrUpdateTableRow(new[] { "38", "12", "CCL Practice Test", "Practice Test - Credentialed Community Language ", "38", "0", "38", "0", "5", "0", "0", "78", "2", "192", "0" });
            CreateOrUpdateTableRow(new[] { "39", "12", "CT Practice Test", "Practice Test - Certified Translator", "39", "0", "39", "1", null, "0", "3", "156", "2", "192", "1" });
            CreateOrUpdateTableRow(new[] { "40", "12", "CI Practice Test", "Practice Test - Certified Interpreter", "40", "1", "40", "1", null, "0", "3", "156", "2", "192", "1" });

        }
    }
}