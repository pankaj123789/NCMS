using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SkillTypeScriptGenerator : BaseScriptGenerator
    {
        public override string TableName => "tblSkillType";

        public override IList<string> Columns => new[]
        {
            "SkillTypeId",
            "Name",
            "DisplayName"
        };

        public SkillTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "RPT", "Recognised Practising Translator" });
            CreateOrUpdateTableRow(new[] { "2", "CT", "Certified Translator" });
            CreateOrUpdateTableRow(new[] { "3", "CAT", "Certified Advanced Translator" });
            CreateOrUpdateTableRow(new[] { "4", "CTLOTE", "Certified Advanced Translator LOTE to LOTE" });
            CreateOrUpdateTableRow(new[] { "5", "RPI", "Recognised Practising Interpreter" });
            CreateOrUpdateTableRow(new[] { "6", "CPI", "Certified Provisional Interpreter" });
            CreateOrUpdateTableRow(new[] { "7", "CI", "Certified Interpreter" });
            CreateOrUpdateTableRow(new[] { "8", "CCI", "Certified Conference Interpreter" });
            CreateOrUpdateTableRow(new[] { "9", "CSHI", "Certified Specialist Health Interpreter" });
            CreateOrUpdateTableRow(new[] { "10", "CSLI", "Certified Specialist Legal Interpreter " });
            CreateOrUpdateTableRow(new[] { "11", "CCILOTE", "Certified Conference Interpreter LOTE to LOTE" });
            CreateOrUpdateTableRow(new[] { "12", "CPDI", "Certified Provisional Deaf Interpreter" });
            CreateOrUpdateTableRow(new[] { "13", "RPDI", "Recognised Practising Deaf Interpreter" });
            CreateOrUpdateTableRow(new[] { "14", "CCL", "Credentialed Community Language" });
            CreateOrUpdateTableRow(new[] { "15", "CLA", "Community Language Aide" });
            CreateOrUpdateTableRow(new[] { "16", "Ethics", "Ethics" });
            CreateOrUpdateTableRow(new[] { "17", "Intercultural", "Intercultural" });
            CreateOrUpdateTableRow(new[] { "18", "Migration", "Migration Assessment" });
            CreateOrUpdateTableRow(new[] { "19", "RPI-AUSLAN", "Recognised Practising Interpreter - Auslan" });
            CreateOrUpdateTableRow(new[] { "20", "CPI-AUSLAN", "Certified Provisional Interpreter - Auslan" });
            CreateOrUpdateTableRow(new[] { "21", "CI-AUSLAN", "Certified Interpreter - Auslan" });
            CreateOrUpdateTableRow(new[] { "22", "CCI-AUSLAN", "Certified Conference Interpreter - Auslan" });
            CreateOrUpdateTableRow(new[] { "23", "CSIH-AUSLAN", "Certified Specialist Interpreter - Health - Auslan" });
            CreateOrUpdateTableRow(new[] { "24", "CSIL-AUSLAN", "Certified Specialist Interpreter - Legal - Auslan" });
            CreateOrUpdateTableRow(new[] { "25", "RPI-INDIGENOUS", "Recognised Practising Interpreter - Indigenous" });
            CreateOrUpdateTableRow(new[] { "26", "CPI-INDIGENOUS", "Certified Provisional Interpreter - Indigenous" });
            CreateOrUpdateTableRow(new[] { "27", "CI-INDIGENOUS", "Certified Interpreter - Indigenous" });
            CreateOrUpdateTableRow(new[] { "28", "RPT-INDIGENOUS", "Recognised Practising Translator - Indigenous" });
            CreateOrUpdateTableRow(new[] { "29", "Ethics-Indigenous", "Indigenous Ethics" });
            CreateOrUpdateTableRow(new[] { "30", "Intercultural-Indigenous", "Indigenous Intercultural" });
            CreateOrUpdateTableRow(new[] { "31", "CSLI-Knowledge", "Specialist Legal Interpreter Knowledge Test" });
            CreateOrUpdateTableRow(new[] { "32", "CSHI-Knowledge", "Specialist Health Interpreter Knowledge Test" });
            CreateOrUpdateTableRow(new[] { "33", "Para-ProfessionalInterpreter", "Para-professional Interpreter" });
            CreateOrUpdateTableRow(new[] { "34", "Professional Interpreter", "Professional Interpreter" });
            CreateOrUpdateTableRow(new[] { "35", "Professional Translator", "Professional Translator" });
            CreateOrUpdateTableRow(new[] { "36", "Cert. Conference Interpreter – English into Auslan", "Cert. Conference Interpreter – English into Auslan" });
            CreateOrUpdateTableRow(new[] { "37", "Cert. Conference Interpreter – Auslan into English", "Cert. Conference Interpreter – Auslan into English" });
            CreateOrUpdateTableRow(new[] { "38", "CCL Practice Test", "Practice Test - Credentialed Community Language" });
            CreateOrUpdateTableRow(new[] { "39", "CT Practice Test", "Practice Test - Certified Translator" });
            CreateOrUpdateTableRow(new[] { "40", "CI Practice Test", "Practice Test - Certified Interpreter" });
        }
    }
}
