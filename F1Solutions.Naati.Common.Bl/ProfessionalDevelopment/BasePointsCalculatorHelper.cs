using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.ProfessionalDevelopment
{
    public abstract class BasePointsCalculatorHelper
    {
        protected ISystemQueryService SystemQueryService { get; }
        protected IPersonQueryService PersonQueryService { get; }
        protected IApplicationBusinessLogicService BusinessLogicService { get; }
        protected ILogbookQueryService LogBookQueryService { get;  }
        protected int policyYears { get { return Int32.Parse(SystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "CertificationPeriodDefaultDuration" }).Value); } }

        protected BasePointsCalculatorHelper(ISystemQueryService systemQueryService, IPersonQueryService personQueryService, IApplicationBusinessLogicService businessLogicService, ILogbookQueryService logbookQueryService)
        {
           SystemQueryService = systemQueryService;
           PersonQueryService = personQueryService;
           BusinessLogicService = businessLogicService;
           LogBookQueryService = logbookQueryService;
        
        }

        protected List<CertificationPeriodDetailsDto> GetAllCertificationPeriodDetails(int naatiNumber)
        {
            var recertifyPeriod = Convert.ToInt32(SystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "CertificationPeriodRecertifyMonths" }).Value);

            var certificationPeriods = PersonQueryService.GetCertificationPeriods(new GetCertificationPeriodsRequest
            {
                CertificationPeriodStatus = new[] { CertificationPeriodStatus.Current, CertificationPeriodStatus.Expired, CertificationPeriodStatus.Future },
                NaatiNumber = naatiNumber
            })
                .Results;

            var periods = certificationPeriods.Select(x =>
            {
                var submittedRecertification =
                    LogBookQueryService.GetSubmittedRecertificationApplicationForPeriod(x.Id);
                var dto = new CertificationPeriodDetailsDto();
                MapCertificationPeriodDetailsDto(dto, x, x.OriginalEndDate.AddMonths(-recertifyPeriod), submittedRecertification);
                return dto;
            }).OrderBy(x => x.Id).ToList();

            return periods;
        }

        protected virtual void MapCertificationPeriodDetailsDto(CertificationPeriodDetailsDto certificationPeriod,
            CertificationPeriodDto dto,
            DateTime eligibilityDate,
            RecertificationDto recertificationApplication)
        {
            certificationPeriod.EndDate = dto.EndDate;
            certificationPeriod.Id = dto.Id;
            certificationPeriod.OriginalEndDate = dto.OriginalEndDate;
            certificationPeriod.StartDate = dto.StartDate;
            certificationPeriod.RecertificationStatus = dto.Id > 0 ? BusinessLogicService.CalculateCertificationPeriodRecertificationStatus(dto.Id) : CertificationPeriodRecertificationStatus.None;
            certificationPeriod.CertificationPeriodStatus = dto.CertificationPeriodStatus;
            certificationPeriod.RecertificationEligibilityDate = eligibilityDate;
            certificationPeriod.SubmittedRecertificationApplicationId = recertificationApplication?.ApplicationId;
            certificationPeriod.RecertificationEnteredDate = recertificationApplication?.SubmittedDate;
            certificationPeriod.CredentialApplicationId = dto.CredentialApplicationId;

        }

        protected virtual bool IsSubmittedPeriod(CertificationPeriodDetailsDto period)
        {
            return period.SubmittedRecertificationApplicationId.HasValue;
        }
    }
}
