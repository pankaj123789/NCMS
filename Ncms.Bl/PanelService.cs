using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.Panel;
using Newtonsoft.Json;

namespace Ncms.Bl
{
    public class PanelService : IPanelService
    {
        private readonly IPanelQueryService _panelQueryService;
        private readonly IExaminerToolsService _examinerToolsService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PanelService(IPanelQueryService panelQueryService, IExaminerToolsService examinerToolsService, IAutoMapperHelper autoMapperHelper)
        {
            _panelQueryService = panelQueryService;
            _examinerToolsService = examinerToolsService;
            _autoMapperHelper = autoMapperHelper;
        }
        public IList<PanelModel> List(string request)
        {
            var panelsRequest = JsonConvert.DeserializeObject<GetPanelsRequest>(request);
            GetPanelsResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.GetPanels(panelsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var panelDtoMapper = new PanelDtoMapper();
            return panelsResponse?.Panels.Select(panelDtoMapper.Map).ToList() ?? new List<PanelModel>();
        }

        public IList<PanelMembershipModel> ListMembership(string request)
        {
            var panelsRequest = JsonConvert.DeserializeObject<GetMembershipsRequest>(request);
            GetMembershipsResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.GetMemberships(panelsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var panelMembershipDtoMapper = new PanelMembershipDtoMapper();
            return panelsResponse?.People.Select(panelMembershipDtoMapper.Map).ToList() ?? new List<PanelMembershipModel>();
        }

        public GenericResponse<IEnumerable<PanelMembershipLookupModel>> GetPanelMembershipLookUp(GetPanelMemberLookupRequestModel request)
        {
            var getRequest = new GetPanelMemberLookupRequest { PanelIds = request.PanelIds, ActiveMembersOnly = request.ActiveMembersOnly, CredentialTypeId = request.CredentialTypeId};
            var response = _panelQueryService.GetPanelMembershipLookUp(getRequest);

            var models = response.Results.Select(_autoMapperHelper.Mapper.Map<PanelMembershipLookupModel>).ToList();
            return new GenericResponse<IEnumerable<PanelMembershipLookupModel>>(models);
        }

        public GenericResponse<PanelModel> GetPanel(int panelId)
        {
            var panel  = _panelQueryService.GetPanel(panelId)?.Panel;

            var result = _autoMapperHelper.Mapper.Map<PanelModel>(panel);
            return result;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetAvailableMembershipCredentialTypes(int panelId, int membershipId)
        {
            var panel = _panelQueryService.GetPanel(panelId).Panel;

            if (!GetPanelTypes().Data.First(x => x.Id == panel.PanelTypeId).AllowCredentialTypeLink)
            {
                return new GenericResponse<IEnumerable<LookupTypeModel>>(Enumerable.Empty<LookupTypeModel>());
            }
        
            var credentialTypes = ServiceLocator.Resolve<IApplicationQueryService>()
                .GetLookupType(LookupType.CredentialType.ToString())
                .Results;

            var result = credentialTypes.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>).ToList();
            return new GenericResponse<IEnumerable<LookupTypeModel>>(result);
        }
        
        public GenericResponse<IEnumerable<PanelTypeModel>> GetPanelTypes()
        {
            var response = _panelQueryService.GetPanelTypes();

            var result = response.Results.Select(_autoMapperHelper.Mapper.Map<PanelTypeModel>);
            return new GenericResponse<IEnumerable<PanelTypeModel>>(result);
        }

        public int CreateOrUpdate(PanelModel model)
        {
            var createPanelMapper = new CreatePanelMapper();
            var panelsRequest = createPanelMapper.Map(model);

            var panelsResponse = _panelQueryService.CreateOrUpdatePanel(panelsRequest);
            if (!string.IsNullOrWhiteSpace(panelsResponse.ErrorMessage))
            {
                throw new UserFriendlySamException(panelsResponse.ErrorMessage);
            }

            return panelsResponse.Data;
        }

        public int AddOrUpdateMember(PanelMembershipModel model)
        {
            var addPanelMembershipMapper = new AddPanelMembershipMapper();
            var panelsRequest = addPanelMembershipMapper.Map(model);
            var rolePlayerSettings = _examinerToolsService.GetRolePlayerSettings(new  GetRolePlayerSettingsRequest { NaatiNumber = model.NaatiNumber});
            
            panelsRequest.RolePlayerSessionLimit = rolePlayerSettings.Settings?.MaximumRolePlaySessions ?? 40;
            panelsRequest.RolePlayerRating = rolePlayerSettings.Settings?.Rating;
            panelsRequest.RolePlayerSenior = rolePlayerSettings.Settings?.Senior ?? false;


            if (model.CredentialTypeIds.Any())
            {
                var credentialTypes = GetAvailableMembershipCredentialTypes(model.PanelId, model.PanelMembershipId ?? 0);
                foreach (var credentialTypeId in model.CredentialTypeIds)
                {
                    if (credentialTypes.Data.All(x => x.Id != credentialTypeId))
                    {
                        throw new UserFriendlySamException(Naati.Resources.Panel.CredentialTypeAlreadyAssigned);
                    }
                }
            }

            AddOrUpdateMembershipResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.AddOrUpdateMembership(panelsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return panelsResponse.PanelMembershipId;
        }

        public PanelModel Delete(int panelId)
        {
            var panelsRequest = new DeletePanelRequest
            {
                PanelId = panelId
            };

            DeletePanelResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.DeletePanel(panelsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var panelDtoMapper = new PanelDtoMapper();
            return panelDtoMapper.Map(panelsResponse.Panel);
        }

        public PanelMembershipModel RemoveMember(int panelMembershipId)
        {
            var panelsRequest = new DeleteMembershipRequest
            {
                PanelMembershipId = panelMembershipId
            };

            DeleteMembershipResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.DeleteMembership(panelsRequest);
            }
            catch (Exception e)
            {
                //This is here because the WebServiceException was not being caught by the catch.
                //This is not a great solution but it will fix the issue for now.
                if (e.Message == "The panel membership cannot be deleted because there are tests tied to it")
                {
                    throw new UserFriendlySamException(e.Message);
                }
            }

            var panelMembershipDtoMapper = new PanelMembershipDtoMapper();
            return panelMembershipDtoMapper.Map(panelsResponse.Member);
        }

        public int[] ReappointMembers(ReappointMembersModel reappointMembersModel)
        {
            var panelsRequest = new ReappointMembersRequest
            {
                PanelId = reappointMembersModel.PanelId,
                PanelMembershipNumbers = reappointMembersModel.PanelMembershipNumbers,
                StartDate = reappointMembersModel.StartDate,
                EndDate = reappointMembersModel.EndDate
            };

            ReappointMembersResponse panelsResponse = null;

            try
            {
                panelsResponse = _panelQueryService.ReappointMembers(panelsRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return panelsResponse.PanelMembershipIds;
        }

        public IList<ExaminerUnavailability> GetUnavailability(int panelMembershipId)
        {
            var request = new GetUnavailabilityRequest
            {
                PanelMembershipId = panelMembershipId
            };

            GetUnavailabilityResponse response = null;

            try
            {
                response = _panelQueryService.GetUnavailability(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var mapper = new ExaminerUnavailabilityDtoMapper();
            return response?.Unavailability?.Where(u => u.StartDate < DateTime.Now.AddMonths(12)).Select(mapper.Map).ToList() ?? new List<ExaminerUnavailability>();
        }

        public IList<MarkingRequest> GetMarkingRequests(int panelMembershipId)
        {
            var request = new GetMarkingRequestsRequest
            {
                PanelMembershipId = panelMembershipId
            };

            GetMarkingRequestsResponse response = null;

            try
            {
                response = _panelQueryService.GetMarkingRequests(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var mapper = new MarkingRequestDtoMapper();
            return response?.MarkingRequests?.Select(mapper.Map).ToList() ?? new List<MarkingRequest>();
        }

        public IList<MaterialRequest> GetMaterialRequests(int panelMembershipId)
        {
            var request = new GetMaterialRequestsRequest
            {
                PanelMembershipId = panelMembershipId
            };

            GetMaterialRequestsResponse response = null;

            try
            {
                response = _panelQueryService.GetMaterialRequests(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var mapper = new MaterialRequestDtoMapper();
            return response?.MaterialRequests?.Select(mapper.Map).ToList() ?? new List<MaterialRequest>();
        }

        public bool HasPersonEmailAddress(int personId)
        {
            var response = false;
            try
            {
                response = _panelQueryService.HasPersonEmailAddress(personId);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response;
        }

        public bool HasOverlappingMembership(OverlappingMembershipModel[] model)
        {
            var response = false;
            try
            {
                var request = model.Select(x => new OverlappingMembershipRequestItem
                {
                    PersonId = x.PersonId,
                    PanelId = x.PanelId,
                    PanelRoleId = x.PanelRoleId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    PanelMembershipId = x.PanelMembershipId
                })
                .ToArray();

                response = _panelQueryService.HasOverlappingMembership(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response;
        }

        public bool HasRolePlayerRatingLocation(int personId)
        {
            var response = false;
            try
            {
                response = _panelQueryService.HasRolePlayerRatingLocation(personId);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response;
        }

        public GenericResponse<IEnumerable<PanelMembershipInfoModel>> GetPanelMembershipInfo(IEnumerable<int> panelMembershipIds)
        {
            var response =
                _panelQueryService.GetPanelMembershipInfo(
                    new PanelMembershipInfoRequest() {PanelMembershipIds = panelMembershipIds.ToArray()});

            var result = response.Data.Select(_autoMapperHelper.Mapper.Map<PanelMembershipInfoModel>);
            return new GenericResponse<IEnumerable<PanelMembershipInfoModel>>(result);
        }
    }
}
