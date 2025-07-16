using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal
{
  public class CredentialQueryService : ICredentialQueryService
    {
        public GetCertificationPeriodResponse GetCertificationPeriod(int certificationPeriodId)
        {
            var period = NHibernateSession.Current.Get<CertificationPeriod>(certificationPeriodId);
            period.NotNull($"Certification Period with id {certificationPeriodId} not found.");
            return new GetCertificationPeriodResponse
            {
                Data = new CertificationPeriodDto
                {
                    Id = certificationPeriodId,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    OriginalEndDate = period.OriginalEndDate,
                    NaatiNumber = period.Person.GetNaatiNumber(),
                    CredentialApplicationId = period.CredentialApplication.Id
                }
            };
        }

        public GetCredentialTypeResponse GetCredentialType(int credentialTypeId)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(credentialTypeId);
            credentialType.NotNull($"Credential Type with id {credentialTypeId} not found.");
            return new GetCredentialTypeResponse
            {
                Data = CredentialTypeToDto(credentialType)
            };
        }

        public CredentialSearchResponse SearchCredential(GetCredentialSearchRequest request)
        {
            var credentialQueryHelper = new CredentialQueryHelper();
            return new CredentialSearchResponse {Results = credentialQueryHelper.SearchCredentials(request)};
        }

        public PersonSearchResponse SearchPerson(GetPersonSearchRequest request)
        {
            var personQueryHelper = new PersonQueryHelper();
            return new PersonSearchResponse { Results = personQueryHelper.SearchPeople(request) };
        }

        public GenericResponse<VerifyDocumentResponse> VerifyCredentialDocument(VerifyDocumentRequest verifyDocumentRequest)
        {
            var credential = NHibernateSession.Current.Get<Credential>(verifyDocumentRequest.CredentialId);
            var credentialApplication = NHibernateSession.Current.Get<CredentialApplication>(verifyDocumentRequest.CredentialApplicationId);
            if(credential == null || credentialApplication == null)
            {
                return new GenericResponse<VerifyDocumentResponse>()
                {
                    Success = false,
                    Messages = new List<string>()
                    {
                        $"Not found Credential:{verifyDocumentRequest.CredentialId} Application:{verifyDocumentRequest.CredentialApplicationId}"
                    }
                };
            }

            //need the Application and Crdential to belong to the same person
            var credentialRequest = credential.CredentialCredentialRequests.FirstOrDefault()?.CredentialRequest;

            var applicationIdToCheck = credentialRequest?.CredentialApplication.Id;

            if(applicationIdToCheck != credentialApplication.Id)
            {
                return new GenericResponse<VerifyDocumentResponse>()
                {
                    Success = false,
                    Messages = new List<string>()
                    {
                        $"Credential and Application do not match:{verifyDocumentRequest.CredentialId} Application:{verifyDocumentRequest.CredentialApplicationId}"
                    }
                };
            }

            var personName = credentialApplication.Person.LatestPersonName.PersonName;
            return new VerifyDocumentResponse()
            {
                PractitionerId = credentialApplication.Person.PractitionerNumber,
                CertificationType = credentialRequest?.CredentialType.ExternalName,
                Skill = credentialRequest?.Skill.DisplayName,
                DateIssued = credential.StartDate,
                Name = $"{personName.GivenName} {personName.Surname}"
            };
        }

        internal static CredentialTypeDto CredentialTypeToDto(CredentialType credentialType)
        {
            var rolePlayersRequired = credentialType.ActiveTestSpecification?.TestComponents.Any(x => x.Type.RoleplayersRequired) ?? false;
            return new CredentialTypeDto
            {
                Id = credentialType.Id,
                ExternalName = credentialType.ExternalName,
                InternalName = credentialType.InternalName,
                DisplayOrder = credentialType.DisplayOrder,
                Simultaneous = credentialType.Simultaneous,
                Certification = credentialType.Certification,
                CredentialApplicationTypeCredentialTypes = credentialType.CredentialApplicationTypeCredentialTypes.Select(CredentialApplicationTypeCredentialTypeToDto).ToList(),
                DefaultExpiry = credentialType.DefaultExpiry,
                ActiveTestSpecificationId = credentialType.ActiveTestSpecification?.Id,
                MarkingSchemaTypeId = (int?)credentialType.ActiveTestSpecification?.MarkingSchemaType(),
                AllowBackdating = credentialType.AllowBackdating,
                RolePlayersRequired = rolePlayersRequired,
                AutomaticIssuing = credentialType.ActiveTestSpecification?.AutomaticIssuing ?? false,
                MaxScoreDifference = credentialType.ActiveTestSpecification?.MaxScoreDifference,
                TestSessionBookingAvailabilityWeeks = credentialType.TestSessionBookingAvailabilityWeeks,
                TestSessionBookingClosedWeeks = credentialType.TestSessionBookingClosedWeeks,
                TestSessionBookingRejectHours = credentialType.TestSessionBookingRejectHours,
                AllowAvailabilityNotice = credentialType.AllowAvailabilityNotice,
                SkillType = credentialType.SkillType.Id
            };
        }

        internal static CredentialApplicationTypeCredentialTypeDto CredentialApplicationTypeCredentialTypeToDto(CredentialApplicationTypeCredentialType credentialApplicationTypeCredentialType)
        {
            return new CredentialApplicationTypeCredentialTypeDto
            {
                Id = credentialApplicationTypeCredentialType.Id,
                CredentialApplicationTypeId = credentialApplicationTypeCredentialType.CredentialApplicationType.Id,
                CredentialTypeId = credentialApplicationTypeCredentialType.CredentialType.Id,
                HasTest = credentialApplicationTypeCredentialType.HasTest,
                AllowSupplementary = credentialApplicationTypeCredentialType.AllowSupplementary,
                AllowPaidReview = credentialApplicationTypeCredentialType.AllowPaidReview
            };
        }


    }
}