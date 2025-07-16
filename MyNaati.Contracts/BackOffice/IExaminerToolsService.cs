using System;
using System.Collections.Generic;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface IExaminerToolsService : IInterceptableservice
    {
        
        GetTestsResponse GetTests(GetTestsRequest request);

        
        GetTestsMaterialResponse GetTestsMaterial(GetTestsMaterialRequest request);

        
        TestMaterialContract GetTestMaterial(GetTestMaterialRequest request);

        
        GetTestDetailsResponse GetTestDetails(GetTestDetailsRequest request);

        
        SubmitTestResponse SubmitTest(SubmitTestRequest request);

        
        bool SaveExaminerPapersRecievedDateRequested(SaveExaminerPapersRecievedDateRequest request);

        
        ListUnavailabilityResponse ListUnavailability(ListUnavailabilityRequest request);

        
        SaveUnavailabilityResponse SaveUnavailability(SaveUnavailabilityRequest request);

        
        DeleteUnavailabilityResponse DeleteUnavailability(DeleteUnavailabilityRequest request);

        
        SaveMaterialResponse SaveMaterial(SaveMaterialRequest request);

        
        SaveAttachmentResponse SaveAttachment(SaveAttachmentRequest request);

        
        DeleteMaterialResponse DeleteMaterial(DeleteMaterialRequest request);

        
        DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request);

        
        bool IsValidAttendeeDeleteAttachment(int testAttendanceDocumentId, int naatiNumber);

        
        GetMaterialFileResponse GetMaterialFile(GetMaterialFileRequest request);

        
        GetTestAssetsFileResponse GetTestAssetsFile(GetTestAssetsFileRequest request);

        
        SubmitMaterialResponse SubmitMaterial(SubmitMaterialRequest request);

        
        GetTestMaterialsFileResponse GetTestMaterialsFile(GetTestMaterialsFileRequest request);

        
        GetTestAttendanceDocumentResponse GetTestAttendanceDocument(GetTestAttendanceDocumentRequest request);

        
        GetPayrollHistoryResponse GetPayrollHistory(GetPayrollHistoryRequest request);

        
        GetDocumentTypesResponse GetDocumentTypes();


        
        TestRubricMarkingContract GetExaminerRubricMarking(int naatiNumber, int testResultId);

        
        SaveExaminerMarkingResponse SaveExaminerRubricMarking(TestRubricMarkingContract model);

        
        void SubmitExaminerRubricMarking(TestRubricMarkingContract model);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetTestMaterialsByAttendaceId(int attendanceId);

        
        GetAttendeesTestSpecificationTestMaterialResponse GetFileStoreTestSpecificationTestMaterialList(GetFileStoreTestSpecificationTestMaterialRequest request);

        
        string[] TestMaterialReplaceTokens(GetAttendeesTestSpecificationTestMaterialResponse request, string tempFileStorePath);
        
        void SaveRolePlayerSettings(RolePlayerSettingsRequest request);
        
        GetRolePlayerSettingsResponse GetRolePlayerSettings(GetRolePlayerSettingsRequest request);
        
        GetRolePlaySessionResponse GetRolePlaySession(GetRolePlaySessionRequest request);

        
        int? GetTestSittingIdByTestResult(int testResultId);

        
        bool IsValidExaminerForAvailability(int examinerUnavailableId, int naatiNumber);

        
        MaterialRequestSearchResponse GetMaterialRequests(GetMaterialRequestsRequest request);
        
        GetMaterialRequestResponse GetMaterialRequest(int materialRequestId, int naatiNumber, bool write);

        
        SaveMaterialRequestRoundAttachmentResponse SaveMaterialRequestRoundAttachment(SaveMaterialRequestRoundAttachmentRequest request);
        
        GetMaterialRequestRoundAttachmentsResponse GetMaterialRequestRoundAttachments(GetMaterialRequestRoundAttachmentsRequest request);
        
        GetMaterialRequestRoundAttachmentResponse GetMaterialRequestRoundAttachment(GetMaterialRequestRoundAttachmentRequest request);
        
        void DeleteMaterialRequestRoundAttachment(DeleteMaterialRequestRoundAttachmentRequest request);
        
        GetPanelMembershipLookUpResponse GetPanelMembershipLookUp(GetPanelMemberLookupRequestModel request);
        
        UpdateMaterialRequestMembersResponse UpdateMaterialRequestMembers(UpdateMaterialRequestMembersRequest request);
        
        ListMaterialRequestPublicNotesResponse ListMaterialRequestPublicNotes(int materialRequestId);
        
        void CreateMaterialRequestPublicNote(int materialRequestId, string note);
     
        IEnumerable<LookupContract> GetMaterialRequestCredentialTypes(int naatiNumber);

        
        IEnumerable<LookupContract> GetRoundStatusTypes();
        
        SaveMaterialRequestRoundLinkResponse SaveMaterialRequestRoundLink(SaveMaterialRequestRoundLinkRequest request);
        
        GetMaterialRequestRoundLinkResponse GetMaterialRequestRoundLink(int materialRequestRoundId, int naatiNumber);
        
        DeleteMaterialRequestLinkResponse DeleteMaterialRequestLink(int materialRequestRoundLinkId, int naatiNumber, int materialRoundId);
        
        IEnumerable<LookupContract> GetTestMaterialDomains(int credentialTypeId);

        GetTestMaterialCreationPaymentsResponse GettestMaterialCreationPayments(GetTestMaterialCreationPaymentsRequest getTestMaterialCreationPaymentsRequest);
    }

    public class DeleteMaterialRequestLinkResponse
    {

    }

    public class MaterialRequestLinkContract
    {
        public int Id { get; set; }
        public int MaterialRequestRoundId { get; set; }
        public string Link { get; set; }
        public bool IsOwner { get; set; }
    }

    public class GetMaterialRequestRoundLinkResponse
    {
        public IEnumerable<MaterialRequestLinkContract> Results { get; set; }
    }

    public class SaveMaterialRequestRoundLinkRequest
    {
        public int MaterialRequestRoundId { get; set; }
        public string Link { get; set; }
        public int NaatiNumber { get; set; }
    }

    public class SaveMaterialRequestRoundLinkResponse
    {

    }

    public class UpdateMaterialRequestMembersResponse
    {

    }

    public class ListMaterialRequestPublicNotesResponse
    {
        public IEnumerable<NoteModel> Notes { get; set; }
    }

    public class UpdateMaterialRequestMembersRequest
    {
        public int MaterialRequestId { get; set; }
        public int RoundId { get; set; }
        public int NaatiNumber { get; set; }
        public IEnumerable<MaterialRequestPanelMembershipContract> Members { get; set; }
    }
    public class NoteModel
    {
        public int? NoteId { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Highlight { get; set; }
        public bool ReadOnly { get; set; }

        public string Reference { get; set; }
        public int ReferenceType { get; set; }
    }

    public class MaterialRequestTaskModel
    {
        public int Id { get; set; }
        public int MaterialRequestTaskTypeId { get; set; }
        public double HoursSpent { get; set; }
    }

    public class PanelMembershipLookupModel
    {
        public int NaatiNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<MaterialRequestTaskModel> Tasks { get; set; }
    }
    public class GetPanelMembershipLookUpResponse
    {
        public IEnumerable<PanelMembershipLookupModel> Members { get; set; }
    }
    public class GetPanelMemberLookupRequestModel
    {
        public int[] PanelIds { get; set; }

        public bool ActiveMembersOnly { get; set; }

        public int? CredentialTypeId { get; set; }
    }

    public class GetMaterialRequestRoundAttachmentRequest
    {
        public int MaterialRequestRoundAttachmentId { get; set; }
        public int MaterialRequestRoundId { get; set; }
        public int NaatiNumber { get; set; }
        public string TempFileStorePath { get; set; }
    }

    public class GetMaterialRequestRoundAttachmentResponse
    {
        public string FileName { get; set; }
        public string[] FilePaths { get; set; }
    }

    public class SaveMaterialRequestRoundAttachmentRequest
    {
        public int NAATINumber { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public int MaterialRequestRoundId { get; set; }
        public int Type { get; set; }
    }

    public class SaveMaterialRequestRoundAttachmentResponse
    {
        public int StoredFileId { get; set; }
    }

    public class GetMaterialRequestsRequest
    {
        public int NaatiNumber { get; set; }
        public int[] RoundStatusId { get; set; }
        public int[] CredentialTypeId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public bool? Overdue { get; set; }
        public int[] TestMaterialId { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }

    public class MaterialRequestSearchResultContract
    {
        public int MaterialRequestId { get; set; }
        public int TestMaterialId { get; set; }
        public string Title { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string SkillDisplayName { get; set; }
        public string TaskTypeName { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public int RoundId { get; set; }
        public int RoundNumber { get; set; }
        public int RoundStatusId { get; set; }
        public string RoundStatusName { get; set; }
        public DateTime DueDate { get; set; }  
        
        public bool IsCoordinator { get; set; }
        public bool IsEditable { get; set; }
    }

    public class TestMaterialRequestContract
    {
        public int MaterialRequestId { get; set; }
        public int TestMaterialId { get; set; }
        public string Title { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string SkillDisplayName { get; set; }
        public string TaskTypeName { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public int RoundId { get; set; }
        public int RoundNumber { get; set; }
        public string Round { get; set; }
        public int RoundStatusId { get; set; }
        public string RoundStatusName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RoundRequestedDate { get; set; }
        public MaterialRequestPanelMembershipContract[] Members { get; set; }
        public decimal ProductSpecificationCostPerUnit { get; set; }
        public int PanelId { get; set; }
        public int CredentialTypeId { get; set; }
        public double MaxBillableHours { get; set; }
        public int TestMaterialDomainId { get; set; }
    }
    public class MaterialRequestPanelMembershipContract
    {
        public int Id { get; set; }

        public int PanelMemberShipId { get; set; }
        public int NaatiNumber { get; set; }
        public int EntityId { get; set; }

        public IList<MaterialRequestTaskContract> Tasks { get; set; }
        public int MemberTypeId { get; set; }

        public string GivenName { get; set; }
        public string PrimaryEmail { get; set; }
    }
    public class MaterialRequestTaskContract
    {
        public int Id { get; set; }
        public int MaterialRequestTaskTypeId { get; set; }
        public double HoursSpent { get; set; }
        public string MaterialRequestTaskTypeDisplayName { get; set; }
    }

    public class GetMaterialRequestsResponse
    {
        public IEnumerable<TestMaterialRequestContract> MaterialRequests { get; set; }
    }

    public class MaterialRequestSearchResponse
    {
        public IEnumerable<MaterialRequestSearchResultContract> MaterialRequests { get; set; }
        public int TotalAvailableRows { get; set; }
    }

    public class GetMaterialRequestResponse
    {
        public TestMaterialRequestContract MaterialRequest { get; set; }
        public IEnumerable<KeyValuePair<int, string>> DocumentTypes { get; set; }
        public IEnumerable<LookupTypeDto> MaterialRequestTaskTypes { get; set; }
    }

    public class GetRolePlaySessionRequest
    {
        public int? TestSessionRolePlayerId { get; set; }

        public int? NaatiNumber { get; set; }
        public int ActionId { get; set; }
    }

    public class GetRolePlaySessionResponse
    {
        public List<RolePlaySessionContract> Sessions { get; set; }
    }

    public class GetRolePlayerSettingsRequest
    {
        public int NaatiNumber { get; set; }
    }

    public class GetRolePlayerSettingsResponse
    {
        public RolePlayerSettingsDto Settings { get; set; }
    }


    public class DeleteUnavailabilityResponse
    {
    }


    public class DeleteUnavailabilityRequest
    {

        public int Id { get; set; }
    }


    public class ExaminerUnavailableContract
    {

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }


    


    public class SaveUnavailabilityRequest
    {
        public int? Id { get; set; }

        public int NAATINumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class RolePlayerSettingsRequest
    {
        public RolePlayerSettingsDto Settings { get; set; }
    }

    public class RolePlayerSettingsDto
    {
        public int[] RolePlayLocations { get; set; }
        public int MaximumRolePlaySessions { get; set; }
        public int NaatiNumber { get; set; }
    }


    public class SaveUnavailabilityResponse
    {
    }


    public class ListUnavailabilityResponse
    {

        public ExaminerUnavailableContract[] Periods { get; set; }
    }


    public class ListUnavailabilityRequest
    {

        public int NAATINumber { get; set; }
    }


    public class GetTestsResponse
    {

        public TestContract[] Tests { get; set; }
    }


    public class GetTestsMaterialResponse
    {

        public TestMaterialContract[] Tests { get; set; }
    }


    public class GetTestDetailsResponse
    {

        public int OverAllPassMark { get; set; }


        public TestComponentContract[] Components { get; set; }


        public TestAttendanceDocumentContract[] Attachments { get; set; }

        public int TestMarkingTypeId { get; set; }
        public string Feedback { get; set; }
    }


    public class GetTestsMaterialRequest
    {

        public int UserId { get; set; }


        public int[] RoleCategoryIds { get; set; }
    }


    public class GetTestMaterialRequest
    {

        public int NAATINumber { get; set; }


        public int TestMaterialId { get; set; }
    }


    public class GetMaterialRequestRoundAttachmentsRequest
    {
        public int MaterialRequestRoundId { get; set; }
        public int NAATINumber { get; set; }

        public bool? NcmsAvailable { get; set; }
        public bool? ExaminerAvailable { get; set; }
    }

    public class DeleteMaterialRequestRoundAttachmentRequest
    {

        public int NaatiNumber { get; set; }
        public int MaterialRequestRoundId { get; set; }
        public int MaterialRequestRoundAttachmentId { get; set; }
    }

    public class GetMaterialRequestRoundAttachmentsResponse
    {
        public IEnumerable<MaterialRequestRoundAttachmentDto> Attachments { get; set; }
    }

    public class MaterialRequestRoundAttachmentDto
    {
        public int MaterialRequestRoundAttachmentId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string DocumentType { get; set; }
        public long FileSize { get; set; }
        public bool IsOwner { get; set; }
    }

    public class GetTestsRequest
    {

        public int UserId { get; set; }


        public int[] RoleCategoryIds { get; set; }


        public bool AsChair { get; set; }


        public int[] NAATINumber { get; set; }


        public DateTime? DateAllocatedFrom;


        public DateTime? DateAllocatedTo { get; set; }


        public int[] PanelId { get; set; }


        public DateTime? DateDueFrom { get; set; }


        public DateTime? DateDueTo { get; set; }


        public string[] TestStatus { get; set; }
    }


    public class GetTestDetailsRequest
    {

        public int TestResultId { get; set; }

        public int NaatiNumber { get; set; }
    }


    public class TestContract
    {

        public int JobID { get; set; }

        public int TestSittingId { get; set; }

        public int TestResultID { get; set; }

        public string SkillDisplayName { get; set; }

        public string Category { get; set; }

        public string Direction { get; set; }

        public string Level { get; set; }

        public string CredentialTypeExternalName { get; set; }

        public DateTime TestDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int MaterialID { get; set; }

        public string Status { get; set; }

        public string Examiner { get; set; }

        public DateTime DateAllocated { get; set; }

        public string Description { get; set; }

        public bool HasAssets { get; set; }

        public bool Supplementary { get; set; }
        public bool HasDefaultSpecification { get; set; }

        public int TestMarkingTypeId { get; set; }
    }


    public class TestMaterialContract
    {

        public int TestMaterialID { get; set; }

        public int JobExaminerID { get; set; }

        public int JobID { get; set; }

        public string Language { get; set; }

        public string Category { get; set; }

        public string Direction { get; set; }

        public string Level { get; set; }

        public DateTime? DueDate { get; set; }

        public List<MaterialContract> Materials { get; set; }
    }


    public class TestComponentContract
    {

        public int Id { get; set; }

        public int TotalMarks { get; set; }

        public double PassMark { get; set; }

        public double? Mark { get; set; }

        public int ComponentNumber { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public string TypeName { get; set; }

        public string TypeLabel { get; set; }

        public int GroupNumber { get; set; }


        public bool ReadOnly { get; set; }
    }


    public class TestAttendanceDocumentContract
    {

        public int TestAttendanceDocumentId { get; set; }


        public string Title { get; set; }


        public string Type { get; set; }
    }


    public class SubmitTestRequest
    {

        public int UserId { get; set; }

        public int TestResultID { get; set; }

        public List<TestComponentContract> Components { get; set; }

        public string Comments { get; set; }
        public string Feedback { get; set; }

        public List<string> ReasonsForPoorPerformance { get; set; }

        public int PrimaryReasonForFailure { get; set; }
    }


    public class SubmitTestResponse
    {
    }


    public class SaveMaterialRequest
    {
        [MessageHeader]
        public int TestMaterialId { get; set; }

        [MessageHeader]
        public int NAATINumber { get; set; }

        [MessageHeader]
        public string Title { get; set; }


        public string FilePath { get; set; }
    }


    public class SaveMaterialResponse
    {

        public int StoredFileId { get; set; }

        public int TestMaterialId { get; set; }
    }


    public class GetMaterialFileRequest
    {

        public int MaterialId { get; set; }


        public string TempFileStorePath { get; set; }
    }


    public class GetTestAttendanceDocumentRequest
    {

        public int TestAttendanceDocumentId { get; set; }


        public string TempFileStorePath { get; set; }
    }


    public class GetTestAssetsFileRequest
    {

        public int TestSittingId { get; set; }


        public string TempFileStorePath { get; set; }
    }


    public class GetTestMaterialsFileRequest
    {

        public int TestMaterialId { get; set; }


        public int NAATINumber { get; set; }


        public string TempFileStorePath { get; set; }
    }


    public class DeleteMaterialResponse
    {
    }


    public class DeleteMaterialRequest
    {

        public int MaterialId { get; set; }


        public int NAATINumber { get; set; }
    }


    public class DeleteAttachmentResponse
    {
    }


    public class DeleteAttachmentRequest
    {

        public int TestAttendanceDocumentId { get; set; }


        public int NAATINumber { get; set; }
    }


    public class GetMaterialFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }


        public string[] FilePaths { get; set; }
    }


    public class GetTestAssetsFileResponse
    {
        [MessageHeader]
        public string FileName { get; set; }


        public string[] FilePaths { get; set; }
    }


    public class GetDocumentResponse
    {
        [MessageHeader]
        public string FileName { get; set; }


        public string[] FilePaths { get; set; }
    }


    public class GetTestMaterialsFileResponse : GetDocumentResponse
    {
    }


    public class GetTestAttendanceDocumentResponse : GetDocumentResponse
    {
    }


    public class DocumentTypeContract
    {

        public int Id { get; set; }

        public string DisplayName { get; set; }
    }


    public class GetDocumentTypesResponse
    {

        public DocumentTypeContract[] DocumentTypes { get; set; }
    }


    public class MaterialContract
    {

        public int MaterialId { get; set; }


        public string Title { get; set; }
    }

    public class TestSessionRolePlayerDetailContract
    {
        public int TestSessionRolePlayerDetailId { get; set; }
        public int SkillId { get; set; }
        public int TestComponentId { get; set; }
        public int LanguageId { get; set; }
        public int RolePlayerRoleTypeId { get; set; }

        public string RolePlayerRoleTypeName { get; set; }
        public string SkillName { get; set; }
        public string TestComponentName { get; set; }

        public string LanguageName { get; set; }
        public string TaskLabel { get; set; }
        public string TaskTypeLabel { get; set; }
    }


    public class RolePlaySessionContract
    {

        public int TestSessionRolePlayerId { get; set; }
        public int TestSessionId { get; set; }
        public int RolePlayerId { get; set; }

        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }
        public int RolePlayerStatusId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int StatusChangeUserId { get; set; }
        public DateTime? RehearsalDate { get; set; }
        public DateTime TestSessionDate { get; set; }
        public string TestSessionName { get; set; }
        public string TestSessionLocation { get; set; }
        public string RolePlayerStatus { get; set; }
        public string RolePlayerStatusDisplayName { get; set; }

        public string CredentialTypeExternalName { get; set; }
        public string RehearsalNotes { get; set; }
        public string PublicNote { get; set; }
        public string VenueCoordinates { get; set; }
        public string VenueAddress { get; set; }
        public string VenueName { get; set; }

        public bool CanAccept { get; set; }
        public bool CanReject { get; set; }
        public int NaatiNumber { get; set; }

        public int AcceptActionId { get; set; }
        public int RejectActionId { get; set; }
        public IList<TestSessionRolePlayerDetailContract> Details { get; set; }


    }

    public class SubmitMaterialResponse
    {
    }

    public class SubmitMaterialRequest
    {

        public int JobExaminerId { get; set; }
    }


    public class SaveAttachmentResponse
    {

        public int TestAttendanceDocumentId { get; set; }


        public int StoredFileId { get; set; }
    }


    public class SaveAttachmentRequest
    {
        [MessageHeader]
        public int TestResultId { get; set; }

        [MessageHeader]
        public int NAATINumber { get; set; }

        [MessageHeader]
        public string Title { get; set; }


        public string FilePath { get; set; }


        public int Type { get; set; }
    }


    public class GetPayrollHistoryRequest
    {

        public string ExaminerNaatiNumber { get; set; }
    }


    public class GetPayrollHistoryResponse
    {

        public IEnumerable<MarkingPayrollItemContract> MarkingPayrollItems { get; set; }
    }

    public class GetTestMaterialCreationPaymentsResponse
    {
        public IEnumerable<TestMaterialCreationPaymentContract> Payments { get; set; }
    }

    public class MarkingPayrollItemContract
    {

        public string Language { get; set; }

        public int TestAttendanceId { get; set; }

        public decimal ExaminerCost { get; set; }

        public decimal InvoiceTotal { get; set; }

        public DateTime? PayrollModifiedDate { get; set; }

        public DateTime ResultReceivedDate { get; set; }

        public DateTime? PaperReceivedDate { get; set; }

        public string TestType { get; set; }

        public string AccountingReference { get; set; }

        public string PayrollStatus { get; set; }

        public bool Supplementary { get; set; }
    }

    public class TestMaterialCreationPaymentContract
    {
        public int TestMaterialId { get; set; }
        public DateTime? MaterialCreationSubmittedDate { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? PaymentApprovedDate { get; set; }
        public DateTime? PaymentProcessedDate { get; set; }
        public string InvoiceNo { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TotalInvoice { get; set; }
        public string Skill { get; set; }
        public string CredentialType { get; set; }
    }


    public class TestMarkingComponentContract
    {

        public int Id { get; set; }

        public int TotalMarks { get; set; }

        public double? Mark { get; set; }

        public double PassMark { get; set; }

        public int ComponentNumber { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public string TypeName { get; set; }

        public string TypeLabel { get; set; }

        public int GroupNumber { get; set; }

        public bool WasAttempted { get; set; }

        public bool? Successful { get; set; }

        public bool ReadOnly { get; set; }

        public int MinExaminerCommentLength { get; set; }

        public int MaxCommentLength { get; set; }

        public IEnumerable<TestCompetenceContract> Competencies { get; set; }
    }


    public class TestCompetenceContract
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public List<TestAssessmentContract> Assessments { get; set; }
    }


    public class TestAssessmentContract
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public string Comment { get; set; }

        public int? SelectedBand { get; set; }

        public IEnumerable<TestBandContract> Bands { get; set; }

        public IEnumerable<ExaminerResultContract> ExaminerResults { get; set; }
    }


    public class TestBandContract
    {

        public int Id { get; set; }

        public string Description { get; set; }

        public string Label { get; set; }
        public string Level { get; set; }
    }


    public class ExaminerResultContract
    {

        public int JobExaminerId { get; set; }

        public string Initials { get; set; }

        public int Band { get; set; }

        public string Comment { get; set; }
    }

    public class TestRubricMarkingContract
    {

        public int NaatiNumber { get; set; }

        public int TestResultId { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public IEnumerable<TestMarkingComponentContract> TestComponents { get; set; }
        public string Feedback { get; set; }
    }


    public class SaveExaminerMarkingResponse
    {

        public string ErrorMessage { get; set; }
    }
}
