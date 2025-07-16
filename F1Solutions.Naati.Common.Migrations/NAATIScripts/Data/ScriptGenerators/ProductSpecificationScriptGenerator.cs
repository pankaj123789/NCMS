using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class ProductSpecificationScriptGenerator : BaseScriptGenerator
    {
        public ProductSpecificationScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblProductSpecification";
        public override IList<string> Columns => new[] {
                                                           "ProductSpecificationId",
                                                           "ProductCategoryId",
                                                           "Name",
                                                           "Description",
                                                           "Code",
                                                           "CostPerUnit",
                                                           "GSTApplies",
                                                           "GlCodeId",
                                                           "BatchQuantity",
                                                           "Inactive",
                                                           "JobTypeId",
                                                           "TrackingActivity"
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] {"1", "1", "Transition Application Fee", "Transition Application Fee", "TRANSASS", "121", "1", "30", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"2", "1", "Recognised Assessment Fee", "Recognised Assessment Fee", "RECOGASS", "198", "1", "29", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"3", "1", "Recertification Assessment Fee", "Recertification Assessment Fee", "RECERT", "220", "1", "32", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"4", "2", "Certified Translator Test Fee", "Certified Translator Test Fee", "CERTTRANS", "605", "1", "7", "0", "0", "1", null});

            CreateOrUpdateTableRow(new[] {"6", "2", "Certified Conference Interpreter Test Fee", "Certified Conference Interpreter Test Fee", "CERTCONINT", "913", "1", "1", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"7", "2", "Certified Advanced Translator Test Fee", "Certified Advanced Translator Test Fee", "CERTADVTRAN", "792", "1", "2", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"8", "2", "Certified Interpreter Test Fee", "Certified Interpreter Test Fee", "CERTINT", "440", "1", "6", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"9", "2", "Certified Specialist Interpreter Test Fee", "Certified Specialist Interpreter Test Fee", "CERTSPECINT", "220", "1", "5", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"10", "2", "Certified Specialist Health Interpreter - Test Fee", "Certified Specialist Health Interpreter - Test Fee", "CERTSPECINTHLTH", "220", "1", "3", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"11", "2", "Certified Specialist Legal Interpreter - Test Fee", "Certified Specialist Legal Interpreter - Test Fee", "CERTSPECINTLGL", "220", "1", "4", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"12", "2", "Certified Provisional Interpreter Test Fee", "Certified Provisional Interpreter Test Fee", "CERTPROVINT", "660", "1", "8", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"13", "2", "Ethical Competency Test Fee", "Ethical Competency Test Fee", "PREETH", "242", "1", "49", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"14", "2", "Intercultural Competency Test Fee", "Intercultural Competency Test Fee", "PREINT", "242", "1", "50", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"15", "2", "CCL Test Fee", "CCL Test Fee", "CCL", "814", "1", "41", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"16", "5", "NAATI Certificate", "NAATI Certificate", "CERTIFICATE", "99", "1", "62", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"17", "5", "ID Card", "ID Card", "IDCRD", "88", "1", "63", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"18", "5", "Translator Stamp", "Translator Stamp", "TRNSTAMP", "220", "1", "64", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"19", "4", "Conference Interpreting Principles and Pratice", "Conference Interpreting Principles and Pratice", "CONFINTPP", "52", "1", "47", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"20", "4", "The Self Employed Translators Guide(USB)", "The Self Employed Translators Guide(USB)", "SEMPLGDE", "40", "1", "47", "0", "0", "1", null});

            CreateOrUpdateTableRow(new[] {"21", "6", "Ethical Competency Marking", "Ethical Competency Marking", "EC MARK", "85", "0", "60", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] {"22", "6", "Intercultural Marking", "Intercultural Marking", "IC MARK", "85", "0", "59", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] {"23", "6", "Certified Provisional Interpreter Marking", "Certified Provisional Interpreter Marking", "CPI MARK", "260", "0", "55", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"24", "6", "Certified Interpreter Marking", "Certified Interpreter Marking", "CI MARK", "270", "0", "51", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"25", "6", "Certified Specialist Legal Interpreter - Marking", "Certified Specialist Legal Interpreter - Marking", "CSLI MARK", "210", "0", "52", "0", "0", "1", "CER-TSTMRK"});

            CreateOrUpdateTableRow(new[] {"26", "6", "Certified Specialist Health Interpreter - Marking", "Certified Specialist Health Interpreter - Marking", "CSHI MARK", "210", "0", "52", "0", "0", "1", "CER-TSTMRK"});

            CreateOrUpdateTableRow(new[] {"27", "6", "Certified Conference Interpreter Marking", "Certified Conference Interpreter Marking", "CCI MARK", "315", "0", "53", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"28", "6", "Certified Translator Marking", "Certified Translator Marking", "CT MARK", "210", "0", "54", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"29", "6", "Certified Advanced Translator Marking", "Certified Advanced Translator Marking", "CAT MARK", "230", "0", "56", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"30", "6", "Community Credential Language Marking", "Community Credential Language Marking", "CCL MARK", "60", "0", "58", "0", "0", "1", "CCL-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"31", "7", "Ethical Competency Test Marking Review", "Ethical Competency Test Marking Review", "EC REVIEW", "85", "0", "60", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"32", "7", "Intercultural Competency Test Marking Review", "Intercultural Competency Test Marking Review", "IC REVIEW", "85", "0", "59", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"33", "7", "Certified Provisional Interpreter Review", "Certified Provisional Interpreter Review", "CPI REVIEW", "260", "0", "55", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"34", "7", "Certified Interpreter Review", "Certified Interpreter Review", "CI REVIEW", "270", "0", "51", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"35", "7", "Certified Specialist Legal Interpreter - Review", "Certified Specialist Legal Interpreter - Review", "CSLI REVIEW", "210", "0", "52", "0", "0", "1", "CER-TSTMRK"});

            CreateOrUpdateTableRow(new[] {"36", "7", "Certified Specialist Health Interpreter - Review", "Certified Specialist Health Interpreter - Review", "CSHI REVIEW", "210", "0", "52", "0", "0", "1", "CER-TSTMRK"});

            CreateOrUpdateTableRow(new[] {"37", "7", "Certified Conference Interpreter Review", "Certified Conference Interpreter Review", "CCI REVIEW", "315", "0", "53", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"38", "7", "Certified Translator Review", "Certified Translator Review", "CT REVIEW", "210", "0", "54", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"39", "7", "Certified Advanced Translator Review", "Certified Advanced Translator Review", "CAT REVIEW", "230", "0", "56", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"40", "7", "Community Credential Language Marking Review", "Community Credential Language Marking Review", "CCL REVIEW", "60", "0", "58", "0", "0", "1", "CCL-TSTMRK" });
            CreateOrUpdateTableRow(new[] {"41", "10", "Community Credential Language Test Review Fee", "Community Credential Language Test Review Fee", "CCLREV", "187", "1", "41", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"42", "2", "Community Credential Language Test Fee", "Community Credential Language Test Fee", "CCLAPP", "814", "1", "41", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"43", "9", "Certified Translator Test Resit Fee", "Certified Translator Test Resit Fee", "CERTTRANSRSIT", "220", "1", "7", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"44", "9", "Certified Conference Interpreter Test Resit Fee", "Certified Conference Interpreter Test Resit Fee", "CERTCONINTRSIT", "220", "1", "1", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"45", "9", "Certified Advanced Translator Test Resit Fee", "Certified Advanced Translator Test Resit Fee", "CERTADVTRANRSIT", "220", "1", "2", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"46", "9", "Certified Interpreter Test Resit Fee", "Certified Interpreter Test Resit Fee", "CERTINTRSIT", "220", "1", "6", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"47", "9", "Certified Specialist Interpreter Test Resit Fee", "Certified Specialist Interpreter Test Resit Fee", "CERTSPECINTRSIT", "220", "1", "5", "1", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"48", "9", "Certified Specialist Health Interpreter - Test Resit Fee", "Certified Specialist Health Interpreter - Test Resit Fee", "CERTSPECINTHLTHRSIT", "220", "1", "3", "0", "0", "1", "NULL"});

            CreateOrUpdateTableRow(new[] {"49", "9", "Certified Specialist Legal Interpreter - Test Resit Fee", "Certified Specialist Legal Interpreter - Test Resit Fee", "CERTSPECINTLGLRSIT", "220", "1", "4", "0", "0", "1", "NULL"});

            CreateOrUpdateTableRow(new[] {"50", "9", "Certified Provisional Interpreter Test Resit Fee", "Certified Provisional Interpreter Test Resit Fee", "CERTPROVINTRSIT", "220", "1", "8", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"51", "8", "NAATI Endorsed Qualification Assessment Fee", "NAATI Endorsed Qualification Assessment Fee - Initial", "ENDORQUALFEE", "2000", "1", "31", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"52", "2", "Community Language Aide Test Fee", "Community Language Aide Test Fee", "CLA", "440", "1", "42", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"53", "10", "Community Language Aide Review Fee", "Community Language Aide Review Fee", "CLAREV", "110", "1", "42", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"54", "6", "Community Language Aide Marking", "Community Language Aide Marking", "CLA MARK", "85", "0", "57", "0", "0", "1", "CLA" });
            CreateOrUpdateTableRow(new[] {"55", "7", "Community Language Aide Marking Review", "Community Language Aide Marking Review", "CLA REVIEW", "85", "0", "57", "0", "0", "1", "CLA"});
            CreateOrUpdateTableRow(new[] {"56", "10", "Ethical Competency Review Fee", "Ethical Competency Review Fee", "PREETHREV", "44", "1", "49", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"57", "10", "Intercultural Competency Review Fee", "Intercultural Competency Review Fee", "PREINTREV", "44", "1", "50", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"58", "10", "Certified Translator Review Fee", "Certified Translator Review Fee", "CERTTRANSREV", "275", "1", "7", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"59", "10", "Certified Conference Interpreter Review Fee", "Certified Conference Interpreter Review Fee", "CERTCONINTREV", "275", "1", "1", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"60", "10", "Certified Advanced Translator Review Fee", "Certified Advanced Translator Review Fee", "CERTADVTRANREV", "275", "1", "2", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"61", "10", "Certified Interpreter Review Fee", "Certified Interpreter Review Fee", "CERTINTREV", "275", "1", "6", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"62", "10", "Certified Specialist Interpreter Review Fee", "Certified Specialist Interpreter Review Fee", "CERTSPECINTREV", "110", "1", "5", "0", "0", "1", "NULL"});

            CreateOrUpdateTableRow(new[] {"63", "10", "Certified Specialist Health Interpreter - Review Fee", "Certified Specialist Health Interpreter - Review Fee", "CERTSPECINTHLTHREV", "110", "1", "3", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"64", "10", "Certified Specialist Legal Interpreter - Review Fee", "Certified Specialist Legal Interpreter - Review Fee", "CERTSPECINTLGLREV", "110", "1", "4", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"65", "10", "Certified Provisional Interpreter Review Fee", "Certified Provisional Interpreter Review Fee", "CERTPROVINTREV", "275", "1", "8", "0", "0", "1", null});
            CreateOrUpdateTableRow(new[] {"66", "11", "CCL Test Development", "CCL Test Development", "CCLTESTDEV", "85", "0", "58", "0", "0", "1", "CCL-TSTDEV" });
            CreateOrUpdateTableRow(new[] {"67", "11", "Cert. Interpreter Test Development", "Cert. Interpreter Test Development", "CERTESTDEV", "85", "0", "51", "0", "0", "1", "CER-TSTDEV" });
            CreateOrUpdateTableRow(new[] {"68", "11", "Cert. Translator Test Development", "Cert. Translator Test Development", "CERTESTDEV", "85", "0", "54", "0", "0", "1", "CER-TSTDEV"});
            CreateOrUpdateTableRow(new[] {"69", "11", "Cert. Provisional Interpreter Test Development", "Cert. Provisional Interpreter Test Development", "CERTESTDEV", "85", "0", "55", "0", "0", "1", "CER-TSTDEV"});
            CreateOrUpdateTableRow(new[] {"70", "11", "Cert. Conference Interpreter Test Development", "Cert. Conference Interpreter Test Development", "CERTESTDEV", "115", "0", "53", "0", "0", "1", "CER-TSTDEV"});
            CreateOrUpdateTableRow(new[] {"71", "11", "Cert. Advanced Translator Test Development", "Cert. Advanced Translator Test Development", "CERTESTDEV", "60", "0", "56", "0", "0", "1", "CER-TSTDEV"});
            CreateOrUpdateTableRow(new[] {"72", "11", "CLA Test Development", "CLA Test Development", "CLA", "85", "0", "57", "0", "0", "1", "CLA"});

            CreateOrUpdateTableRow(new[] {"78", "6", "Certified Specialist Legal Interpreter Knowledge Test – Marking", "Certified Specialist Legal Interpreter Knowledge Test – Marking", "CSLI KT MARK", "150", "0", "52", "0", "0", "1", "CER-TSTMRK"});
            CreateOrUpdateTableRow(new[] {"79", "10", "Certified Specialist Legal Interpreter Knowledge Test – Review", "Certified Specialist Legal Interpreter Knowledge Test – Review", "SPECINTKNOWLEDGEREV", "110", "1", "4", "0", "1", "1", "NULL" });
            CreateOrUpdateTableRow(new[] {"80", "6", "Certified Specialist Health Interpreter Knowledge Test – Marking", "Certified Specialist Health Interpreter Knowledge Test – Marking", "CSHI KT MARK", "150", "0", "52", "0", "0", "1", "CER-TSTMRK"});

            CreateOrUpdateTableRow(new[] {"81", "10", "Certified Specialist Health Interpreter Knowledge Test – Review", "Certified Specialist Health Interpreter Knowledge Test – Review", "SPECINTKNOWLEDGEREV", "110", "1", "3", "0", "1", "1", "NULL" });

            CreateOrUpdateTableRow(new[] {"82", "11", "IC Test Development", "IC Test Development", "ICTESTDEV", "85", "0", "59", "0", "0", "1", "CER-TSTDEV" });
            CreateOrUpdateTableRow(new[] {"83", "11", "EC Test Development", "EC Test Development", "ECTESTDEV", "85", "0", "60", "0", "0", "1", "CER-TSTDEV" });

            CreateOrUpdateTableRow(new[] {"84", "11", "Cert. Specialist Interpreter Test Development", "Cert. Specialist Interpreter Test Development", "CERTESTDEV", "95", "0", "52", "0", "0", "1", "CER-TSTDEV" });
            CreateOrUpdateTableRow(new[] {"85", "8", "Para-professional Interpreter Application Fee - Resident", "Para-professional Interpreter Application Fee - Resident", "AUSCOURSE", "391", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"86", "8", "Para-professional Interpreter Application Fee - Non-Resident", "Para-professional Interpreter Application Fee - Non-Resident", "AUSCOURSE", "529", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"87", "8", "Professional Translator Application Fee - Resident", "Professional Translator Application Fee - Resident", "AUSCOURSE", "391", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"88", "8", "Professional Translator Application Fee - Non-Resident", "Professional Translator Application Fee - Non-Resident", "AUSCOURSE", "529", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"89", "8", "Professional Interpreter Application Fee - Resident", "Professional Interpreter Application Fee - Resident", "AUSCOURSE", "391", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"90", "8", "Professional Interpreter Application Fee - Non-Resident", "Professional Interpreter Application Fee - Non-Resident", "AUSCOURSE", "529", "1", "61", "0", "0", "1", "NULL"});
            CreateOrUpdateTableRow(new[] {"91", "2", "Certified Specialist Legal Interpreter Knowledge Test Fee", "Certified Specialist Legal Interpreter Knowledge Test Fee", "CERTSPECINTLGLKT", "198", "1", "4", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] {"92", "2", "Certified Specialist Health Interpreter Knowledge Test Fee", "Certified Specialist Health Interpreter Knowledge Test Fee", "CERTSPECINTHLTHKT", "198", "1", "3", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] {"93", "3", "Re-issue Letter", "Re-issue Letter", "REISSUELETTER", "44", "1", "39", "0", "0", "1", "NULL" });

            CreateOrUpdateTableRow(new[] { "94", "2", "Certified Conference Interpreter Auslan - Test Fee", "Certified Conference Interpreter Auslan - Test Fee", "CERTCONINT", "913", "1", "1", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] { "95", "2", "Certified Interpreter Auslan - Test Fee", "Certified Interpreter Auslan - Test Fee", "CERTINT", "990", "1", "6", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] { "96", "2", "Certified Specialist Health Interpreter Auslan - Test Fee", "Certified Specialist Health Interpreter Auslan - Test Fee", "CERTSPECINTHLTH", "220", "1", "3", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] { "97", "2", "Certified Specialist Legal Interpreter Auslan - Test Fee", "Certified Specialist Legal Interpreter Auslan - Test Fee", "CERTSPECINTLGL", "220", "1", "4", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] { "98", "2", "Certified Provisional Interpreter Auslan - Test Fee", "Certified Provisional Interpreter Auslan - Test Fee", "CERTPROVINT", "660", "1", "8", "0", "0", "1", "NULL" });
            CreateOrUpdateTableRow(new[] { "99", "2", "CCL Practice Test Fee", "Community Credential Language Practice Test Fee", "PRACTICECCL", "165", "1", "41", "0", "0", "1", "NULL" });

            CreateOrUpdateTableRow(new[] { "100", "2", "CT Practice Test Fee", "Practice Test Fee - Certified Translator", "PRACTICECT", "165", "1", "7", "0", "0", "1", "" });
            CreateOrUpdateTableRow(new[] { "101", "2", "CI Practice Test Fee", "Practice Test Fee - Certified Interpreter", "PRACTICECI", "165", "1", "6", "0", "0", "1", "" });

            CreateOrUpdateTableRow(new[] { "102", "6", "CCL Practice Test - marking", "CCL Practice Test - marking", "CCL PRAC MARK", "60", "0", "58", "0", "0", "1", "CCL-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "103", "6", "CT Practice Test - marking", "CT Practice Test - marking", "CT PRAC MARK", "85", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "104", "6", "CI Practice Test - marking", "CI Practice Test - marking", "CI PRAC MARK", "130", "0", "51", "0", "0", "1", "CER-TSTMRK" });

            CreateOrUpdateTableRow(new[] { "105", "6", "Certified Translator Test - Supplementary Marking", "CT Supplementary marking", "CT SUPP MARK", "85", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "106", "6", "Certified Provisional Interpreter Test - Supplementary Marking", "CPI Supplementary marking", "CPI SUPP MARK", "85", "0", "55", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "107", "6", "Certified Interpreter Test - Supplementary Marking 1 Task", "CI Supplementary marking - 1 task", "CI SUPP MARK 1", "85", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "108", "6", "Certified Interpreter Test - Supplementary Marking 2 Tasks", "CI Supplementary marking - 2 tasks", "CI SUPP MARK 2", "140", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "109", "6", "Third marking - 1 task CPI", "Third marking - 1 task CPI", "CPI 3RD MARK 1", "85", "0", "55", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "110", "6", "Third marking - 1 task CI", "Third marking - 1 task CI", "CI 3RD MARK 1", "85", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "111", "6", "Third marking - 1 tasks CSI", "Third marking - 1 task CSI", "CSI 3RD MARK 1", "95", "0", "52", "0", "0", "1", "CER-TSTMRK" });

            CreateOrUpdateTableRow(new[] { "112", "6", "Third marking - 1 task CT", "Third marking - 1 task CT", "CT 3RD MARK 1", "85", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "113", "6", "Third marking - 2 tasks CI", "Third marking - 2 tasks CI", "CI 3RD MARK 2", "140", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "114", "6", "Third marking - 2 tasks CSI", "Third marking - 2 tasks CSI", "CSI 3RD MARK 2", "140", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "115", "6", "Third marking - 3 tasks CI", "Third marking - 3 tasks CI", "CI 3RD MARK 3", "190", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "116", "6", "Third marking - 3 tasks CSI", "Third marking - 3 tasks CI", "CSI 3RD MARK 3", "190", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "117", "6", "CCI third marking - 1 task", "CCI third marking - 1 task", "CCI 3RD MARK 1", "115", "0", "53", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "118", "6", "CCI third marking - 2 tasks", "Third marking - 2 tasks CT", "CCI 3RD MARK 2", "230", "0", "53", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "119", "6", "Third marking - 2 tasks CT", "Third marking - 2 tasks CT", "CT 3RD MARK 2", "170", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "120", "6", "Third marking - 2 tasks CPI", "Third marking - 2 tasks CPI", "CPI 3RD MARK 2", "170", "0", "55", "0", "0", "1", "CER-TSTMRK" });

            CreateOrUpdateTableRow(new[] { "121", "7", "Paid review - 1 task CPI", "Paid review - 1 task", "CPI PAID REVIEW 1", "85", "0", "55", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "122", "7", "Paid review - 1 task CI", "Paid review - 1 task", "CI PAID REVIEW 1", "85", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "123", "7", "Paid review - 1 task CSI", "Paid review - 1 task", "CSI PAID REVIEW 1", "95", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "124", "7", "Paid review - 1 task CT", "Paid review - 1 task", "CT PAID REVIEW 1", "85", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "125", "7", "Paid review - 2 tasks CI", "Paid review - 2 tasks CI", "CI PAID REVIEW 2", "140", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "126", "7", "Paid review - 2 tasks CSI", "Paid review - 2 tasks CSI", "CSI PAID REVIEW 2", "140", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "127", "7", "Paid review - 2 tasks CPI", "Paid review - 2 tasks CPI", "CPI PAID REVIEW 2", "170", "0", "55", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "128", "7", "Paid review - 2 tasks CT", "Paid review - 2 tasks CT", "CT PAID REVIEW 2", "170", "0", "54", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "129", "7", "Paid review - 3 tasks CI", "Paid review - 3 tasks CI", "CI PAID REVIEW 3", "190", "0", "51", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "130", "7", "Paid review - 3 tasks CSI", "Paid review - 3 tasks CSI", "CSI PAID REVIEW 3", "190", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "131", "7", "CCI paid review - 1 task", "CCI paid review - 1 task", "CCI PAID REVIEW 1", "115", "0", "53", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "132", "7", "CCI paid review - 2 tasks", "CCI paid review - 2 tasks", "CCI PAID REVIEW 2", "230", "0", "53", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "133", "8", "NAATI Endorsed Qualification Assessment Fee - Annual", "NAATI Endorsed Qualification Assessment Fee - Annual", "ENDORQUALFEEANNUAL", "1500", "1", "31", "0", "0", "1", null });
            CreateOrUpdateTableRow(new[] { "134", "12", "Scholarship – NAATI funded", "Scholarship – NAATI funded", "SCHOLARSHIPNAATIFUND", "0", "0", "65", "0", "0", "1", null });
            CreateOrUpdateTableRow(new[] { "135", "6", "CSI Supplementary marking", "CSI Supplementary marking", "CSI SUPP MARK", "95", "0", "52", "0", "0", "1", "CER-TSTMRK" });
            CreateOrUpdateTableRow(new[] { "136", "6", "CCI Supplementary marking", "CCI Supplementary marking", "CCI SUPP MARK", "140", "0", "53", "0", "0", "1", "CER-TSTMRK" });

        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("5");
        }
    }
}