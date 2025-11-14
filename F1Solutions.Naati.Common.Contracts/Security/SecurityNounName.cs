using System.ComponentModel;

namespace F1Solutions.Naati.Common.Contracts.Security
{
    public enum SecurityNounName
    {
        [Description("Applications")]
        Application = 1,
        [Description("Certification Periods")]
        CertificationPeriod=2,
        [Description("Credentials")]
        Credential=3,
        [Description("Credential Requests")]
        CredentialRequest=4,
        [Description("Files/Documents")]
        Document=5,
        Email=6,
        [Description("Email Templates")]
        EmailTemplate=7,
        [Description("Endorsed Qualifications")]
        EndorsedQualification=8,
        [Description("Examiners")]
        Examiner=9,
        [Description("Examiner Marks")]
        ExaminerMarks=10,
        [Description("Examiner Payments")]
        ExaminerPayment=11,
        [Description("Miscellaneous Finance Functions")]
        FinanceOther=12,
        [Description("Invoices")]
        Invoice=13,
        [Description("Languages")]
        Language=14,
        [Description("Logbooks")]
        Logbook=15,
        [Description("Material Requests")]
        MaterialRequest=16,
        [Description("Organisations")]
        Organisation=17,
        [Description("Paid Reviews")]
        PaidReview=18,
        [Description("Panels")]
        Panel=19,
        [Description("Payments")]
        Payment=20,
        [Description("Pay Runs")]
        PayRun=21,
        [Description("Customers")]
        Person=22,
        [Description("Customer Finance Details")]
        PersonFinanceDetails=23,
        [Description("myNAATI Registrations")]
        PersonMyNaatiRegistration=24,
        [Description("Role-Players")]
        RolePlayer=25,
        [Description("Rubric Results")]
        RubricResult=26,
        [Description("Skills")]
        Skill=27,
        [Description("Supplementary Tests")]
        SupplementaryTest=28,
        [Description("System Settings")]
        System=29,
        [Description("Test Assets")]
        TestAsset=30,
        [Description("Test Materials")]
        TestMaterial=31,
        [Description("Test Results")]
        TestResult=32,
        [Description("Test Sessions")]
        TestSession=33,
        [Description("Test Sittings")]
        TestSitting=34,
        [Description("Test Specifications")]
        TestSpecification=35,
        [Description("Users")]
        User=36,
        [Description("Venues")]
        Venue=37,
        [Description("Entity Lookup")]
        Entity = 38,
        [Description("Contact")]
        Contact = 39,
        [Description("Audit")]
        Audit = 40,
        [Description("General")]
        General = 41,
        [Description("Notes")]
        Notes = 42,
        Bill = 43,
        PersonHistory = 44,
        OrganisationHistory = 45,
        [Description("Panel Member")]
        PanelMember = 46,
        Dashboard =47,
        ApiAdministrator = 48,
        Location = 50
    }
}
