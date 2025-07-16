using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using System;
using System.Linq;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Global.Common.Mapping;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Global.Common.Logging;
using System.Threading;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Request;

namespace F1Solutions.Naati.Common.Dal
{
    public class CredentialQrCodeDalService : ICredentialQrCodeDalService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly ISecretsCacheQueryService _secretsProvider;

        public CredentialQrCodeDalService(IAutoMapperHelper autoMapperHelper, ISecretsCacheQueryService secretsProvider)
        {
            _autoMapperHelper = autoMapperHelper;
            _secretsProvider = secretsProvider;
        }
        public GenericResponse<CredentialQrCodeDto> GetCredentialQRCodeForStampAndId(int credentialId)
        {
            //get credential and check for QRCodes
            var credential =  NHibernateSession.Current.Get<Credential>(credentialId);

            if(credential == null)
            {
                return new GenericResponse<CredentialQrCodeDto>() { Success = false, Errors = new List<string>() {$"Credential does not exist for Id {credentialId}." } };
            }

            //TFS220404 removed the code to check if one had already been issued today. 

            return CreateNewCredentialQrCode(credential);
        }

        public GenericResponse<CredentialQrCodeDto> VerifyCredentialQRCode(Guid qrCodeGuid)
        {
            var credentialQrCode = NHibernateSession.Current.Query<CredentialQrCode>().FirstOrDefault(x => x.QrCodeGuid == qrCodeGuid);
            if(credentialQrCode == null)
            {
                return new GenericResponse<CredentialQrCodeDto>() { Success = false, Errors = new List<string>() { "Credential does not exist" } };
            }

            return credentialQrCode.ToCredentialQrCodeDto(_autoMapperHelper);
        }

        public GenericResponse<string> GetCredentialCatergoryByCredentialTypeExternalName(string credentialTypeExternalName)
        {
            var credentialCatergory = NHibernateSession.Current.Query<CredentialType>()
                .Where(x => x.ExternalName == credentialTypeExternalName)
                .Select(x => x.CredentialCategory).FirstOrDefault();

            if (credentialCatergory.IsNull())
            {
                return new GenericResponse<string>(null) { Success = false, Errors = new List<string>() { $"Could not get Credential Category for Credential Type {credentialTypeExternalName}" } };
            }

            var credentialCatergoryDisplayName = credentialCatergory.DisplayName;

            return new GenericResponse<string>(credentialCatergoryDisplayName) { Success = true };
        }

        public GenericResponse<Person> GetPerson(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().Where(x => x.Entity.NaatiNumber == naatiNumber).FirstOrDefault();

            if(person.IsNull())
            {
                return new GenericResponse<Person>(null) { Success = false, Errors = new List<string>() { $"Could not find a person with Naati Number {naatiNumber}." } };
            }

            return person;
        }

        public GenericResponse<bool> GetIsDeceasedFromPractitionerId(string practitionerId)
        {
            var person = NHibernateSession.Current.Query<Person>().Where(x => x.PractitionerNumber == practitionerId).FirstOrDefault();

            if (person.IsNull())
            {
                return new GenericResponse<bool>(false)
                {
                    Success = false,
                    Errors = new List<string>() { $"Could not get person for practitioner number {practitionerId}" }
                };
            }

            return person.Deceased;
        }

        /// <summary>
        /// PersonId is not in the QrCode table so we have to gets the person from the passed in code and then go
        /// backwards to get all the QrCodes
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        public GenericResponse<QrCodeVerificationModelDto> GetPractitionerVerificationModelFromQrCode(Guid qrCode)
        {
            var result = new QrCodeVerificationModelDto();
            var credentialQrCode = NHibernateSession.Current.Query<CredentialQrCode>().FirstOrDefault(x => x.QrCodeGuid == qrCode);
            if (credentialQrCode == null)
            {
                return (new GenericResponse<QrCodeVerificationModelDto>() { Success = false, Errors = new List<string>() { "Not a valid Practitioner Identifier" } });
            }
            var credentialCredentialRequest = credentialQrCode.Credential.CredentialCredentialRequests.First();
            var credential = NHibernateSession.Current.Query<Credential>().FirstOrDefault(x => x.Id == credentialCredentialRequest.Credential.Id);
            var person = credential.CertificationPeriod.Person;
            result.PractitionerId = person.PractitionerNumber;
            result.GeneratedOn = credentialQrCode.IssueDate;
            var allCredentials = NHibernateSession.Current.Query<CertificationPeriod>().Where(x => x.Person.Id == person.Id).SelectMany(x => x.Credentials);
            var allQrCodes = allCredentials.SelectMany(x => x.CredentialQrCodes);
            result.QrCodeModelDtos = allQrCodes.Select(x => new QrCodeModelDto() 
            { 
                QrCode = x.QrCodeGuid, 
                IssueDate = x.IssueDate, 
                InactiveDate = x.InactiveDate,
                AlowVerifyOnline = person.AllowVerifyOnline,
                IsDeceased = person.Deceased,
            }).ToList();
            return result;
        }

