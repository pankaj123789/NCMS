using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface IApiPublicService : IInterceptableservice
    {
      
        ApiPublicPractitionerSearchResponse SearchPractitioner(ApiPublicPractitionerSearchRequest request);

       
        ApiPublicPractitionerCountResponse PractitionersCount(PractitionerCountRequest request);

    
        PublicLookupResponse GetLookups(LookupRequest request);

      
        LanguagesResponse GetLanguages(LanguagesRequest request);

   
        PublicLegacyAccreditionsResponse GetLegacyAccreditions(GetLegacyAccreditionsRequest request);

       
        ApiTestSessionSearchResponse SearchTestSession(ApiTestSessionSearchRequest request);

      
        ApiEndorsedQualificationResponse SearchEndorsedQualification(ApiEndorseQualificationSearchRequest request);

      
        ApiTestSessionAvailabilityResponse GetTestSessionAvailability(ApiTestSessionAvailabilityRequest request);

   
        ApiPublicCertificationsResponse GetCertifications(GetCertificationsRequest request);
       
        ApiPersonImageResponse GetPersonPhoto(ApiPublicPersonPhotoRequest request);

        ApiPublicVerifyCertificationQrCodeResponse VerifyCertificationQrCode(ApiCredentialQrCodeRequest request);

        /// <summary>
        /// Designed to allow www.naati.com.au to verify the document number of any document
        /// To be use to prevent false documents being presented
        /// </summary>
        /// <param name="request"></param>
        /// <returns>name, date of issue, certificaiton type</returns>
        ApiVerifyDocumentResponse VerifyDocument(ApiVerifyDocumentRequest request);
    }

    public class ApiTestSessionSearchResponse :BaseResponse
    {
        public IEnumerable<ApiTestSessionContract> Results { get; set; }
    }

    public class ApiTestSessionAvailabilityResponse : BaseResponse
    {
        public IEnumerable<ApiTestSessionAvailabilityContract> Results { get; set; }
    }

    public class ApiTestSessionAvailabilityContract
    {
        public int TestSessionId { get; set; }
        public string Name { get; set; }
        public virtual string TestDateTime { get; set; }
        public int DurationInMinute { get; set; }
        public int TestLocationId { get; set; }
        public string TestLocation { get; set; }
        public string Venue { get; set; }
        public string VenueAddress { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsPreferredLocation { get; set; }
    }

    public class ApiTestSessionContract
    {
        public virtual string TestDateTime { get; set; }
        public string Venue { get; set; }
        public virtual int TestSessionId { get; set; }
        public virtual int CredentialTypeId { get; set; }
        public virtual string CredentialType { get; set; }
        public virtual int Capacity { get; set; }
        public virtual bool Completed { get; set; }
        public virtual string TestLocationName { get; set; }
        public virtual string TestLocationStateName { get; set; }
        public virtual string SessionName { get; set; }
        public virtual int? DurationInMinute { get; set; }
    }

    public class ApiEndorsedQualificationResponse : BaseResponse
    {
        public IEnumerable<ApiEndorsedQualificationContract> Results { get; set; }
    }

    public class ApiEndorseQualificationSearchRequest
    {
        public IEnumerable<ApiPublicSearchFilter> Filters { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }

    public class ApiTestSessionAvailabilityRequest
    {
        public int SkillId { get; set; }
        public int CredentialTypeId { get; set; }
        public int PreferredTestLocationId { get; set; }
        public bool IncludeBacklog { get; set; }
        // public string FromTestDate { get; set; }
       // public string ToTestDate { get; set; }
    }

    public class ApiEndorsedQualificationContract
    {
        public int      InstitutionId { get; set; }
        public string   InstitutionName { get; set; }
        public string   Location              { get; set; }
        public string   Qualification { get; set; }
        public int      CredentialTypeId { get; set; }
        public string   CredentialType { get; set; }
        public string EndorsementPeriodFrom { get; set; }
        public string EndorsementPeriodTo { get; set; }
        public bool     Active { get; set; }
    }

    public class ApiPublicCredentials
    {
        public string CertificationType { get; set; }
        public string Skill { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string GeneratedOn { get; set; }
    }

    public class ApiPublicPractitioner
    {
        public string PractitionerId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Country { get; set; }
    }

    public class ApiPublicCertifications
    {
        public ApiPublicPractitioner Practitioner { get; set; }
        public IEnumerable<ApiPublicCredentials> CurrentCertifications { get; set; }
        public IEnumerable<ApiPublicCredentials> PreviousCertifications { get; set; }
    }

    public class ApiPublicCertificationsResponse :BaseResponse
    {
        public ApiPublicPractitioner Practitioner { get; set; }
        public IEnumerable<ApiPublicCredentials> CurrentCertifications { get; set; }
        public IEnumerable<ApiPublicCredentials> PreviousCertifications { get; set; }
    }

    public class ApiPublicVerifyCertificationQrCodeResponse : BaseResponse
    {
        public string PractionerNumber { get; set; }
        public string Certification { get; set; }
        public DateTime DateIssued { get; set; }
    }

    public class ApiVerifyDocumentRequest
    {
        public string DocumentNumber { get; set; }
    }

    public class ApiVerifyDocumentResponse : BaseResponse
    {
        public string Name { get; set; }
        public string PractitionerId { get; set; }
        public string CertificationType { get; set; }
        public string Skill { get; set; }
        public DateTime DateIssued { get; set; }
    }

    //these are duplicated in NCMS.BI.Extensions.ApiPermissions. Need to maintain both
    [Flags]
    public enum EndpointPermission
    {
        None = 0x1,
        PractitionersCount = 0x2,
        Practitioners = 0x4,
        Lookups = 0x8,
        Languages = 0x10,
        LegacyAccreditations = 0x20,
        TestSessions = 0x40,
        EndorsedQualifications = 0x80,
        TestSessionAvailability = 0x100,
        Certifications = 0x200,
        PersonPhoto = 0x400
    }
}
