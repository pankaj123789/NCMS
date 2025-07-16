using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    class ProfessionalDevelopmentCategoryScriptGenerator:BaseScriptGenerator
    {
        public ProfessionalDevelopmentCategoryScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblProfessionalDevelopmentCategory";
        public override IList<string> Columns => new[] {
            "ProfessionalDevelopmentCategoryId",
            "Name",
            "Description",
            "ProfessionalDevelopmentCategoryGroupId",
        };
        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "1.1", "<p>Completed a formal course unit or module at a tertiary education institution on translating or interpreting or a related field</p>", null });
            CreateOrUpdateTableRow(new[] { "2", "1.2", "<p>Attended a formal professional development workshop, training module or short course at a tertiary education institution on translating and interpreting or a related field</p>", null });
            CreateOrUpdateTableRow(new[] { "3", "1.3", "<p>Attended a formal professional development workshop, training module or short course on the specialisation area of the practitioner (e.g. legal, medical)</p>", null });
            CreateOrUpdateTableRow(new[] { "4", "1.4", "<p>Attended a formal professional development session, workshop, seminar, conference or webinar conducted by a professional body, NAATI, RTO, university department or language service provider – either in translating or interpreting or a related field</p>", null });
            CreateOrUpdateTableRow(new[] { "5", "1.5", "<p>Attended a formal training session, workshop or seminar relating to:</p><ul> <li> Business management </li><li> Personal development</li><li> Family violence, access and equity or other issues impacting on culturally and linguistically diverse communities </li><li> Computer literacy </li><li> Information and communications technology </li><li> Computer aided translation tools</li> </ul>", null });
            CreateOrUpdateTableRow(new[] { "6", "1.6", "<p>Attended a translating or interpreting networking session, forum, awareness day, professional association branch meeting/ National AGM or service provider developmental team meetings.</p>", null });
            CreateOrUpdateTableRow(new[] { "7", "1.7", "<p>Attended a translating or interpreting industry-related employer/workplace induction and orientation</p>", null });
            CreateOrUpdateTableRow(new[] { "8", "1.8", "<p>Received mentoring as part of an official mentoring program for interpreters or translators</p>", null });
            CreateOrUpdateTableRow(new[] { "9", "1.9", "<p>Current paid membership of a translating or interpreting professional association or representative body (e.g. AUSIT, ASLIA, WAITI, Professionals Australia, AALITRA or CITAA)</p>", null });
            CreateOrUpdateTableRow(new[] { "10", "1.10", "<p>Undergone membership renewal process with professional association or representative body(e.g. AUSIT, ASLIA, WAITI, Professionals Australia, AALITRA or CITAA) which included professional development requirements</p>", null });
            CreateOrUpdateTableRow(new[] { "11", "1.11", "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion.) e.g. Acquisition of another language</p>", null });
            CreateOrUpdateTableRow(new[] { "12", "2.1", "<p>Published an article on a Translating or Interpreting subject matter in a refereed translation, interpreting or linguistics industry journal, book or relevant peer-reviewed journal</p>", null });
            CreateOrUpdateTableRow(new[] { "13", "2.2", "<p>Developed and published resource materials related to translating or interpreting, such as:</p><li>Dictionaries </li><li> Thesauri </li><li> Course materials </li><li> Specialist glossaries </li><li> Multimedia content (audio, video, digital, etc) </li><li> Editing a peer-reviewed journal</li>", null });
            CreateOrUpdateTableRow(new[] { "14", "2.3", "<p>Presented a paper at a translating or interpreting or related discipline conference</p>", null });
            CreateOrUpdateTableRow(new[] { "15", "2.4", "<p>Presented at a translating or interpreting workshop (either for T & I practitioners or for other professional cohorts who work with interpreters)</p>", null });
            CreateOrUpdateTableRow(new[] { "16", "2.5", "<p>Taught translating or interpreting in a formal course that may result in an AQF qualification, for one semester or more. You can only claim limited workload (e.g. 2 hrs per week for 12 weeks = 24 hours)</p>", null });
            CreateOrUpdateTableRow(new[] { "17", "2.6", "<p>Taught a translating or interpreting course at tertiary level for one semester or more (Covers full-time employment or maximum teaching workload)</p>", null });
            CreateOrUpdateTableRow(new[] { "18", "2.7", "<p>Taught a unit(s) as part of a NAATI endorsed qualification</p>", null });
            CreateOrUpdateTableRow(new[] { "19", "2.8", "<p>Shared practical experience through mentoring new practitioners or supervising work experience students</p>", null });
            CreateOrUpdateTableRow(new[] { "20", "2.9", "<p>Served as a NAATI examiner during the recertification period and attended a NAATI examiner training workshop</p>", null });
            CreateOrUpdateTableRow(new[] { "21", "2.10", "<p>Contributed to the setting of NAATI Certification tests</p>", null });
            CreateOrUpdateTableRow(new[] { "22", "2.11", "<p>Made regular contributions as part of an AUSIT, ASLIA, WAITI, AALC, Professionals Australia or NAATI committee, sub-committee or conference organising committee</p>", null });
            CreateOrUpdateTableRow(new[] { "23", "2.12", "<p>Actively participated in the ASLIA, AUSIT, WAITI, AALC, NAATI, Professionals Australia or other translating or interpreting language online forum</p>", null });
            CreateOrUpdateTableRow(new[] { "24", "2.13", "<p>Held active membership and participated in a LOTE cultural club or similar body, or contribution to LOTE or specialist media (e.g. community newsletter, radio, community television)</p>", null });
            CreateOrUpdateTableRow(new[] { "25", "2.14", "<p>Published an article on the AUSIT, ASLIA, WAITI, AALC, Professionals Australia or NAATI website or in an industry related newsletter or magazine</p>", null });
            CreateOrUpdateTableRow(new[] { "26", "2.15", "<p>Attended a relevant industry information session or completion of a survey(or research activity) associated with the profession or certification which was advertised by NAATI as carrying PD points</p>", null });
            CreateOrUpdateTableRow(new[] { "27", "2.16", "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion)</p>", null });
            CreateOrUpdateTableRow(new[] { "28", "2.17", "<p>Taught ethics for translators and interpreters as part of a formal course that meets Australian Quality Training Framework (AQTF) criteria</p>", "1" });
            CreateOrUpdateTableRow(new[] { "29", "2.18", "<p>Facilitated a training event (e.g. refresher, workshop, seminar, webinar, etc.) for a professional body, NAATI or language service provider specifically concerned with ethics of the profession</p>", "1" });
            CreateOrUpdateTableRow(new[] { "30", "2.19", "<p>Completed regular self-directed, or group, learning activities such as writing or reading articles on ethics, engaging in ethics debates or other professional development activities with a high ethical emphasis</p>", "1" });

            
            CreateOrUpdateTableRow(new[] { "32", "2.20", "<p>Attended a training event (e.g. refresher, workshop, seminar, webinar, etc.) run by a professional body, tertiary institution, NAATI or language service provider specifically concerned with ethics of the profession. This includes completion of online courses</p>", "1" });
            CreateOrUpdateTableRow(new[] { "33", "2.21", "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion)</p>", "1" });
            CreateOrUpdateTableRow(new[] { "34", "3.1", "<p>Completed a formal professional development workshop, course unit or module at a tertiary institution on LOTE, English, Auslan/Deaf studies or linguistics</p>", null });
            CreateOrUpdateTableRow(new[] { "35", "3.2", "<p>Completed work practice in excess of NAATI’s recertification requirements</p>", null });
            CreateOrUpdateTableRow(new[] { "36", "3.3", "<p>Taught a LOTE unit or course at a tertiary institute or community language school</p>s", null });
            CreateOrUpdateTableRow(new[] { "37", "3.4", "<p>Spent time in a county or region where LOTE is the primary language spoken <br />NOTE: This can only be claimed by practitioners living in a country where the primary language is English. Shorter stays can be combined and claimed at the end of the recertification period</p>", null });
            CreateOrUpdateTableRow(new[] { "38", "3.5", "<p>Spent time in a country or region where English is the primary language <br />NOTE: This can only be claimed by practitioners living in a country where the primary language is the LOTE. Shorter stays can be combined and claimed at the end of the recertification period</p>", null });
            CreateOrUpdateTableRow(new[] { "39", "3.6", "<p>Subscribed to LOTE websites, newsgroups, newspapers, magazines, periodicals or television channels</p>", null });
            CreateOrUpdateTableRow(new[] { "40", "3.7", "<p>Subscribed to Deaf community websites, newsgroups, newspapers, magazines, journals, periodicals, etc that are written in English or have content in Auslan, such as:</p> <li>World Federation of the Deaf News </li><li> Deaf Australia Outlook </li><li> British Deaf News </li><li> Journal of Deaf Studies & Deaf Education </li><li> Deaf Worlds: International </li><li> Sign Language Studies</li>", null });
            CreateOrUpdateTableRow(new[] { "41", "3.8", "<p>Completed private advance language (English or LOTE/Auslan (for Auslan practitioners)) study at an organisation recognised as providing expert language tuition (e.g. Alliance Française)</p>", null });
            CreateOrUpdateTableRow(new[] { "42", "3.9", "<p>Attended an Auslan workshop held by recognised organisations providing expert language tuition. <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>", null });
            CreateOrUpdateTableRow(new[] { "43", "3.10", "<p>Participated in networking, community or other relevant events run by an established community organisation or professional body where the primary language used in participation is the LOTE (or Auslan for Auslan practitioners)</p>", null });
            CreateOrUpdateTableRow(new[] { "44", "3.11", "<p>Attended an ASLIA networking event or Deaf community events if held in Auslan <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>", null });
            CreateOrUpdateTableRow(new[] { "45", "3.12", "<p>Current membership of Deaf Australia <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>", null });
            CreateOrUpdateTableRow(new[] { "46", "3.13", "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation in your logbook to justify its inclusion)</p>", null });
            CreateOrUpdateTableRow(new[] { "47", "Tertiary Qualification", "<p>There is no need to complete activities from categories 1 and 2 if, during the recertification period, the practitioner has completed a translating or interpreting or related discipline (such as Linguistics, TESOL or Deaf Studies) tertiary qualification at the Diploma level or above</p>", "1" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow( "31");
        }
    }
}
