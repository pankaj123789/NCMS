using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.ProfessionalDevelopment
{
    
    public class MyNaatiCredentialPointsCalculatorHelper : BasePointsCalculatorHelper, ICredentialPointsCalculatorService
    {
        private readonly IActivityPointsCalculatorService mActivityPointsCalculator;
        public MyNaatiCredentialPointsCalculatorHelper(ILogbookQueryService logbookQueryService, ISystemQueryService systemQueryService,
            IPersonQueryService personQueryService, IApplicationBusinessLogicService businessLogicService, IActivityPointsCalculatorService activityPointsCalculator)
            : base(systemQueryService, personQueryService, businessLogicService, logbookQueryService)
        {
            mActivityPointsCalculator = activityPointsCalculator;
        }

        public IEnumerable<WorkPracticeResponse> GetWorkPractices(int credentialId, int naatiNumber, int certificationPeriodId)
        {
            var certificationPeriod = GetCertificationPeriodDetails(naatiNumber, credentialId).First(x => x.Id == certificationPeriodId);

            var includedPractices = certificationPeriod.Id > 0 ? GetWorkPracticeCredential(credentialId, certificationPeriod).IncludedWorkPracticeIds : Enumerable.Empty<int>();
            var result = GetWorkPractices(credentialId, certificationPeriod).Select(x => MapWorkPracticeResponse(x, includedPractices.Contains(x.Id)));
            return result;
        }

        public virtual IEnumerable<CredentialCertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber, int credentialId)
        {
            var periods = GetAllCertificationPeriodDetails(naatiNumber);
            var credentialPeriods = periods.Select(MapCertificationPeriod).Where(x => x.Credentials.Any(c => c.Id == credentialId) || x.SubmittedCredentials.Any(c => c.Id == credentialId)).ToList();
            var defaultPeriod = GetDefaultCertificationPeriod(credentialPeriods, credentialId);

            if (defaultPeriod.Id == 0)
            {
                credentialPeriods.Insert(0, defaultPeriod);
            }
            defaultPeriod.Editable = IsEditablePeriod(defaultPeriod, credentialId);
            foreach (var certificationPeriod in credentialPeriods)
            {
                certificationPeriod.IsCredentialSubmitted = IsSubmittedCredential(certificationPeriod, credentialId);
            }
            var mappedCredentials = credentialPeriods.OrderByDescending(x => x.IsDefault).ToList();
            return mappedCredentials;
        }



        protected virtual bool IsEditablePeriod(CredentialCertificationPeriodDetailsDto period, int credentialId)
        {
            var isReadOnly = (period.RecertificationStatus == CertificationPeriodRecertificationStatus.BeingAssessed ||
                              period.RecertificationStatus == CertificationPeriodRecertificationStatus.Completed)
                             && IsSubmittedCredential(period, credentialId);
            return !isReadOnly;
        }

        protected virtual CredentialCertificationPeriodDetailsDto GetDefaultCertificationPeriod(IList<CredentialCertificationPeriodDetailsDto> periods, int credentialId)
        {
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

            var defaultPeriod = earliestCurrentNotSubmitted.FirstOrDefault() ??
                                earliestFutureNotSubmitted.FirstOrDefault() ??
                                latestExpiredNotSubmitted.FirstOrDefault() ??
                                latestSubmittedButNotAttachedCredential.FirstOrDefault() ??
                                MapCertificationPeriod(new CertificationPeriodDetailsDto
                                {
                                    Id = 0,
                                    StartDate = DateTime.Now.AddYears(-10),
                                    EndDate = DateTime.Now.AddYears(10),
                                    RecertificationStatus = CertificationPeriodRecertificationStatus.None,
                                    CertificationPeriodStatus = CertificationPeriodStatus.Current

                                });
            defaultPeriod.IsDefault = true;
            defaultPeriod.IsCurrent = true;
            return defaultPeriod;


        }


        protected CredentialCertificationPeriodDetailsDto MapCertificationPeriod(CertificationPeriodDetailsDto certificationPeriod)
        {
            var dto = new CredentialCertificationPeriodDetailsDto();
            var submittedRecertification =
                LogBookQueryService.GetSubmittedRecertificationApplicationForPeriod(certificationPeriod.Id);
            MapCertificationPeriodDetailsDto(dto, certificationPeriod, certificationPeriod.RecertificationEligibilityDate, submittedRecertification);
            dto.Credentials = certificationPeriod.Id > 0 ? LogBookQueryService.GetCertificationPeriodCredentials(certificationPeriod.Id) : Enumerable.Empty<CredentialsDetailsDto>();
            dto.SubmittedCredentials = Enumerable.Empty<CredentialsDetailsDto>();
            if (IsSubmittedPeriod(dto))
            {
                dto.SubmittedCredentials = LogBookQueryService.GetSubmittedRecertificationApplicationCredentials(certificationPeriod.SubmittedRecertificationApplicationId.GetValueOrDefault());
            }

            return dto;
        }



        protected bool IsSubmittedCredential(CredentialCertificationPeriodDetailsDto period, int credentialId)
        {
            return IsSubmittedPeriod(period) && period.SubmittedCredentials.Any(x => x.Id == credentialId);
        }

        public WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId, int credentialId)
        {
            if (certificationPeriodId == 0)
            {
                var credential = LogBookQueryService.GetCredentials(naatiNumber).List.First(x => x.Id == credentialId);
                var mappedCredential = MapCredential(credential, new WorkPracticeDetails[0], DateTime.Now, 0, false);
                return mappedCredential;
            }

            var periods = GetCertificationPeriodDetails(naatiNumber, credentialId).ToList();
            var certificationPeriod = periods.First(x => x.Id == certificationPeriodId);

            return GetWorkPracticeCredential(credentialId, certificationPeriod);
        }

        private WorkPracticeCredentialDto GetWorkPracticeCredential(int credentialId, CredentialCertificationPeriodDetailsDto certificationPeriod)
        {
            var credential = certificationPeriod.SubmittedCredentials.FirstOrDefault(x => x.Id == credentialId) ??
                             certificationPeriod.Credentials.First(x => x.Id == credentialId);

            DateTime startDate;

            var requirementStartDate = credential.StartDate > certificationPeriod.StartDate
                ? credential.StartDate
                : certificationPeriod.StartDate;
            var totalDays = (certificationPeriod.OriginalEndDate - requirementStartDate).TotalDays;

            var policyEndDate = requirementStartDate.GetLeapYearAdjustedEndDate(policyYears);
            if(DateTime.Compare(policyEndDate,certificationPeriod.OriginalEndDate) > 0)
            {
                policyEndDate = certificationPeriod.OriginalEndDate;
            }

            int years = 0;
            for (DateTime currentDate = requirementStartDate; currentDate < policyEndDate; currentDate = currentDate.AddYears(1))
            {
                years++;
            }           
            var requirement = credential.WorkPracticePoints.GetValueOrDefault() * years;
            //2020 was excluded from requirements because of Covid
            if (IsYearInPeriod(certificationPeriod.StartDate, certificationPeriod.OriginalEndDate, 2020))
            {
                requirement = requirement * 2 / 3;
            }

            IList<WorkPracticeDetails> workPractices = GetWorkPractices(credential.Id, certificationPeriod).ToList();
            if (IsSubmittedCredential(certificationPeriod, credential.Id))
            {
                startDate = certificationPeriod.StartDate;
            }
            else
            {
                switch (credential.CredentialApplicationTypeCategoryId)
                {
                    case (int)CredentialApplicationTypeCategoryName.Transition:
                        var pdTransitionStartDate = DateTime.Parse(SystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "PDTransitionStartDate" })
                            .Value);
                        startDate = pdTransitionStartDate;
                        break;
                    case (int)CredentialApplicationTypeCategoryName.Recertification:
                        startDate = credential.CredentialApplicationEnteredDate.Date;
                        break;
                    default:
                        startDate = credential.StartDate;
                        break;
                }

                workPractices = workPractices.Where(x => x.Date >= startDate && x.Date <= credential.EndDate).ToList();
            }

            var mappedCredential = MapCredential(credential, workPractices, startDate, requirement, true);

            return mappedCredential;
        }


        private WorkPracticeCredentialDto MapCredential(CredentialsDetailsDto credential, IList<WorkPracticeDetails> workPractices, DateTime recertificationActivitiesDate, double requirement, bool calculated)
        {
            return new WorkPracticeCredentialDto
            {
                CredentialApplicationTypeCategoryId = credential.CredentialApplicationTypeCategoryId,
                CredentialApplicationEnteredDate = credential.CredentialApplicationEnteredDate,
                CredentialRequestId = credential.CredentialRequestId,
                CredentialType = credential.CredentialType,
                Direction = credential.Direction,
                EndDate = credential.EndDate,
                StartDate = credential.StartDate,
                RecertificationActivitesStartDate = recertificationActivitiesDate,
                Id = credential.Id,
                IncludeOD = credential.IncludeOD,
                Points = workPractices.Sum(y => y.Points),
                IsIndigenous = credential.IsIndigenous,
                Language = credential.Language,
                Requirement = requirement,
                WorkPracticePoints = credential.WorkPracticePoints,
                Skill = credential.Skill,
                TerminationDate = credential.TerminationDate,
                ToLanguage = credential.ToLanguage,
                WorkPracticeUnits = credential.WorkPracticeUnits,
                SkillId = credential.SkillId,
                CredentialTypeId = credential.CredentialTypeId,
                CategoryId = credential.CategoryId,
                CredentialStatus = credential.CredentialStatus,
                Calculated = calculated,
                IncludedWorkPracticeIds = workPractices.Select(x => x.Id),

            };
        }

        private IEnumerable<WorkPracticeDetails> GetWorkPractices(int credentialId, CredentialCertificationPeriodDetailsDto certificationPeriod)
        {
            if (IsSubmittedCredential(certificationPeriod, credentialId))
            {
                return LogBookQueryService.GetWorkPracticesForRecertificationApplication(credentialId, certificationPeriod.SubmittedRecertificationApplicationId.GetValueOrDefault());
            }
            var result = LogBookQueryService.GetWorkPractices(credentialId);
            return result;
        }



        private WorkPracticeResponse MapWorkPracticeResponse(WorkPracticeDetails workPractice, bool current)
        {

            return new WorkPracticeResponse
            {
                Id = workPractice.Id,
                Points = workPractice.Points,
                Date = workPractice.Date,
                Description = workPractice.Description,
                HasAttachments = workPractice.HasAttachments,
                Status = current ? (int)WorkPracticeStatus.Current : 0
            };

        }

        public IEnumerable<WorkPracticeCredentialDto> GetDefaultPeriodCredentials(int naatiNumber)
        {
            var certificationPeriod = mActivityPointsCalculator.GetCertificationPeriodDetails(naatiNumber).First();

            if (certificationPeriod.Id == 0 || IsSubmittedPeriod(certificationPeriod))
            {
                return Enumerable.Empty<WorkPracticeCredentialDto>();
            }

            var credentialCertificationPeriod = MapCertificationPeriod(certificationPeriod);

            credentialCertificationPeriod.IsDefault = true;
            credentialCertificationPeriod.IsCurrent = true;
            var workPracticeCredentials =
                credentialCertificationPeriod.Credentials.Select(
                    x => GetWorkPracticeCredential(x.Id, credentialCertificationPeriod));

            return workPracticeCredentials.Where(x => IsEligibleForRecertification(x.Id));

        }

        protected virtual bool IsEligibleForRecertification(int credentialId)
        {
            return BusinessLogicService.CalculateCredentialRecertificationStatus(credentialId) == RecertificationStatus.EligibleForNew;
        }

        private bool IsYearInPeriod(DateTime startDate, DateTime endDate, int year)
        {
            var yearStartDate = new DateTime(year, 1,1);
            var yearEndDate = new DateTime(year, 12,31);
            return IsBetweenDate(yearStartDate, yearEndDate, startDate) ||
                   IsBetweenDate(yearStartDate, yearEndDate, endDate) ||
                   (startDate < yearStartDate && endDate > yearEndDate);
        }

        private bool IsBetweenDate(DateTime startDate, DateTime endDate, DateTime date)
        {
            return date >= startDate && date <= endDate;
        }
    }
}
