using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.FileDeletion;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.Office365;
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal
{
   public class SystemQueryService : ISystemQueryService
    {
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public SystemQueryService(ISecretsCacheQueryService secretsProvider, IAutoMapperHelper autoMapperHelper)
        {
            _secretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
        }

        public GetSystemValueResponse GetSystemValue(GetSystemValueRequest request)
        {
            var sysValue = GetSystemValue(request.ValueKey);
            return new GetSystemValueResponse { Value = sysValue };
        }

        private string GetSystemValue(string key)
        {
            key = key.ToUpper();
          
            var sysValue = NHibernateSession.Current.Query<SystemValue>().SingleOrDefault(x => x.ValueKey.ToUpper() == key);
            if (sysValue != null)
            {
                NHibernateSession.Current.Refresh(sysValue);
            }
          
            return sysValue?.Value;
        }

        public void SetSystemValue(SetSystemValueRequest request)
        {
            SetSystemValue(request.ValueKey, request.Value);
            NHibernateSession.Current.Flush();
        }

        public void SetSystemValues(SetSystemValuesRequest request)
        {
            foreach (var value in request.Values)
            {
                SetSystemValue(value.Key, value.Value);
            }
            NHibernateSession.Current.Flush();
        }

        private void SetSystemValue(string valueKey, string value)
        {
            var key = valueKey.ToUpper();
            var sysValue = NHibernateSession.Current.Query<SystemValue>().SingleOrDefault(x => x.ValueKey.ToUpper() == key);
            if (sysValue == null)
            {
                sysValue = new SystemValue();
                sysValue.ValueKey = valueKey;
            }
            sysValue.Value = value;
            NHibernateSession.Current.Save(sysValue);
        }

        public GetAllSystemValuesResponse GetAllSystemValues()
        {
            var sysValues = NHibernateSession.Current.Query<SystemValue>().ToArray();
            return new GetAllSystemValuesResponse { SystemValues = sysValues.Select(_autoMapperHelper.Mapper.Map<SystemValueDto>) };
        }

        public GetConfigDetailsResponse GetConfigDetails()
        {
            var connectionString = _secretsProvider.Get("ConnectionString");
            var dbName = connectionString.Split(new[] { "Initial Catalog=" }, 2, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(';')[0];
            return new GetConfigDetailsResponse
            {
                ConfigDetails = new ConfigDetailsDto
                {
                    DatabaseName = dbName
                }
            };
        }

        public void ObtainGraphAccessToken(string accessCode)
        {
            try
            {
                var tokenService = new AzureAuthorisationService(this, _secretsProvider, false);
                var newToken = tokenService.GetToken(accessCode);
                SetSystemValue("MicrosoftGraphAccessToken", newToken);
                NHibernateSession.Current.Flush();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Something went wrong while getting and saving a new Graph token");
                throw;
            }
        }

        public void ObtainWiiseAccessToken(string accessCode)
        {
            try
            {
                var tokenService = new WiiseAuthorisationService(this, _secretsProvider, false);
                var tenantId = String.Empty;
                var newToken = tokenService.GetTokenAndTenant(accessCode, out tenantId);
                SetSystemValue("WiiseAccessToken", newToken);
                SetSystemValue("WiiseTenantId", tenantId);
                NHibernateSession.Current.Flush();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Something went wrong while getting and saving a new Wiise token");
                throw;
            }
        }

        public LanguageSearchResponse LanguageSearch(LanguageSearchRequest request)
        {
            var queryHelper = new LanguageQueryHelper();
            var result = queryHelper.SearchLanguages(request);
            return new LanguageSearchResponse { Results = result };
        }

        public VenueSearchResponse VenueSearch(VenueSearchRequest request)
        {
            var queryHelper = new VenueQueryHelper();
            var result = queryHelper.SearchVenues(request);
            return new VenueSearchResponse { Results = result };
        }

        public SkillSearchResponse SkillSearch(SkillSearchRequest request)
        {
            var queryHelper = new SkillQueryHelper();
            var result = queryHelper.SearchSkills(request);
            return new SkillSearchResponse { Results = result };
        }

        public ServiceResponse<int> SaveSkill(SaveSkillRequest request)
        {
            var session = NHibernateSession.Current;
            var skill = new Skill();

            if (request.SkillId.HasValue)
            {
                skill = session.Load<Skill>(request.SkillId);
                skill.NotNull($"Skill is not found (Skill ID {request.SkillId})");
            }

            var skillType = session.Load<SkillType>(request.SkillTypeId);
            skillType.NotNull($"Skill Type is not found (Skill Type ID {request.SkillTypeId})");

            var directionType = session.Load<DirectionType>(request.DirectionTypeId);
            directionType.NotNull($"Direction Type is not found (Direction Type ID {request.DirectionTypeId})");

            var language1 = session.Load<Language>(request.Language1Id);
            language1.NotNull($"Language is not found (Language ID {request.Language1Id})");

            Language language2 = null;

            if (request.Language2Id.HasValue)
            {
                language2 = session.Load<Language>(request.Language2Id);
                language2.NotNull($"Language is not found (Language ID {request.Language2Id})");
            }
            else if (directionType.Id != (int)DirectionTypeName.L1)
            {
                throw new WebServiceException($"Language 2 is not defined");
            }
            else
            {
                language2 = language1;
            }

            var response = new ServiceResponse<int>();

            if (language1.Id == language2.Id && directionType.Id != (int)DirectionTypeName.L1)
            {
                response.ErrorMessage = $"Language 1 and Language 2 are the same.";
                response.Error = true;
                return response;
            }

            var query = session.Query<Skill>();
            var duplicated = query.FirstOrDefault(s =>
                s.Id != request.SkillId.GetValueOrDefault() && s.SkillType.Id == skillType.Id &&
                (
                    (directionType == s.DirectionType && s.Language1 == language1 && s.Language2 == language2) ||
                    (
                           (s.Language1 == language2 && s.Language2 == language1) && (
                                (directionType.Id == (int)DirectionTypeName.L1ToL2 && s.DirectionType.Id == (int)DirectionTypeName.L2ToL1) ||
                                 (directionType.Id == (int)DirectionTypeName.L2ToL1 && s.DirectionType.Id == (int)DirectionTypeName.L1ToL2))
                   )
                ));

            if (duplicated != null)
            {
                response.ErrorMessage = $"A skill with the same skill type and language combination already exists: Skill ID {duplicated.Id}";
                response.Error = true;
                return response;
            }

            var user = session.Load<User>(request.UserId);
            user.NotNull($"User is not found (User ID {request.UserId})");

            skill.SkillType = skillType;
            skill.DirectionType = directionType;
            skill.Language1 = language1;
            skill.Language2 = language2;
            skill.ModifiedUser = user;
            skill.ModifiedDate = DateTime.Now;
            skill.ModifiedByNaati = true;
            skill.Note = request.Note;

            var delete = skill.SkillApplicationTypes.Where(s => !request.CredentialApplicationTypeId.Contains(s.Id)).ToList();
            foreach (var d in delete)
            {
                skill.RemoveSkillApplicationType(d);
            }

            session.Save(skill);

            var ids = skill.SkillApplicationTypes.Select(sat => sat.CredentialApplicationType.Id).ToList();
            var add = request.CredentialApplicationTypeId.Where(s => !ids.Contains(s));
            foreach (var a in add)
            {
                var applicationType = session.Load<CredentialApplicationType>(a);
                skill.AddSkillApplicationType(new SkillApplicationType
                {
                    Skill = skill,
                    CredentialApplicationType = applicationType,
                    ModifiedUser = user,
                    ModifiedDate = DateTime.Now,
                    ModifiedByNaati = true
                });
            }

            session.Flush();

            response.Data = skill.Id;

            return response;
        }

        public ServiceResponse<int> SaveLanguage(SaveLanguageRequest request)
        {
            var language = new Language();

            if (request.LanguageId.HasValue)
            {
                language = NHibernateSession.Current.Load<Language>(request.LanguageId);
                if (language == null)
                {
                    throw new WebServiceException($"Language is not found (Language ID {request.LanguageId})");
                }
            }

            LanguageGroup languageGroup = null;

            if (request.GroupLanguageId.HasValue)
            {
                languageGroup = NHibernateSession.Current.Load<LanguageGroup>(request.GroupLanguageId);
                if (languageGroup == null)
                {
                    throw new WebServiceException($"Language Group is not found (Language Group ID {request.GroupLanguageId})");
                }
            }

            var user = NHibernateSession.Current.Load<User>(request.UserId);
            user.NotNull($"User is not found (User ID {request.UserId})");

            var query = NHibernateSession.Current.Query<Language>();
            var duplicated = query.FirstOrDefault(l => l.Id != request.LanguageId.GetValueOrDefault() && l.Name == request.Name);

            var response = new ServiceResponse<int>();
            if (duplicated != null)
            {
                response.ErrorMessage = $"A language with this name already exists: Language ID {duplicated.Id}";
                response.Error = true;
                return response;
            }

            language.Name = request.Name;
            language.Code = request.Code;
            language.LanguageGroup = languageGroup;
            language.ModifiedUser = user;
            language.ModifiedDate = DateTime.Now;
            language.ModifiedByNaati = true;
            language.Note = request.Note;

            NHibernateSession.Current.Save(language);
            NHibernateSession.Current.Flush();

            response.Data = language.Id;

            return response;
        }

        public SaveResponse SaveVenue(SaveVenueRequest request)
        {
            var venue = new Venue();

            if (request.VenueId.HasValue)
            {
                venue = NHibernateSession.Current.Get<Venue>(request.VenueId);
                venue.NotNull($"Venue is not found (Venue ID {request.VenueId})");
            }

            var user = NHibernateSession.Current.Get<User>(request.ModifiedUser);
            user.NotNull($"User is not found (User ID {request.ModifiedUser})");

            var testLocation = NHibernateSession.Current.Get<TestLocation>(request.TestLocationId);
            testLocation.NotNull($"Test Location is not found (Test Location ID {request.TestLocationId})");

            var existingVenue = NHibernateSession.Current
                .Query<Venue>().Any(x => x.Id != request.VenueId.GetValueOrDefault() && x.Name == request.Name.Trim() && x.TestLocation.Id == request.TestLocationId);

            var response = new SaveResponse();
            if (existingVenue)
            {
                response.ErrorMessage = "Venue name already exists for selected test location.";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(request.Coordinates))
            {
                var coordinates = request.Coordinates.Split(',');
                if (coordinates.Length != 2)
                {
                    response.ErrorMessage = "Invalid coordinates";
                    return response;
                }

                double value;
                if (!double.TryParse(coordinates[0], out value) || !double.TryParse(coordinates[1], out value))
                {
                    response.ErrorMessage = "Invalid coordinates";
                    return response;
                }
            }

            var futureDatedTestSessions = NHibernateSession.Current.Query<Venue>().Where(x => x.Id == request.VenueId)
                                                                                  .SelectMany(x => x.TestSessions)
                                                                                  .Where(x => x.TestDateTime >= DateTime.Now).ToList();
            if (futureDatedTestSessions.Count > 0 && request.Inactive == true)
            {
                response.ErrorMessage = $"Venue cannot be set as inactive as it has upcoming test sessions.";
                return response;
            }

            venue.Name = request.Name;
            venue.TestLocation = testLocation;
            venue.Address = request.Address;
            venue.Coordinates = request.Coordinates;
            venue.Capacity = request.Capacity;
            venue.PublicNotes = request.PublicNotes;
            venue.Inactive = request.Inactive;

            venue.ModifiedByNaati = true;
            venue.ModifiedUser = user;
            venue.ModifiedDate = DateTime.Now;

            NHibernateSession.Current.Save(venue);
            NHibernateSession.Current.Flush();

            response.Id = venue.Id;

            return response;
        }

        public void ThrowException()
        {
            LoggingHelper.LogException(new Exception("Test (not thrown)"), "Logging test");
            throw new Exception("This exception was thrown deliberately for the purpose of testing.");
        }

        public GenericResponse<string> GetFileDeleteReport()
        {

            var estimationDetails = NHibernateSession.Current.TransformSqlQueryDataRowResult<FileDeletionEstimationDetails>($"exec GetRemainingStoredFilesForDeletionCount null, 0").FirstOrDefault();

            if (estimationDetails.IsNull())
            {
                return "Could not get count of files remaining for deletion from the GetRemainingStoredFilesForDeletionCount stored procedure";
            }

            return new GenericResponse<string>(estimationDetails.Log);
        }

        public ApiAdminSearchResponse ApiAdminSearch(int ApiAccessId = 0)
        {
            var response = NHibernateSession.Current.Query<ApiAccess>().Select(x=> new ApiAdminSearchResultDto()
            {
                ApiAccessId = x.Id,
                PrivateKey = x.PrivateKey,
                PublicKey = x.PublicKey,
                InActive = x.Inactive,
                Permissions = x.Permissions,
                Name = x.Institution.InstitutionName,
                InstitutionId = x.Institution.Id,
                OrgNaatiNumber = x.Institution.Entity.NaatiNumber
            });

            if(ApiAccessId > 0)
            {
                response = response.Where(x => x.ApiAccessId == ApiAccessId);
            }

            return new ApiAdminSearchResponse()
            {
                Results = response
            };
        }

        public SaveResponse SaveApiAdmin(SaveApiAdminRequest request)
        {
            var apiAccess = new ApiAccess();

            if (request.ApiAccessId.HasValue)
            {
                apiAccess = NHibernateSession.Current.Get<ApiAccess>(request.ApiAccessId);
                if (apiAccess == null){
                    throw new UserFriendlySamException($"Api Client record is not found (Venue ID {request.ApiAccessId})");
                }
            }

            if (!request.ApiAccessId.HasValue)
            {
                var clientRecordAlreadyExists = NHibernateSession.Current.Query<ApiAccess>().FirstOrDefault(x => x.Institution.Id == request.InstitutionId && x.Inactive == false);
                if (clientRecordAlreadyExists != null)
                {
                    throw new UserFriendlySamException($"Api Client record already exists (InstitutionId ID {request.InstitutionId})");
                }
                apiAccess.CreatedDate = DateTime.Now;
            }

            var user = NHibernateSession.Current.Get<User>(request.ModifiedUser);
            user.NotNull($"User is not found (User ID {request.ModifiedUser})");

            var institution = NHibernateSession.Current.Get<Institution>(request.InstitutionId);
            institution.NotNull($"Test Location is not found (Test Location ID {request.InstitutionId})");

            var existingApiAccess = NHibernateSession.Current
                .Query<ApiAccess>().Any(x => x.Id != request.InstitutionId);

            var response = new SaveResponse();

            apiAccess.Institution = institution;
            apiAccess.PublicKey = request.PublicKey;
            apiAccess.PrivateKey = request.PrivateKey;
            apiAccess.Permissions = request.Permissions;
            apiAccess.Inactive = request.Inactive;

            apiAccess.ModifiedUser = user;
            apiAccess.ModifiedDate = DateTime.Now;

            NHibernateSession.Current.Save(apiAccess);
            NHibernateSession.Current.Flush();

            response.Id = apiAccess.Id;

            return response;
        }
    }
}