using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class VenueScriptGenerator : BaseScriptGenerator
    {
        public VenueScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }
        public override string TableName => "tblVenue";

        public override IList<string> Columns => new[]
        {
            "VenueId",
            "TestLocationId",
            "Address",
            "Capacity",
            "Name",
            "PublicNotes",
            "Inactive",
            "Coordinates"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1", "280-282 Pitt Street, Sydney NSW 2000", "6", "NAATI NSW Office (Translation)", "Suite 3, Level 5", "0", null });
            CreateOrUpdateTableRow(new[] { "2", "3", "600 Lonsdale Street, Melbourne VIC 3000", "16", "NAATI VIC Office", "Lonsdale Court Suite 14, Level 1", "0", null });
            CreateOrUpdateTableRow(new[] { "3", "2", "Unit 16, 2 King Street Deakin ACT 2600", "10", "NAATI National Office", "", "0", null });
            CreateOrUpdateTableRow(new[] { "4", "2", "84 Northbourne Avenue, Canberra ACT 2612", "50", "Mantra Hotel on Northbourne", "Go to reception and ask for the NAATI test room.", "0", null });
            CreateOrUpdateTableRow(new[] { "5", "1", "280-282 Pitt Street, Sydney NSW 2000", "3", "NAATI NSW Office (Interpreting)", "Suite 3, Level 5", "0", null });
            CreateOrUpdateTableRow(new[] { "6", "1", "Level 1, 280-282 Pitt Street, Sydney NSW 2000", "45", "Sydney Mechanics School of Arts Building (CCL Testing)", "", "0", null });
            CreateOrUpdateTableRow(new[] { "7", "1", "Level 1, 111 Harrington Street, Sydney NSW 2000", "45", "Karstens (CCL Testing)", "", "0", null });
            CreateOrUpdateTableRow(new[] { "8", "1", "431-439 Pitt Street, Sydney NSW 2000", "30", "Metro Hotel Marlow Sydney Central", "Metro Hotel Marlow Funtion Room", "0", null });
            CreateOrUpdateTableRow(new[] { "9", "1", "280-282 Pitt Street, Sydney NSW 2000", "8", "NAATI NSW Office (Prerequisite testing)", "Suite 3, Level 5", "0", null });
            CreateOrUpdateTableRow(new[] { "10", "7", "Unit 3, 50 Oxford Close, West Leederville WA 6007", "8", "NAATI WA Office", "", "0", null });
            CreateOrUpdateTableRow(new[] { "11", "7", "34 Harrogate Street, West Leederville WA 6007", "22", "The Royal Australian College of General Practitioners - WA Faculty", "Ring the doorbell and open after hearing the \"click\"", "0", null });
            CreateOrUpdateTableRow(new[] { "12", "5", "McDougall Building, 9 Ellerslie Road, Battery Point TAS 7004", "10", "NAATI TAS Office", "McDougall Building Level 1. Entrance from Ellerslie Rd", "0", "-42.888680,147.327745" });
            CreateOrUpdateTableRow(new[] { "13", "3", "123 Queen Street, Melbourne VIC 3000", "56", "Karstens", "Check the screen at reception to confirm Level", "0", null });
            CreateOrUpdateTableRow(new[] { "14", "3", "600 Lonsdale Street, Melbourne VIC 3000", "30", "MTM Mediation Centre", "Lonsdale Court Suite 9, Level 1", "0", null });
            CreateOrUpdateTableRow(new[] { "15", "6", "Level 8, 170 North Terrace, Adelaide SA 5000", "8", "NAATI SA Office", "", "0", null });
            CreateOrUpdateTableRow(new[] { "16", "4", "Room 10, 2nd Floor, 102 Main Street, Kangaroo Point, Brisbane QLD 4169", "20", "NAATI QLD Office", "", "0", null });
            CreateOrUpdateTableRow(new[] { "17", "1", "Level 13/60 Margaret Street, Sydney NSW 2000", "45", "Cliftons", "Please check at reception for the location of NAATI registration", "0", null });
            CreateOrUpdateTableRow(new[] { "18", "7", "191 St Georges Terrace, Perth WA 6000", "10", "Cliftons", "Parmelia House", "0", null });
            CreateOrUpdateTableRow(new[] { "19", "3", "555 Lonsdale Street, Melbourne VIC 3000", "30", "Monash University Law Chambers", "", "0", null });
            CreateOrUpdateTableRow(new[] { "20", "6", "97 Pirie Street, Adelaide SA 5000", "12", "Adelaide Meeting Room Hire", "Level 5", "0", null });
            CreateOrUpdateTableRow(new[] { "21", "2", "4 National Circuit, Barton ACT 2600", "20", "Dialogue", "", "0", null });
            CreateOrUpdateTableRow(new[] { "22", "1", "207 Kent Street, Sydney NSW 2000", "56", "The Portside Centre, Level 5, Symantec House", "Please note, on arrival you are on the Ground Floor which is Level 6. To access Level 5, you need to select the DOWN lift button.", "0", null });
            CreateOrUpdateTableRow(new[] { "23", "6", "80 King William Street, Adelaide SA 5000", "50", "Cliftons", "Level 1", "0", null });
            CreateOrUpdateTableRow(new[] { "24", "8", "85 Smith Street, Darwin City NT 0800", "6", "RCG House", "Ground Floor", "0", null });
            CreateOrUpdateTableRow(new[] { "25", "6", "38 Gawler Place, Adelaide SA 5000", "50", "Express Training Connections", "Level 5, Edments Building", "0", null });
            CreateOrUpdateTableRow(new[] { "26", "7", "108 St Georges Terrace, Perth WA 6000", "10", "Executive Centre", "Reception on Level 25", "0", null });
            CreateOrUpdateTableRow(new[] { "27", "5", "410 Sandy Bay Road, Sandy Bay TAS 7005", "60", "Wrest Point Exhibition Centre", "", "0", null });
        }
    }
}
