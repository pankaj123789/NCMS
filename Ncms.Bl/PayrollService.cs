using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;

namespace Ncms.Bl
{
    public class PayrollService : IPayrollService
    {
        private readonly IUserService _userService;
        private readonly IPayrollQueryService _payrollQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PayrollService(IUserService userService, IPayrollQueryService payrollQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _userService = userService;
            _payrollQueryService = payrollQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public IEnumerable<ProductSpecificationLookupModel> GetProductSpecificationsForMarkingClaimsLookupList()
        {
            var results = new List<ProductSpecificationLookupModel>();

			var response = _payrollQueryService.GetProductSpecificationsForProductType((int)ProductType.MarkingClaims);
            results.AddRange(response.ProductSpecificationDetails.Select(SharedCustomMapper.MapProductSpecificationDetails).ToList());

            return results;
        }

        private GetMarkingsForPayrollResponse GetMarkingsForPayroll(PayrollStatusName status, PreviousMarkingFilterForPayrollModel filter)
        {
            GetMarkingsForPayrollResponse response = null;
            response = _payrollQueryService.GetMarkingsForPayroll(new GetMarkingsForPayrollRequest { PayrollStatuses = new[] { status }, From = filter.from, To = filter.to});
            return response;
        }

        public IEnumerable<MarkingForPayrollModel> GetMarkingsForValidation()
        {
            return GetMarkingsForPayroll(PayrollStatusName.Received, new PreviousMarkingFilterForPayrollModel()).Markings
                .Select(_autoMapperHelper.Mapper.Map<MarkingForPayrollModel>);
        }

        public IEnumerable<MarkingForPayrollModel> GetMarkingsForNewPayroll()
        {
            return GetMarkingsForPayroll(PayrollStatusName.Ready, new PreviousMarkingFilterForPayrollModel()).Markings
                .Select(_autoMapperHelper.Mapper.Map<MarkingForPayrollModel>);
        }

        public IEnumerable<MarkingForPayrollModel> GetMarkingsForInProgressPayroll()
        {
            return GetMarkingsForPayroll(PayrollStatusName.InProgress, new PreviousMarkingFilterForPayrollModel()).Markings
                .Select(_autoMapperHelper.Mapper.Map<MarkingForPayrollModel>);
        }

        public IEnumerable<MarkingForPayrollModel> GetMarkingsForPreviousPayroll(PreviousMarkingFilterForPayrollModel filter)
        {

            var filterData = filter;
            if ((filterData != null) && filterData.to.HasValue)
            {
                filterData.to = filterData.to.Value.AddDays(1);
            }
            return GetMarkingsForPayroll(PayrollStatusName.Complete, filterData).Markings
                .Select(_autoMapperHelper.Mapper.Map<MarkingForPayrollModel>);
        }

        public void ValidateMarkingForPayroll(IEnumerable<int> jobExaminerIds)
        {
            var request = new ValidateMarkingForPayrollRequest
            {
                JobExaminerIds = jobExaminerIds,
                UserId = _userService.Get().Id
            };

            _payrollQueryService.ValidateMarkingForPayroll(request);
        }

        public void UnvalidateMarkingForPayroll(int jobExaminerId)
        {
            _payrollQueryService.UnvalidateMarkingForPayroll(jobExaminerId);
        }

        public void CreatePayroll(IEnumerable<int> jobExaminerIds)
        {
            if (IsPayrollInProgress())
            {
                throw new UserFriendlySamException(Naati.Resources.Payroll.PayRunAlreadyProgressError);
            }

            CreatePayrollResponse response = null;
            response = _payrollQueryService.CreatePayroll(new CreatePayrollRequest
            {
                JobExaminerIds = jobExaminerIds,
                UserId = _userService.Get().Id
            });
            if (response.Error)
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }
        }

        public bool IsPayrollInProgress()
        {
            bool result = false;
            result = _payrollQueryService.IsPayrollInProgress();
            return result;
        }

        public GenericResponse<bool> AssignInvoiceNumber(JobExaminerPayrollModel model)
        {
            AssignPayrollAccountingReferenceResponse response = null;
            response = _payrollQueryService.AssignPayrollAccountingReference(new AssignPayrollAccountingReferenceRequest
            {
                PayrollId = model.PayrollId,
                AccountingReference = model.AccountingReference,
                JobExaminerIds = model.JobExaminerIds,
                UserId = _userService.Get().Id
            });

            if (response.Error)
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            var bsr = new GenericResponse<bool>();
            if (response.PayrollComplete)
            {
                bsr.Data = true;
                bsr.Messages.Add("This pay run is now complete.");
            }
            return bsr;
        }

        public GenericResponse<bool> RemoveExaminerFromPayroll(JobExaminerPayrollModel model)
        {
            RemoveExaminerFromPayrollResponse response = null;
            response = _payrollQueryService.RemoveExaminerFromPayroll(new RemoveExaminerFromPayrollRequest
            {
                PayrollId = model.PayrollId,
                JobExaminerIds = model.JobExaminerIds
            });

            if (response.Error)
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }

            return new GenericResponse<bool>(true);
        }

        public void FinalisePayroll(int payrollId)
        {
            _payrollQueryService.SetPayrollStatus(new SetPayrollStatusRequest
            {
                PayrollId = payrollId,
                Status = PayrollStatusName.Complete,
                UserId = _userService.Get().Id
            });
        }
        
    }
}