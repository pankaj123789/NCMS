using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    class ProfessionalDevelopmentRequirementScriptGenerator: BaseScriptGenerator
    {
        public ProfessionalDevelopmentRequirementScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentRequirement";
        public override IList<string> Columns => new[] {
            "ProfessionalDevelopmentRequirementId",
            "Name",
            "DisplayName"
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Over 20 hours", "Over 20 hours" });
            CreateOrUpdateTableRow(new[] { "2", "Over one day", "Over one day" });
            CreateOrUpdateTableRow(new[] { "3", "Over 4 hours", "Over 4 hours" });
            CreateOrUpdateTableRow(new[] { "4", "Between 1 and 4 hours", "Between 1 and 4 hours" });
            CreateOrUpdateTableRow(new[] { "5", "2-4 hours", "2-4 hours" });
            CreateOrUpdateTableRow(new[] { "6", "Maximum of 10 points per recertification application", "Maximum of 10 points per recertification application" });
            CreateOrUpdateTableRow(new[] { "7", "2 hours minimum", "2 hours minimum" });
            CreateOrUpdateTableRow(new[] { "8", "6 hours minimum", "6 hours minimum" });
            CreateOrUpdateTableRow(new[] { "9", "Maximum of 10 points per year", "Maximum of 10 points per year" });
            CreateOrUpdateTableRow(new[] { "10", "Maximum of 10 points per activity", "Maximum of 10 points per activity" });
            CreateOrUpdateTableRow(new[] { "11", "2000 words minimum", "2000 words minimum" });
            CreateOrUpdateTableRow(new[] { "12", "Maximum of 20 points per recertification application", "Maximum of 20 points per recertification application" });
            CreateOrUpdateTableRow(new[] { "13", "20 minutes minimum", "20 minutes minimum" });
            CreateOrUpdateTableRow(new[] { "14", "1 hour minimum", "1 hour minimum" });
            CreateOrUpdateTableRow(new[] { "15", "20 points per semester", "20 points per semester" });
            CreateOrUpdateTableRow(new[] { "16", "1 semester or more", "1 semester or more" });
            CreateOrUpdateTableRow(new[] { "17", "At least 1 semester, working the equivalent of 10 hours per week", "At least 1 semester, working the equivalent of 10 hours per week" });
            CreateOrUpdateTableRow(new[] { "18", "Up to 3 hours", "Up to 3 hours" });
            CreateOrUpdateTableRow(new[] { "19", "Over 3 hours", "Over 3 hours" });
            CreateOrUpdateTableRow(new[] { "20", "Involved in setting a minimum of 1 test task per recertification period", "Involved in setting a minimum of 1 test task per recertification period" });
            CreateOrUpdateTableRow(new[] { "21", "Productive input posted at least twice per year", "Productive input posted at least twice per year" });
            CreateOrUpdateTableRow(new[] { "22", "Maximum of 10 points per period", "Maximum of 10 points per period" });
            CreateOrUpdateTableRow(new[] { "23", "Must have received acknowledgement of your participation via email or pos", "Must have received acknowledgement of your participation via email or pos" });
            CreateOrUpdateTableRow(new[] { "24", "Written evidence (either a statutory declaration by a peer, certificate of attendance or a written report of about 700 words) must be provided with the log book", "Written evidence (either a statutory declaration by a peer, certificate of attendance or a written report of about 700 words) must be provided with the log book" });
            CreateOrUpdateTableRow(new[] { "25", "10 to 20 hours", "10 to 20 hours" });
            CreateOrUpdateTableRow(new[] { "26", "More than 20 hours", "More than 20 hours" });
            CreateOrUpdateTableRow(new[] { "27", "50,000-70,000 words translated/reviewed or 200-250 hours/assignments as an interpreter", "50,000-70,000 words translated/reviewed or 200-250 hours/assignments as an interpreter" });
            CreateOrUpdateTableRow(new[] { "28", "70,000 + words translated / reviewed or  250 + hours / assignments as an interpreter", "70,000 + words translated / reviewed or  250 + hours / assignments as an interpreter" });
            CreateOrUpdateTableRow(new[] { "29", "1 school term", "1 school term" });
            CreateOrUpdateTableRow(new[] { "30", "1 to 4 weeks", "1 to 4 weeks" });
            CreateOrUpdateTableRow(new[] { "31", "Over 4 weeks", "Over 4 weeks" });
            CreateOrUpdateTableRow(new[] { "32", "1 year minimum", "1 year minimum" });
            CreateOrUpdateTableRow(new[] { "33", "1 day minimum", "1 day minimum" });
            CreateOrUpdateTableRow(new[] { "34", "2 or more events during the recertification period", "2 or more events during the recertification period" });
            CreateOrUpdateTableRow(new[] { "35", "Two days or more", "Two days or more" });
            CreateOrUpdateTableRow(new[] { "36", "Completed Qualification", "Completed Qualification" });
            CreateOrUpdateTableRow(new[] { "37", "One full day", "One full day" });
        }
    }
}
