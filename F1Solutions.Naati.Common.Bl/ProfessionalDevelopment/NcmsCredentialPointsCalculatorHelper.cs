using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.ProfessionalDevelopment
{
    public class NcmsCredentialPointsCalculatorHelper : MyNaatiCredentialPointsCalculatorHelper
    {
        public NcmsCredentialPointsCalculatorHelper(ILogbookQueryService logbookQueryService, ISystemQueryService systemQueryService, IActivityPointsCalculatorService activityPointsCalculator, IPersonQueryService personQueryService, IApplicationBusinessLogicService businessLogicService) : base(logbookQueryService, systemQueryService, personQueryService, businessLogicService, activityPointsCalculator)
        {

        }

        public override IEnumerable<CredentialCertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber, int credentialId)
        {
            var periods = GetAllCertificationPeriodDetails(naatiNumber);
            var credentialPeriods = periods.Select(MapCertificationPeriod).Where(x => x.Credentials.Any(c => c.Id == credentialId) || x.SubmittedCredentials.Any(c => c.Id == credentialId)).ToList();
            var defaultPeriod = GetDefaultCertificationPeriod(credentialPeriods, credentialId);
            var currentPeriod = base.GetDefaultCertificationPeriod(credentialPeriods, credentialId);

            var currentAssigned = false;
            foreach (var certificationPeriodDetailsDto in credentialPeriods)
            {
                certificationPeriodDetailsDto.IsCredentialSubmitted = IsSubmittedCredential(certificationPeriodDetailsDto, credentialId);
                certificationPeriodDetailsDto.IsDefault = certificationPeriodDetailsDto.Id == defaultPeriod.Id;
                certificationPeriodDetailsDto.Editable = IsEditablePeriod(certificationPeriodDetailsDto, credentialId);
                certificationPeriodDetailsDto.IsCurrent = certificationPeriodDetailsDto.Id == currentPeriod.Id;
                currentAssigned = currentAssigned || certificationPeriodDetailsDto.IsCurrent;
            }

            if (!currentAssigned)
            {
                currentPeriod = MapCertificationPeriod(new CertificationPeriodDetailsDto
                {
                    Id = 0,
                    StartDate = DateTime.Now.AddYears(-10),
                    EndDate = DateTime.Now.AddYears(10),
                    RecertificationStatus = CertificationPeriodRecertificationStatus.None,
                    CertificationPeriodStatus = CertificationPeriodStatus.Current,
                });
                currentPeriod.IsCurrent = true;
                currentPeriod.Editable = IsEditablePeriod(currentPeriod, credentialId);
                credentialPeriods.Insert(0, currentPeriod);
            }

            var mappedCredentials = credentialPeriods.OrderByDescending(x => x.IsDefault).ToList();
            return mappedCredentials;
        }

        protected override CredentialCertificationPeriodDetailsDto GetDefaultCertificationPeriod(IList<CredentialCertificationPeriodDetailsDto> periods, int credentialId)
        {
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

            var latestSubmittedButNotAttachedCredential = periods
                .Where(p => IsSubmittedPeriod(p) && !IsSubmittedCredential(p, credentialId))
                .OrderByDescending(y => y.EndDate);

            var defaultPeriod = beingAssessed.FirstOrDefault() ??
                earliestCurrentNotSubmitted.FirstOrDefault() ??
                                earliestFutureNotSubmitted.FirstOrDefault() ??
                                latestExpiredNotSubmitted.FirstOrDefault() ??
                                latestSubmittedButNotAttachedCredential.FirstOrDefault() ??
                                periods.First();

            return defaultPeriod;

        }

        protected override bool IsEditablePeriod(CredentialCertificationPeriodDetailsDto period, int credentialId)
        {
            var isReadOnly = period.RecertificationStatus == CertificationPeriodRecertificationStatus.Completed
                             && IsSubmittedCredential(period, credentialId);
            return !isReadOnly;

        }
    }
}