        public GenericResponse<bool> DoesCredentialBelongToUser(int credentialId, int currentUserNaatiNumber)
        {
            var credential = NHibernateSession.Current.Query<Credential>().FirstOrDefault(x => x.Id == credentialId);
            //no such credential
            if (credential == null)
            {
                LoggingHelper.LogError($"Credential {credentialId} does not belong to {currentUserNaatiNumber}");
                return new GenericResponse<bool>()
                {
                    Success = false
                };
            }
            var success = credential.CertificationPeriod.Person.NaatiNumberDisplay == currentUserNaatiNumber.ToString();
            //user does not have that credential
            if (!success)
            {
                LoggingHelper.LogError($"Credential {credentialId} does not belong to {currentUserNaatiNumber}");
                return new GenericResponse<bool>()
                {
                    Success = false
                };
            }
            return success;
        }

        private CredentialQrCodeDto CreateNewCredentialQrCode(Credential credential)
        {

            var newCredentialQrCode = new CredentialQrCode()
            {
                Credential = credential,
                IssueDate = DateTime.Now.Date,
                QrCodeGuid = Guid.NewGuid(),
                InactiveDate = null,
                ModifiedBy = 0,
                ModifiedDate = DateTime.Now.Date
            };

            newCredentialQrCode.Credential.AddQrCode(newCredentialQrCode);

            NHibernateSession.Current.Flush();

            var credentialQrCodeDto = newCredentialQrCode.ToCredentialQrCodeDto(_autoMapperHelper);

            newCredentialQrCode.CreateEntityNote(_secretsProvider);

            return credentialQrCodeDto;
        }
    }

    public static class CredentialQrCodeExtensions
    {
        public static CredentialQrCodeDto ToCredentialQrCodeDto(this CredentialQrCode credentialQrCode, IAutoMapperHelper _autoMapperHelper)
        {
            var credentialRequest = GetCredentialRequest(credentialQrCode);
            var person = GetPerson(credentialRequest);

            return new CredentialQrCodeDto()
            {
                FirstName = person.GivenName,
                LastName = person.Surname,
                MiddleName = person.OtherNames,
                Title = person.Title,
                IssueDate = credentialQrCode.IssueDate,
                QrCodeGuid = credentialQrCode.QrCodeGuid,
                PractitionerNumber = person.PractitionerNumber,
                CredentialDto = _autoMapperHelper.Mapper.Map<CredentialDto>(credentialQrCode.Credential),
                SkillDto = _autoMapperHelper.Mapper.Map<SkillDto>(credentialRequest.Skill),
                CredentialTypeName = credentialRequest.CredentialType.ExternalName,
                ModifiedDate = credentialQrCode.ModifiedDate
            };
        }

        public static void CreateEntityNote(this CredentialQrCode credentialQrCode, ISecretsCacheQueryService _secretsProvider)
        {
            var credentialRequest = GetCredentialRequest(credentialQrCode);
            var person = GetPerson(credentialRequest);

            var qrGeneratedMessage = $"{person.PrimaryEmailAddress} has generated a QR for a digital translation stamp for {credentialRequest.CredentialType.DisplayName} {credentialRequest.Skill.DisplayName}.";

            LoggingHelper.LogInfo(qrGeneratedMessage);

            var userName = _secretsProvider.Get("MyNaatiDefaultIdentity");

            var user = NHibernateSession.Current.Query<User>().Where(x => x.UserName == userName).FirstOrDefault();

            if (user.IsNull())
            {
                LoggingHelper.LogError($"Could not create note for credential QR generation for user {person.PrimaryEmailAddress} as no authenticated user could be found.");
                return;
            }

            var noteToCreate = new Note()
            {
                User = user,
                Description = qrGeneratedMessage,
                CreatedDate = DateTime.Now,
                ModifiedDate = null,
                Highlight = false,
                ReadOnly = true
            };

            NHibernateSession.Current.Save(noteToCreate);
            NHibernateSession.Current.Flush();

            var entityNote = new NaatiEntityNote()
            {
                Entity = NHibernateSession.Current.Get<NaatiEntity>(person.Entity.Id),
                Note = noteToCreate
            };

            NHibernateSession.Current.Save(entityNote);
            NHibernateSession.Current.Flush();
        }

        private static Person GetPerson(CredentialRequest credentialRequest)
        {
            var person = credentialRequest.CredentialApplication.Person.LatestPersonName.Person;

            return person;
        }

        private static CredentialRequest GetCredentialRequest(CredentialQrCode credentialQrCode)
        {
            var credentialRequest = credentialQrCode.Credential.CredentialCredentialRequests.First(x => x.CredentialRequest.CredentialRequestStatusType.Id == 12).CredentialRequest; //CertificationIssued

            return credentialRequest;
        }
    }
}
