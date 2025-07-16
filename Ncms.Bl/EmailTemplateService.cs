using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using IEmailTemplateService = Ncms.Contracts.IEmailTemplateService;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl
{
    public class EmailTemplateService : Contracts.IEmailTemplateService
    {
        private readonly IEmailTemplateQueryService _emailTemplateQueryService;
        private readonly IUserService _userService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public EmailTemplateService(IEmailTemplateQueryService emailTemplateQueryService, IUserService userService, IAutoMapperHelper autoMapperHelper)
        {
            _emailTemplateQueryService = emailTemplateQueryService;
			_userService = userService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<IList<ServiceEmailTemplateModel>> GetAllEmailTemplates(SearchRequest request)
        {
            var filters = request.Filter.ToFilterList<EmailTemplateSearchCriteria, EmailTemplateFilterType>();
            var getRequest = _autoMapperHelper.Mapper.Map<EmailTemplateSearchRequest>(request);
            getRequest.Filter = filters;

            var emailResponse =
				_emailTemplateQueryService.SearchEmailTemplates(getRequest)
					.Results
					.ToList()
					.Select(_autoMapperHelper.Mapper.Map<ServiceEmailTemplateModel>);

			return new GenericResponse<IList<ServiceEmailTemplateModel>> {Data = emailResponse.ToList() };
        }

        public GenericResponse<ServiceEmailTemplateModel> Get(int id)
        {
            if (id <= 0)
            {

                throw new ArgumentOutOfRangeException(nameof(id));
            }
            
            var emailResponse = _emailTemplateQueryService.Get(new EmailTemplateRequest {Id = id});

            var emailModel = _autoMapperHelper.Mapper.Map<ServiceEmailTemplateModel>(emailResponse);

            return emailModel;
        }

        public void Save(ServiceEmailTemplateModel model)
        {
            if (model.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.Id));

            var requestModel = _autoMapperHelper.Mapper.Map<EmailTemplateRequest>(model);
			requestModel.UserId = _userService.Get().Id;
			 _emailTemplateQueryService.Save(requestModel);
        }
    }
}
