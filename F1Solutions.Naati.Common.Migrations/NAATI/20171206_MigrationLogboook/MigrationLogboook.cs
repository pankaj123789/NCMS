
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171206_MigrationLogboook
{
    [NaatiMigration(201712061100)]
    public class MigrationLogboook : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE tblProfessionalDevelopmentRequirement ALTER COLUMN Name nvarchar(200)
                          ALTER TABLE tblProfessionalDevelopmentCategory ALTER COLUMN Description nvarchar(500)");

            if (!Schema.Table("tblProfessionalDevelopmentCategoryGroup").Exists())
            {
                Create.Table("tblProfessionalDevelopmentCategoryGroup")
                    .WithColumn("ProfessionalDevelopmentCategoryGroupId").AsInt32().Identity().PrimaryKey()
                    .WithColumn("Name").AsAnsiString(50).NotNullable()
                    .WithColumn("Description").AsString(200).NotNullable();

                Create.Column("ProfessionalDevelopmentCategoryGroupId")
                    .OnTable("tblProfessionalDevelopmentCategory")
                    .AsInt32()
                    .Nullable();
                Create.ForeignKey("FK_ProfessionalDevelopmentCategory_ProfessionalDevelopmentCategoryGroup")
                    .FromTable("tblProfessionalDevelopmentCategory")
                    .ForeignColumn("ProfessionalDevelopmentCategoryGroupId")
                    .ToTable("tblProfessionalDevelopmentCategoryGroup")
                    .PrimaryColumn("ProfessionalDevelopmentCategoryGroupId");


                Create.Column("PersonId").OnTable("tblProfessionalDevelopmentActivity").AsInt32().NotNullable();

                Create.ForeignKey("FK_ProfessionalDevelopmentActivity_Person")
                    .FromTable("tblProfessionalDevelopmentActivity")
                    .ForeignColumn("PersonId")
                    .ToTable("tblPerson")
                    .PrimaryColumn("PersonId");
            }


            Insert.IntoTable("tblProfessionalDevelopmentSection").Row(new
            {
                Name = "CATEGORY 1: SKILLS DEVELOPMENT AND KNOWLEDGE",
                Description = "CATEGORY 1: SKILLS DEVELOPMENT AND KNOWLEDGE"
            });
            Insert.IntoTable("tblProfessionalDevelopmentSection").Row(new
            {
                Name = "CATEGORY 2: INDUSTRY ENGAGEMENT",
                Description = "CATEGORY 2: INDUSTRY ENGAGEMENT"
            });
            Insert.IntoTable("tblProfessionalDevelopmentSection").Row(new
            {
                Name = "CATEGORY 3: MAINTENANCE OF LANGUAGE",
                Description = "CATEGORY 3: MAINTENANCE OF LANGUAGE"
            });



            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Over 20 hours",
                DisplayName = "Over 20 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Over one day",
                DisplayName = "Over one day"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "Over 4 hours",
                DisplayName = "Over 4 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Between 1 and 4 hours",
                DisplayName = "Between 1 and 4 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "2-4 hours",
                DisplayName = "2-4 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new      
            {
                Name = "Maximum of 10 points per recertification application",
                DisplayName = "Maximum of 10 points per recertification application"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "2 hours minimum",
                DisplayName = "2 hours minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "6 hours minimum",
                DisplayName = "6 hours minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "Maximum of 10 points per year",
                DisplayName = "Maximum of 10 points per year"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Maximum of 10 points per activity",
                DisplayName = "Maximum of 10 points per activity"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "2000 words minimum",
                DisplayName = "2000 words minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new       
            {
                Name = "Maximum of 20 points per recertification application",
                DisplayName = "Maximum of 20 points per recertification application"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "20 minutes minimum",
                DisplayName = "20 minutes minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "1 hour minimum",
                DisplayName = "1 hour minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "20 points per semester",
                DisplayName = "20 points per semester"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "1 semester or more",
                DisplayName = "1 semester or more"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "At least 1 semester, working the equivalent of 10 hours per week",
                DisplayName = "At least 1 semester, working the equivalent of 10 hours per week"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Up to 3 hours",
                DisplayName = "Up to 3 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "Over 3 hours",
                DisplayName = "Over 3 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "Involved in setting a minimum of 1 test task per recertification period",
                DisplayName = "Involved in setting a minimum of 1 test task per recertification period"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Productive messages posted at least twice per year",
                DisplayName = "Productive messages posted at least twice per year"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Maximum of 10 points per period",
                DisplayName = "Maximum of 10 points per period"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Must have received acknowledgement of your participation via email or pos",
                DisplayName = "Must have received acknowledgement of your participation via email or pos"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "Written evidence (either a statutory declaration by a peer, certificate of attendance or a written report of about 700 words) must be provided with the log book",
                DisplayName = "Written evidence (either a statutory declaration by a peer, certificate of attendance or a written report of about 700 words) must be provided with the log book"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "10 to 20 hours",
                DisplayName = "10 to 20 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "More than 20 hours",
                DisplayName = "More than 20 hours"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "50,000-70,000 words translated/reviewed or 200-250 hours/assignments as an interpreter",
                DisplayName = "50,000-70,000 words translated/reviewed or 200-250 hours/assignments as an interpreter"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "70,000 + words translated / reviewed or  250 + hours / assignments as an interpreter",
                DisplayName = "70,000 + words translated / reviewed or  250 + hours / assignments as an interpreter"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "1 school term",
                DisplayName = "1 school term"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "1 to 4 weeks",
                DisplayName = "1 to 4 weeks"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "Over 4 weeks",
                DisplayName = "Over 4 weeks"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "1 year minimum",
                DisplayName = "1 year minimum"
            });
            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new
            {
                Name = "1 day minimum",
                DisplayName = "1 day minimum"
            });

            Insert.IntoTable("tblProfessionalDevelopmentRequirement").Row(new 
            {
                Name = "2 or more events during the recertification period",
                DisplayName = "2 or more events during the recertification period"
            });
            


            Insert.IntoTable("tblProfessionalDevelopmentCategoryGroup").Row(new
            {
                Name = "ETHICS",
                Description = "ETHICS"
            });



            
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.1",
                Description = "<p>Completed a formal course unit or module at a tertiary education institution on translating or interpreting or a related field</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.2",
                Description = "<p>Attended a formal professional development workshop, training module or short course at a tertiary education institution on translating and interpreting or a related field</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.3",
                Description = "<p>Attended a formal professional development workshop, training module or short course on the specialisation area of the practitioner (e.g. legal, medical)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.4",
                Description = "<p>Attended a formal professional development session, workshop, seminar, conference or webinar conducted by a professional body, NAATI, RTO, university department or language service provider – either in translating or interpreting or a related field</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.5",
                Description = "<p>Attended a formal training session, workshop or seminar relating to:</p><ul> <li> Business management </li><li> Personal development</li><li> Domestic violence, access and equity or other issues impacting on culturally and linguistically diverse communities in Australia </li><li> Computer literacy </li><li> Information and communications technology </li><li> Computer aided translation tools</li> </ul>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.6",
                Description = "<p>Attended a translating or interpreting networking session, forum, awareness day, professional association branch meeting/ National AGM or service provider developmental team meetings.</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.7",
                Description = "<p>Attended a translating or interpreting industry-related employer/workplace induction and orientation</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.8",
                Description = "<p>Received mentoring as part of an official mentoring program for interpreters or translators</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.9",
                Description = "<p>Current paid membership of a translating or interpreting professional association or representative body (e.g. AUSIT, ASLIA, WAITI, Professionals Australia [T&I section], AALITRA or CITAA)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.10",
                Description = "<p>Undergone membership renewal process with professional association or representative body(e.g. AUSIT, ASLIA, WAITI, Professionals Australia [T&I section], AALITRA or CITAA) which included professional development requirements</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 1,
                Name = "1.11",
                Description = "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion.) e.g. Acquisition of another language</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.1",
                Description = "<p>Published an article on a Translating or Interpreting subject matter in a refereed translation, interpreting or linguistics industry journal, book or relevant peer-reviewed journal</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.2",
                Description = "<p>Developed and published resource materials related to translating or interpreting, such as:</p><li>Dictionaries </li><li> Thesauri </li><li> Course materials </li><li> Specialist glossaries </li><li> Multimedia content (audio, video, digital, etc) </li><li> Editing a peer-reviewed journal</li>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.3",
                Description = "<p>Presented a paper at a translating or interpreting or related discipline conference</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.4",
                Description = "<p>Presented at a translating or interpreting workshop (either for T & I practitioners or for other professional cohorts who work with interpreters)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.5",
                Description = "<p>Taught translating or interpreting in a formal course that may result in an AQF qualification, for one semester or more. You can only claim limited workload (e.g. 2 hrs per week for 12 weeks = 24 hours)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.6",
                Description = "<p>Taught a translating or interpreting course at tertiary level for one semester or more (Covers full-time employment or maximum teaching workload)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.7",
                Description = "<p>Taught a unit(s) as part of a NAATI endorsed qualification</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.8",
                Description = "<p>Shared practical experience through mentoring new practitioners or supervising work experience students</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.9",
                Description = "<p>Served as a NAATI examiner during the recertification period and attended a NAATI examiner training workshop</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.10",
                Description = "<p>Contributed to the setting of NAATI Certification tests</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.11",
                Description = "<p>Made regular contributions as part of an AUSIT, ASLIA, WAITI, AALC or NAATI committee, sub-committee or conference organising committee</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.12",
                Description = "<p>Actively participated in the ASLIA, AUSIT, WAITI, AALC, NAATI, Professionals Australia or other translating or interpreting language forum</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.13",
                Description = "<p>Held active membership and participated in a LOTE cultural club or similar body, or contribution to LOTE or specialist media (e.g. community newsletter, radio, community television)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.14",
                Description = "<p>Published an article on the AUSIT, ASLIA, WAITI, AALC, Professionals Australia or NAATI website or in an industry related newsletter or magazine</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.15",
                Description = "<p>Completed a survey or research activity associated with the profession or certification which was advertised by NAATI as carrying PD points</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.16",
                Description = "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.17",
                Description = "<p>Taught ethics for translators and interpreters as part of a formal course that meets Australian Quality Training Framework (AQTF) criteria</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.18",
                Description = "<p>Facilitated a training event (e.g. refresher, workshop, seminar, webinar, etc.) for a professional body, NAATI or language service provider specifically concerned with ethics of the profession</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.19",
                Description = "<p>Completed regular self-directed, or group, learning activities such as writing or reading articles on ethics, engaging in ethics debates or other professional development activities with a high ethical emphasis</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.20",
                Description = "<p>Completion of an ethics online course offered by NAATI</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.21",
                Description = "<p>Attended a training event (e.g. refresher, workshop, seminar, webinar, etc.) run by a professional body, tertiary institution, NAATI or language service provider specifically concerned with ethics of the profession</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 2,
                Name = "2.22",
                Description = "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation to justify its inclusion)</p>",
                ProfessionalDevelopmentCategoryGroupId = 1
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.1",
                Description = "<p>Completed a formal professional development workshop, course unit or module at a tertiary institution on LOTE, English, Auslan/Deaf studies or linguistics</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.2",
                Description = "<p>Completed work practice in excess of NAATI’s recertification requirements</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.3",
                Description = "<p>Taught a LOTE unit or course at a tertiary institute or community language school</p>s"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.4",
                Description = "<p>Spent time in a county or region where LOTE is the primary language spoken <br />NOTE: This can only be claimed by practitioners living in a country where the primary language is English. Shorter stays can be combined and claimed at the end of the recertification period</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.5",
                Description = "<p>Spent time in a country or region where English is the primary language <br />NOTE: This can only be claimed by practitioners living in a country where the primary language is the LOTE. Shorter stays can be combined and claimed at the end of the recertification period</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.6",
                Description = "<p>Subscribed to LOTE websites, newsgroups, newspapers, magazines, periodicals or television channels</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.7",
                Description = "<p>Subscribed to Deaf community websites, newsgroups, newspapers, magazines, journals, periodicals, etc that are written in English or have content in Auslan, such as:</p> <li>World Federation of the Deaf News </li><li> Deaf Australia Outlook </li><li> British Deaf News </li><li> Journal of Deaf Studies & Deaf Education </li><li> Deaf Worlds: International </li><li> Sign Language Studies</li>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.8",
                Description = "<p>Completed private advance language (English or LOTE/Auslan (for Auslan practitioners)) study at an organisation recognised as providing expert language tuition (e.g. Alliance Française)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.9",
                Description = "<p>Attended an Auslan workshop held by recognised organisations providing expert language tuition. <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.10",
                Description = "<p>Participated in networking, community or other relevant events run by an established community organisation or professional body where the primary language used in participation is the LOTE (or Auslan for Auslan practitioners)</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.11",
                Description = "<p>Attended an ASLIA networking event or Deaf community events if held in Auslan <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.12",
                Description = "<p>Current membership of Deaf Australia <br /> NOTE: This can only be claimed by Auslan or Deaf interpreters</p>"
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategory").Row(new
            {
                ProfessionalDevelopmentSectionId = 3,
                Name = "3.13",
                Description = "<p>Practitioners are invited to nominate additional relevant PD activities, which are not listed in this section. (For each activity, please provide a brief explanation in your logbook to justify its inclusion)</p>"
            });




            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 1,
                ProfessionalDevelopmentRequirementId = 1,
                Points = 60
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 2,
                ProfessionalDevelopmentRequirementId = 2,
                Points = 40
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 3,
                ProfessionalDevelopmentRequirementId = 2,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 4,
                ProfessionalDevelopmentRequirementId = 3,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 4,
                ProfessionalDevelopmentRequirementId = 4,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 5,
                ProfessionalDevelopmentRequirementId = 3,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 5,
                ProfessionalDevelopmentRequirementId = 5,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 6,
                ProfessionalDevelopmentRequirementId = 6,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 7,
                ProfessionalDevelopmentRequirementId = 7,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 8,
                ProfessionalDevelopmentRequirementId = 8,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 9,
                ProfessionalDevelopmentRequirementId = 6,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 10,
                ProfessionalDevelopmentRequirementId = 9,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 11,
                ProfessionalDevelopmentRequirementId = 10,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 12,
                ProfessionalDevelopmentRequirementId = 11,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 13,
                ProfessionalDevelopmentRequirementId = 12,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 14,
                ProfessionalDevelopmentRequirementId = 13,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 15,
                ProfessionalDevelopmentRequirementId = 14,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 16,
                ProfessionalDevelopmentRequirementId = 15,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 17,
                ProfessionalDevelopmentRequirementId = 16,
                Points = 40
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 18,
                ProfessionalDevelopmentRequirementId = 17,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 19,
                ProfessionalDevelopmentRequirementId = 8,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 20,
                ProfessionalDevelopmentRequirementId = 18,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 20,
                ProfessionalDevelopmentRequirementId = 19,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 21,
                ProfessionalDevelopmentRequirementId = 20,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 22,
                ProfessionalDevelopmentRequirementId = 9,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 23,
                ProfessionalDevelopmentRequirementId = 21,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 24,
                ProfessionalDevelopmentRequirementId = 9,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 25,
                ProfessionalDevelopmentRequirementId = 22,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 26,
                ProfessionalDevelopmentRequirementId = 23,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 27,
                ProfessionalDevelopmentRequirementId = 10,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            { 
                ProfessionalDevelopmentCategoryId = 28,
                ProfessionalDevelopmentRequirementId = 16,
                Points = 30
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 29,
                ProfessionalDevelopmentRequirementId = 4,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 29,
                ProfessionalDevelopmentRequirementId = 3,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 30,
                ProfessionalDevelopmentRequirementId = 24,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 31,
                ProfessionalDevelopmentRequirementId = 12,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 32,
                ProfessionalDevelopmentRequirementId = 4,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 32,
                ProfessionalDevelopmentRequirementId = 3,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 33,
                ProfessionalDevelopmentRequirementId = 10,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 34,
                ProfessionalDevelopmentRequirementId = 25,
                Points = 40
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 34,
                ProfessionalDevelopmentRequirementId = 26,
                Points = 60
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 35,
                ProfessionalDevelopmentRequirementId = 27,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 35,
                ProfessionalDevelopmentRequirementId = 28,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 36,
                ProfessionalDevelopmentRequirementId = 29,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 36,
                ProfessionalDevelopmentRequirementId = 16,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 37,
                ProfessionalDevelopmentRequirementId = 30,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 37,
                ProfessionalDevelopmentRequirementId = 31,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 38,
                ProfessionalDevelopmentRequirementId = 30,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 38,
                ProfessionalDevelopmentRequirementId = 31,
                Points = 20
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 39,
                ProfessionalDevelopmentRequirementId = 32,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 40,
                ProfessionalDevelopmentRequirementId = 32,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 41,
                ProfessionalDevelopmentRequirementId = 33,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 42,
                ProfessionalDevelopmentRequirementId = 33,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 43,
                ProfessionalDevelopmentRequirementId = 34,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 44,
                ProfessionalDevelopmentRequirementId = 9,
                Points = 10
            });
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 45,
                ProfessionalDevelopmentRequirementId = 32,
                Points = 10
            });                                                                                                      
            Insert.IntoTable("tblProfessionalDevelopmentCategoryRequirement").Row(new
            {
                ProfessionalDevelopmentCategoryId = 46,
                ProfessionalDevelopmentRequirementId = 10,
                Points = 10
            });
        }
    }
}
