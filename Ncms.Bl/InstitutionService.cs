using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Institution;

namespace Ncms.Bl
{
    public class InstitutionService : IInstitutionService
    {
        private readonly IInstitutionQueryService _institutionQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public InstitutionService(IInstitutionQueryService institutionQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _institutionQueryService = institutionQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<InstitutionModel> GetInstitution(int naatiNumber)
        {
            var entity = _institutionQueryService.GetInstitution(naatiNumber);
            var institution = new InstitutionModel();
            _autoMapperHelper.Mapper.Map(entity, institution);


            return institution;
        }

        public GenericResponse<bool> UpdateInstitution(InstitutionModel model)
        {
            var institution = new InstitutionDto();
            _autoMapperHelper.Mapper.Map(model, institution);
            _institutionQueryService.UpdateInstitution(institution);

            return true;
        }

        public GenericResponse<bool> AddName(InstitutionModel model)
        {
            var institution = new InstitutionDto();
            _autoMapperHelper.Mapper.Map(model, institution);
            _institutionQueryService.AddName(institution);

            return true;
        }

        public GenericResponse<InstitutionInsertResponse> InsertInstitution(InstitutionModel model)
        {
            const int minNaatiNum = 900000;
            const int maxNaatiNum = 949999;

            if (model.NaatiNumber != 0 && model.NaatiNumber < minNaatiNum || model.NaatiNumber > maxNaatiNum)
            {
                return new GenericResponse<InstitutionInsertResponse>
                {
                    Success = false,
                    Errors = new List<string> { Naati.Resources.Institution.NaatiNumberValidationMessage }
                };
            }

            return _institutionQueryService.InsertInstitution(_autoMapperHelper.Mapper.Map<InstitutionDto>(model));
        }

        public GenericResponse<InstitutionInsertResponse> CheckDuplicatedInstitution(InstitutionModel model)
        {
            return _institutionQueryService.CheckDuplicatedInstitution(_autoMapperHelper.Mapper.Map<InstitutionDto>(model));
        }

        public GenericResponse<IEnumerable<InstitutionResultModel>> Search(InstitutionSearchRequest request)
        {
            {

                var getRequest = new GetInstituteSearchRequest()
                {
                    Skip = request.Skip,
                    Take = request.Take,
                    Filters = request.Filter.ToFilterList<InstituteSearchCriteria, InstituteFilterType>()
                };

                InstituteSearchResponse serviceReponse = null;
                serviceReponse = _institutionQueryService.SearchInstitute(getRequest);

                var institutionResult = serviceReponse.Results.Select(x => new InstitutionResultModel
                {
                    Id = x.InstituteId,
                    Name = x.Name,
                    NaatiNumber = x.NaatiNumber,
                    NoOfContacts = x.NoOfContacts,
                    PrimaryContactNo = x.PrimaryContactNo,
                    PrimaryEmail = x.PrimaryEmail


                }).ToList();

                var response = new GenericResponse<IEnumerable<InstitutionResultModel>>(institutionResult);

                if (request.Take.HasValue && institutionResult.Count == request.Take.Value)
                {
                    response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
                }

                if (request.Skip.HasValue)
                {
                    response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
                }

                return response;
            }
        }

        public GenericResponse<IEnumerable<EndorsedQualificationSearchResultModel>> SearchQualifications(EndorsedQualificationSearchRequest request)
        {
            var getRequest = new GetEndorsedQualificationSearchRequest()
            {
                Skip = request.Skip,
                Take = request.Take,
                Filters = request.Filter.ToFilterList<EndorsedQualificationSearchCriteria, EndorsedQualificationFilterType>()
            };

            var serviceReponse = _institutionQueryService.SearchEndorsedQualification(getRequest);

            var result = serviceReponse.Data.Select(_autoMapperHelper.Mapper.Map<EndorsedQualificationSearchResultModel>).ToList();

            var response = new GenericResponse<IEnumerable<EndorsedQualificationSearchResultModel>>(result);

            if (request.Take.HasValue && result.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public CreateOrUpdateResponse CreateOrUpdateQualification(EndorsedQualificationRequest model)
        {
            if (model.CredentialTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CredentialTypeId));
            }
            if (model.InstitutionId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.InstitutionId));
            }
            if (string.IsNullOrWhiteSpace(model.Location))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Location));
            }

            if (string.IsNullOrWhiteSpace(model.Qualification))
            {
                throw new ArgumentOutOfRangeException(nameof(model.Qualification));
            }

            if (model.EndorsementPeriodFrom < MinDate.Value)
            {
                throw new UserFriendlySamException(Naati.Resources.Institution.InvalidEndorsementStartDate);
            }

            if (model.EndorsementPeriodFrom > model.EndorsementPeriodTo)
            {
                throw new UserFriendlySamException(Naati.Resources.Institution.InvalidEndorsementEndDate);
            }

            var dto = _autoMapperHelper.Mapper.Map<EndorsedQualificationDto>(model);
            return _institutionQueryService.CreateOrUpdateQualification(dto);
        }

        public GenericResponse<EndorsedQualificationModel> GetEndorsedQualification(int endorsedQualificationId)
        {
            var response = _institutionQueryService.GetEndorsedQualification(endorsedQualificationId);

            return _autoMapperHelper.Mapper.Map<EndorsedQualificationModel>(response.Data);
        }
    }
}
