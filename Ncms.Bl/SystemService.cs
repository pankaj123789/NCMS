using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using Ncms.Bl.Extensions;
using Ncms.Contracts;
using Ncms.Contracts.Models.File;
using SaveResponse = Ncms.Contracts.SaveResponse;
using SearchRequest = Ncms.Contracts.SearchRequest;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl
{
    public class SystemService : ISystemService
    {
        private ISystemQueryService _systemQueryService;
        private IUserService _userService;
        private IFileDeletionDalService _fileDeletionDalService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public SystemService(ISystemQueryService systemService, IUserService userService, IUtilityQueryService utilityQueryService, IUserQueryService userQueryService,
            IFileDeletionDalService fileDeletionDalService, IAutoMapperHelper autoMapperHelper)
        {
            _systemQueryService = systemService;
            _userService = userService;
            _fileDeletionDalService = fileDeletionDalService;
            _autoMapperHelper = autoMapperHelper;
        }

        public string GetSystemValue(string systemValueKey, bool forceRefresh = false)
        {
            GetSystemValueResponse response = null;
            var request = new GetSystemValueRequest { ValueKey = systemValueKey, ForceRefresh = forceRefresh };
            response = _systemQueryService.GetSystemValue(request);
            return response.Value;
        }

        public void SetSystemValue(string systemValueKey, string value)
        {
            var request = new SetSystemValueRequest();
            request.ValueKey = systemValueKey;
            request.Value = value;
            _systemQueryService.SetSystemValue(request);
        }

        public IList<SystemValueModel> GetAllSystemValues()
        {
            GetAllSystemValuesResponse response = null;
            response = _systemQueryService.GetAllSystemValues();
            return response.SystemValues.Select(_autoMapperHelper.Mapper.Map<SystemValueModel>).ToList();
        }

        public void SetSystemValues(IDictionary<string, string> values)
        {
            var request = new SetSystemValuesRequest();
            request.Values = values;
            _systemQueryService.SetSystemValues(request);
        }

        public ConfigDetailsModel GetConfigDetails()
        {
            GetConfigDetailsResponse response = null;
            response = _systemQueryService.GetConfigDetails();
            return _autoMapperHelper.Mapper.Map<ConfigDetailsModel>(response.ConfigDetails);
        }

        public void ObtainGraphAccessToken(string accessCode)
        {
            _systemQueryService.ObtainGraphAccessToken(accessCode);
        }

        public void ObtainWiiseAccessToken(string accessCode)
        {
            _systemQueryService.ObtainWiiseAccessToken(accessCode);
        }

        public IEnumerable<LanguageSearchResultModel> LanguageSearch(SearchRequest request)
        {
            var getRequest = new LanguageSearchRequest
            {
                Take = request.Take,
                Skip = request.Skip,
                Filters = request.Filter.ToFilterList<LanguageSearchCriteria, LanguageFilterType>()
            };

            return _systemQueryService.LanguageSearch(getRequest).Results.Select(_autoMapperHelper.Mapper.Map<LanguageSearchResultModel>).ToList();
        }

        public LanguageSearchResultModel GetLanguage(int languageId)
        {
            var getRequest = new LanguageSearchRequest
            {
                Filters = new[]{
                    new LanguageSearchCriteria {
                        Filter = LanguageFilterType.LanguageIdIntList,
                        Values = new[]{ languageId.ToString() }
                    }
                }
            };

            var results = _systemQueryService.LanguageSearch(getRequest).Results;
            var language = _autoMapperHelper.Mapper.Map<LanguageSearchResultModel>(results.FirstOrDefault());
            var skillFilter = new[] {
                    new SkillSearchCriteria {
                        Filter  = SkillFilterType.LanguageIntList,
                        Values = new[]{ languageId.ToString() }
                    }
                };

            language.SkillsAttached = SkillSearch(skillFilter)
                .Select(_autoMapperHelper.Mapper.Map<SkillSearchResultModel>);

            return language;
        }

        public VenueSearchResultModel GetVenue(int venueId)
        {
            var getRequest = new VenueSearchRequest
            {
                Filters = new[]{
                    new VenueSearchCriteria {
                        Filter = VenueFilterType.VenueIdIntList,
                        Values = new[]{ venueId.ToString() }
                    }
                }
            };

            var results = _systemQueryService.VenueSearch(getRequest).Results;
            var venue = _autoMapperHelper.Mapper.Map<VenueSearchResultModel>(results.FirstOrDefault());

            return venue;
        }

        public SaveResponse SaveVenue(VenueRequest venue)
        {
            var request = _autoMapperHelper.Mapper.Map<SaveVenueRequest>(venue);
            request.ModifiedUser = _userService.Get().Id;
            request.ModifiedByNaati = true;
            request.ModifiedDate = DateTime.Now;

            var response = _systemQueryService.SaveVenue(request);

            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            return new SaveResponse { Id = response.Id };
        }

        public IEnumerable<VenueSearchResultModel> VenueSearch(SearchRequest request)
        {
            var getRequest = new VenueSearchRequest
            {
                Take = request.Take,
                Skip = request.Skip,
                Filters = request.Filter.ToFilterList<VenueSearchCriteria, VenueFilterType>()
            };

            return _systemQueryService.VenueSearch(getRequest)
                .Results
                .Select(_autoMapperHelper.Mapper.Map<VenueSearchResultModel>)
                .ToList();
        }

        public IEnumerable<ApiAdminSearchResultModel> GetApiAdmin(int apiAccessId)
        {
            var results = _systemQueryService.ApiAdminSearch(apiAccessId).Results;

            return results.Select(x => ApiPermissions.ToSearchResultModel(x));
        }

        public SaveResponse SaveApiAdmin(ApiAdminRequest apiAdminRequest)
        {
            var saveApiAdminRequest = new SaveApiAdminRequest()
            {
                ApiAccessId = apiAdminRequest.ApiAccessId,
                Inactive = !apiAdminRequest.Active,
                InstitutionId = apiAdminRequest.InstitutionId,
                Permissions = apiAdminRequest.PermissionIds.Sum(),
                PublicKey = apiAdminRequest.PublicKey,
                PrivateKey = apiAdminRequest.PrivateKey,
                ModifiedUser = _userService.Get().Id
            };

            var response = _systemQueryService.SaveApiAdmin(saveApiAdminRequest);

            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            return new SaveResponse { Id = response.Id };


        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetApiPermissionOptions()
        {
            return ApiPermissions.GetApiPermissions();
        }



        public IEnumerable<ApiAdminSearchResultModel> ApiAdminSearch(SearchRequest request)
        {
            var results = new List<ApiAdminSearchResultModel>();

            var searchResults = _systemQueryService.ApiAdminSearch().Results;

            foreach (var result in searchResults)
            {
                results.Add(new ApiAdminSearchResultModel()
                {
                    ApiAccessId = result.ApiAccessId,
                    Active = !result.InActive,
                    Name = result.Name,
                    PrivateKey = result.PrivateKey,
                    PublicKey = result.PublicKey,
                    OrgNaatiNumber = result.OrgNaatiNumber,
                    Permissions = ApiPermissions.ToDictionary(result.Permissions)
                });
            }

            return results;
        }


        public GenericResponse<IEnumerable<SkillSearchResultModel>> SkillSearch(SearchRequest request)
        {
            var response = new GenericResponse<IEnumerable<SkillSearchResultModel>>();

            var result = SkillSearch(request.Filter.ToFilterList<SkillSearchCriteria, SkillFilterType>(), request.Take, request.Skip)
                .Select(_autoMapperHelper.Mapper.Map<SkillSearchResultModel>).ToList();

            if (request.Take.HasValue && result.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }
            response.Data = result;
            return response;
        }

        private IEnumerable<SkillSearchResultDto> SkillSearch(IEnumerable<SkillSearchCriteria> filters, int? take = null, int? skip = null)
        {
            var getRequest = new SkillSearchRequest
            {
                Take = take,
                Skip = skip,
                Filters = filters
            };

            return _systemQueryService.SkillSearch(getRequest).Results;
        }

        public SkillSearchResultModel GetSkill(int skillId)
        {
            var getRequest = new SkillSearchRequest
            {
                Filters = new[]{
                    new SkillSearchCriteria {
                        Filter = SkillFilterType.SkillIdIntList,
                        Values = new[]{ skillId.ToString() }
                    }
                }
            };

            var results = _systemQueryService.SkillSearch(getRequest).Results;
            return _autoMapperHelper.Mapper.Map<SkillSearchResultModel>(results.FirstOrDefault());
        }

        public SaveResponse SaveSkill(SkillRequest skill)
        {
            var request = _autoMapperHelper.Mapper.Map<SaveSkillRequest>(skill);
            request.UserId = _userService.Get().Id;
            var response = _systemQueryService.SaveSkill(request);

            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            return new SaveResponse
            {
                Id = response.Data
            };
        }
        public SaveResponse SaveLanguage(LanguageRequest language)
        {
            var request = _autoMapperHelper.Mapper.Map<SaveLanguageRequest>(language);
            request.UserId = _userService.Get().Id;
            var response = _systemQueryService.SaveLanguage(request);

            if (!String.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            return new SaveResponse { Id = response.Data };
        }

        public void ThrowFrontEndException()
        {
            throw new Exception("This exception was thrown deliberately for the purpose of testing.");
        }

        public void ThrowBackEndException()
        {
            _systemQueryService.ThrowException();
        }

        public void ThrowUserFriendlyException()
        {
            throw new UserFriendlySamException("This exception was thrown deliberately for the purpose of testing.");
        }

        public string GetEnvironmentName()
        {
            return new EnvironmentConfigurationHelper().GetEnvironmentDetails().EnvironmentDisplayName;
        }

        public GenericResponse<IEnumerable<object>> ValidateSystemValues(SystemValueRequest[] systemValuePairs)
        {
            var errors = new List<object>();

            foreach(var systemValuePair in systemValuePairs)
            {
                ValidateNullValue(systemValuePair.Key, systemValuePair.Value, errors);

                switch (systemValuePair.DataType.ToLower())
                {
                    case ("int"):
                        try 
                        {
                            int.Parse(systemValuePair.Value);
                        }
                        catch
                        {
                            AddError(systemValuePair.Key, $"{systemValuePair.Key} must be a whole number.", errors);
                        }
                        break;
                    case ("bool"):
                        break;
                    case ("string"):
                        break;
                    default:
                        throw new Exception($"DataType {systemValuePair.DataType} for System Value {systemValuePair.Key} is not valid.");
                }
            }

            return new GenericResponse<IEnumerable<object>>(errors);
        }

        private void ValidateNullValue(string property, object value, List<object> errors)
        {
            if (value == null)
            {
                AddError(property, Naati.Resources.Shared.RequiredFieldValidationError, errors);
            }
        }

        private void AddError(string property, string message, IList<object> errors)
        {
            errors.Add(new
            {
                FieldName = property,
                Message = message
            });
        }

        public GenericResponse<string> GetFileDeleteReport()
        {
            // stringbuilder to build the final output
            var finalReport = new StringBuilder();

            // get the count of files deleted for each day for the last 7 days including today
            var filesDeletedInLast7DaysResponse = _fileDeletionDalService.GetCountOfFilesDeletedEachDayLast7Days();
            // set total to 0. This will be added up to get the total over all 7 days
            var totalCountOfFilesDeleted = 0;
            // reverse the list to log the most recent day first
            foreach (var dateAndCount in filesDeletedInLast7DaysResponse.Data.Reverse())
            {
                finalReport.AppendLine($"{dateAndCount.Value} files were processed on {dateAndCount.Key}");
                // add to total count
                totalCountOfFilesDeleted += dateAndCount.Value;
            }
            // log total files processed for the whole 7 days
            finalReport.AppendLine($"{totalCountOfFilesDeleted} files processed in the last 7 days.");

            finalReport.AppendLine(_systemQueryService.GetFileDeleteReport().Data);

            return finalReport.ToString();
        }
    }
}
