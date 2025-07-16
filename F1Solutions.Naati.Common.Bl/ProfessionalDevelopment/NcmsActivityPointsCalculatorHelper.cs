using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.ProfessionalDevelopment
{
    public class NcmsActivityPointsCalculatorHelper : MyNaatiActivityPointsCalculatorHelper
    {
        public NcmsActivityPointsCalculatorHelper(ILogbookQueryService logbookQueryService, IPersonQueryService personQueryService, IApplicationQueryService applicationQueryService, ISystemQueryService systemQueryService, IApplicationBusinessLogicService businessLogicService) : base(logbookQueryService, personQueryService, applicationQueryService, systemQueryService, businessLogicService)
        {
        }

        public override List<CertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber)
        {
            var periods = GetAllCertificationPeriodDetails(naatiNumber);
            var defaultPeriod = GetDefaultCertificationPeriod(periods);
            var currentPeriod = base.GetDefaultCertificationPeriod(periods);

            var currentAssigned = false;
            foreach (var certificationPeriodDetailsDto in periods)
            {
                certificationPeriodDetailsDto.IsDefault = certificationPeriodDetailsDto.Id == defaultPeriod.Id;
                certificationPeriodDetailsDto.Editable = IsEditablePeriod(certificationPeriodDetailsDto);
                certificationPeriodDetailsDto.IsCurrent = certificationPeriodDetailsDto.Id == currentPeriod.Id;
                currentAssigned = currentAssigned || certificationPeriodDetailsDto.IsCurrent;
            }

            if (!currentAssigned)
            {
                currentPeriod = new CertificationPeriodDetailsDto
                {
                    Id = 0,
                    StartDate = DateTime.Now.AddYears(-10),
                    EndDate = DateTime.Now.AddYears(10),
                    RecertificationStatus = CertificationPeriodRecertificationStatus.None,
                    CertificationPeriodStatus = CertificationPeriodStatus.Current,
                    IsCurrent = true
                };
                currentPeriod.Editable = IsEditablePeriod(currentPeriod);
                periods.Insert(0,currentPeriod);
            }
            return periods.OrderByDescending(x => x.IsDefault).ToList();
        }

        protected override CertificationPeriodDetailsDto GetDefaultCertificationPeriod(IList<CertificationPeriodDetailsDto> periods)
        {
            if (!periods.Any())
            {
                throw new NullReferenceException("User doesnt have any certification Period");
            }
            var beingAssessed = periods
                .Where(p => p.RecertificationStatus == CertificationPeriodRecertificationStatus.BeingAssessed)
                .OrderBy(y => y.StartDate);
            var earliestCurrentNotSubmitted = periods
                .Where(p => p.CertificationPeriodStatus == CertificationPeriodStatus.Current &&
                            p.RecertificationStatus == CertificationPeriodRecertificationStatus.None)
                .OrderBy(y => y.StartDate);
            var earliestFutureNotSubmitted = periods
                .Where(p => p.CertificationPeriodStatus == CertificationPeriodStatus.Future &&
                            p.RecertificationStatus == CertificationPeriodRecertificationStatus.None)
                .OrderBy(y => y.StartDate);

            var latestExpiredNotSubmitted = periods
                .Where(p => p.CertificationPeriodStatus == CertificationPeriodStatus.Expired &&
                            p.RecertificationStatus == CertificationPeriodRecertificationStatus.None)
                .OrderByDescending(y => y.EndDate);

            var defaultPeriod = beingAssessed.FirstOrDefault() ??
                                earliestCurrentNotSubmitted.FirstOrDefault() ??
                                earliestFutureNotSubmitted.FirstOrDefault() ??
                                latestExpiredNotSubmitted.FirstOrDefault() ??
                                periods.First();
           
            return defaultPeriod;

        }

        protected override bool IsEditablePeriod(CertificationPeriodDetailsDto period)
        {

            var isReadOnly = period.RecertificationStatus == CertificationPeriodRecertificationStatus.Completed;
            return !isReadOnly;

        }
    }
}
