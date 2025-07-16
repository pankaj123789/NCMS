using System.ComponentModel.DataAnnotations;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public enum TokenReplacementField
    {
        //time
        [Display(Name = "Today Short")]
        TodayShort,
        [Display(Name = "Today Long")]
        TodayLong,
        [Display(Name = "Current Time")]
        CurrentTime,
        //cusomter
        [Display(Name = "NAATI No")]
        NaatiNo,
        [Display(Name = "NAATI Number")]
        NaatiNumber,
        [Display(Name = "Practitioner No")]
        PractitionerNo,
        [Display(Name = "Practitioner Number")]
        PractitionerNumber,
        [Display(Name = "Given Name")]
        GivenName,
        [Display(Name = "Other Names")]
        OtherNames,
        [Display(Name = "Family Name")]
        FamilyName,
        [Display(Name = "Primary Email")]
        PrimaryEmail,
        [Display(Name = "Secondary Email")]
        SecondaryEmail,
        [Display(Name = "In OD Emails")]
        InOdEmails,
        [Display(Name = "Primary Phone")]
        PrimaryPhone,
        [Display(Name = "Secondary Phone")]
        SecondaryPhone,
        [Display(Name = "In OD Phones")]
        InOdPhones,
        [Display(Name = "Primary Address")]
        PrimaryAddress,
        [Display(Name = "Secondary Address")]
        SecondaryAddress,
        [Display(Name = "In OD Address")]
        InOdAddresses,
        [Display(Name = "Preferred Test Location")]
        PreferredTestLocation,
        [Display(Name = "Sponsor")]
        Sponsor,
        //application
        [Display(Name = "Application Type")]
        ApplicationType,
        [Display(Name = "Application Status")]
        ApplicationStatus,
        [Display(Name = "Application Reference")]
        ApplicationReference,
        [Display(Name = "Credential Request Status")]
        CredentialRequestStatus,
        [Display(Name = "Credential Type")]
        CredentialType,
        [Display(Name = "Credential Request Type")]
        CredentialRequestType,
        [Display(Name = "Skill")]
        Skill,
        [Display(Name = "Action Public Note")]
        ActionPublicNote,
        [Display(Name = "Certificate Long Date")]
        CertificateLongDate,
        [Display(Name = "Certificate Long Expiry Date")]
        CertificateLongExpiryDate,
        [Display(Name = "Certificate Short Date")]
        CertificateShortDate,
        [Display(Name = "Certificate Short Expiry Date")]
        CertificateShortExpiryDate,
        [Display(Name = "Credential Expiry Date")]
        CredentialExpiryDate,
        [Display(Name = "Document Number")]
        DocumentNumber,
        [Display(Name = "Country")]
        Country,
        [Display(Name = "Suburb")]
        Suburb,
        [Display(Name = "State")]
        State,
        [Display(Name = "PostCode")]
        PostCode,
        [Display(Name = "Credential Requests")]
        CredentialRequests,
        [Display(Name = "Product Type")]
        ProductType,
        [Display(Name = "Venue Name")]
        VenueName,
        [Display(Name = "Venue Address")]
        VenueAddress,
        [Display(Name = "Test Session ID")]
        TestSessionId,
        [Display(Name = "Attendance ID")]
        TestAttendanceId,
        [Display(Name = "Test Session Name")]
        TestSessionName,
        [Display(Name = "Test Session Date")]
        TestSessionDate,
        [Display(Name = "Test Session Start Time")]
        TestSessionStartTime,
        [Display(Name = "Test Session Arrival Time")]
        TestSessionArrivalTime,
        [Display(Name = "Test Session Completion Time")]
        TestSessionCompletionTime,
        [Display(Name = "Test Session Public Notes")]
        TestSessionPublicNotes,
        [Display(Name = "Test Results Standard Marking Scheme")]
        TestResultsStandardMarkingScheme,
        [Display(Name = "Test Comments")]
        TestComments,
        [Display(Name = "Task Label")]
        TaskLabel,
        [Display(Name = "Payment Reference")]
        PaymentReference,
        [Display(Name = "PD Points Summary")]
        PdPointsSummary,
        [Display(Name = "Test Material ID")]
        TestMaterialId,
        [Display(Name = "Test Specification ID")]
        TestSpecificationId,
        [Display(Name = "Conceded Credential Type")]
        ConcededCredentialType,
        [Display(Name = "Rehearsal Date")]
        RehearsalDate,
        [Display(Name = "Rehearsal Time")]
        RehearsalTime,
        [Display(Name = "Rehearsal Notes")]
        RehearsalNotes,
        [Display(Name = "Role Play Tasks")]
        RolePlayTasks,
        [Display(Name = "Test Material Title")]
        TestMaterialTitle,
        [Display(Name = "Test Task Description")]
        TestTaskDescription,
        [Display(Name = "Language")]
        Language,
        [Display(Name = "Certification Period End Date")]
        CertificationPeriodEndDate,
        [Display(Name = "Eligible Recertification Credentials")]
        EligibleRecertificationCredentials,
        [Display(Name = "Refund Policy")]
        RefundPolicy,
        [Display(Name = "Refund Amount")]
        RefundAmount,
        [Display(Name = "Public Note")]
        PublicNote,
        [Display(Name = "Payment Type")]
        PaymentType,
        [Display(Name = "Panel Language")]
        PanelLanguage,
        [Display(Name = "Material Request Owner")]
        MaterialRequestOwner,
        [Display(Name = "Material Owner Email")]
        MaterialRequestOwnerEmail,
        [Display(Name = "Coordinator Email")]
        CoordinatorEmail,
        [Display(Name = "Collaborator Name")]
        CollaboratorName,
        [Display(Name = "Test Material Coordinator")]
        MaterialCoordinator,
        [Display(Name = "Test Material Development Due")]
        RoundDueDate,
        [Display(Name = "Cost Per Hour")]
        MaterialRequestCost,
        [Display(Name = "Max Billable Hours")]
        MaxBillableHours,
        [Display(Name = "Test Material Round")]
        RoundNumber,
        [Display(Name = "Test Material Submit Approval Date")]
        SubmittedDate,
        [Display(Name = "Panel Membership Type")]
        PanelMembershipType,
        [Display(Name = "Panel Membership Task")]
        PanelMembershipTask,
        // Email Templates
        [Display(Name = "Email Footer")]
        EmailFooter,
        [Display(Name = "Div")]
        Div,
        [Display(Name = "Div End")]
        DivEnd,
        [Display(Name = "Div Class Test Results")]
        DivClassTestResults,
        [Display(Name = "CCL Specific Examiner Comments Text")]
        CCLSpecificExaminerCommentsText,
        [Display(Name = "CCL Specific Comment Reference Table")]
        CCLSpecificCommentReferenceTable,
    }
}
