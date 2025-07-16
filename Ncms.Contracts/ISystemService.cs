using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts
{
    public interface ISystemService
    {
        string GetSystemValue(string systemValueKey, bool forceRefresh = false);
        void SetSystemValue(string systemValueKey, string value);
        void SetSystemValues(IDictionary<string, string> values);
        IList<SystemValueModel> GetAllSystemValues();
        ConfigDetailsModel GetConfigDetails();
        void ObtainGraphAccessToken(string accessCode);
        void ObtainWiiseAccessToken(string accessCode);
        IEnumerable<LanguageSearchResultModel> LanguageSearch(SearchRequest request);
        LanguageSearchResultModel GetLanguage(int languageId);
        SaveResponse SaveLanguage(LanguageRequest language);
        GenericResponse<IEnumerable<SkillSearchResultModel>> SkillSearch(SearchRequest request);
        IEnumerable<VenueSearchResultModel> VenueSearch(SearchRequest request);
        VenueSearchResultModel GetVenue(int venueId);
        SaveResponse SaveVenue(VenueRequest venue);

        /// <summary>
        /// Gets all Organisations with API Access
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<ApiAdminSearchResultModel> ApiAdminSearch(SearchRequest request);

        /// <summary>
        /// Get details of Organisation's API Access
        /// </summary>
        /// <param name="venueId"></param>
        /// <returns></returns>
        IEnumerable<ApiAdminSearchResultModel> GetApiAdmin(int apiAccessId);

        /// <summary>
        /// Saves API Details for an Organisation
        /// </summary>
        /// <param name="venue"></param>
        /// <returns></returns>
        SaveResponse SaveApiAdmin(ApiAdminRequest apiAdminRequest);

        SkillSearchResultModel GetSkill(int skillId);
        SaveResponse SaveSkill(SkillRequest skill);
        void ThrowFrontEndException();
        void ThrowBackEndException();
        void ThrowUserFriendlyException();
        string GetEnvironmentName();
        GenericResponse<IEnumerable<object>> ValidateSystemValues(SystemValueRequest[] systemValuePairs);

        /// <summary>
        /// Purpose built call to retrieve file delete report
        /// which is SP [GetRemainingStoredFilesForDeletionCount]
        /// </summary>
        /// <returns></returns>
        GenericResponse<string> GetFileDeleteReport();

        /// <summary>
        /// Returns the current Permissions defined for API access
        /// </summary>
        /// <returns></returns>
        GenericResponse<IEnumerable<LookupTypeModel>> GetApiPermissionOptions();
    }

    public class SystemValueModel
    {
        public int Id { get; set; }
        public string ValueKey { get; set; }
        public string Value { get; set; }
    }

    public class UsersSearchResultModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public int OfficeId { get; set; }
        public IList<string> UserRoles { get; set; }
        public bool Active { get; set; }
    }

    public class LanguageSearchResultModel
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? GroupLanguageId { get; set; }
        public string GroupLanguageName { get; set; }
        public string Note { get; set; }
        public IEnumerable<SkillSearchResultModel> SkillsAttached { get; set; }
    }

    public class VenueSearchResultModel
    {
        public int VenueId { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string PublicNotes { get; set; }
        public string Address { get; set; }
        public int? TestLocationId { get; set; }
        public string Coordinates { get; set; }
        public bool Inactive { get; set; }
        public bool Active { get; set; }
    }

    public class ApiAdminSearchResultModel
    {
        public int ApiAccessId { get; set; }
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public bool Active { get; set; }
        public string PermissionLabels { get; set; }
        public Dictionary<string,bool> Permissions { get; set; }
        public List<int> PermissionIds { get; set; }
        public int OrgNaatiNumber { get; set; }
    }

    public class SkillSearchResultModel
    {
		public int SkillId { get; set; }
		public int SkillTypeId { get; set; }
		public int Language1Id { get; set; }
		public int? Language2Id { get; set; }
		public int DirectionTypeId { get; set; }
		public string TypeDisplayName { get; set; }
		public string DisplayName { get; set; }
		public int NumberOfExistingCredentials { get; set; }
		public int NumberOfCredentialRequests { get; set; }
        public IEnumerable<LookupTypeModel> ApplicationTypes { get; set; }
        public string Note { get; set; }
    }

    public class SkillRequest
    {
        public int? SkillId { get; set; }
        public int SkillTypeId { get; set; }
        public int Language1Id { get; set; }
        public int? Language2Id { get; set; }
        public int DirectionTypeId { get; set; }
        public IEnumerable<int> CredentialApplicationTypeId { get; set; }
        public string Note { get; set; }
    }

    public class LanguageRequest
    {
        public int? LanguageId { get; set; }
        public int? GroupLanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
    }

    public class VenueRequest
    {
        public int? VenueId { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string PublicNotes { get; set; }
        public string Address { get; set; }
        public int? TestLocationId { get; set; }
        public string Coordinates { get; set; }
        public bool Inactive { get; set; }
        public bool Active { get; set; }
    }

    public class ApiAdminRequest
    {
        public int? ApiAccessId { get; set; }
        public bool Active { get; set; }
        public int InstitutionId { get; set; }
        public IEnumerable<int> PermissionIds { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
}

    public class SaveResponse
    {
        public int Id { get; set; }
    }

    public class UserRolesModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class UserPermissionsModel
    {
        public SecurityNounName Noun { get; set; }
        public string NounName { get; set; }
        public string NounDisplayName { get; set; }
        public long Permissions { get; set; }
    }

    public class ConfigDetailsModel
    {
        public string DatabaseName { get; set; }
    }
}
