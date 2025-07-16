using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.ProfessionalDevelopment
{

    public class MyNaatiActivityPointsCalculatorHelper : BasePointsCalculatorHelper, IActivityPointsCalculatorService
    {
        private readonly ILogbookQueryService mLogbookQueryService;
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly ISystemQueryService mSystemQueryService;

        public MyNaatiActivityPointsCalculatorHelper(ILogbookQueryService logbookQueryService, IPersonQueryService personQueryService, IApplicationQueryService applicationQueryService, ISystemQueryService systemQueryService,
            IApplicationBusinessLogicService businessLogicService) : base(systemQueryService, personQueryService, businessLogicService, logbookQueryService)
        {
            mLogbookQueryService = logbookQueryService;
            mApplicationQueryService = applicationQueryService;
            mSystemQueryService = systemQueryService;
        }

        public virtual List<CertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber)
        {
            var periods = GetAllCertificationPeriodDetails(naatiNumber);
            var defaultPeriod = GetDefaultCertificationPeriod(periods);

            if (defaultPeriod.Id == 0)
            {
                periods.Insert(0, defaultPeriod);
            }

            defaultPeriod.Editable = IsEditablePeriod(defaultPeriod);
            return periods.OrderByDescending(x => x.IsDefault).ToList();

        }

        protected virtual bool IsEditablePeriod(CertificationPeriodDetailsDto period)
        {
            var isReadOnly = period.RecertificationStatus == CertificationPeriodRecertificationStatus.BeingAssessed || period.RecertificationStatus == CertificationPeriodRecertificationStatus.Completed;
            return !isReadOnly;
        }

        protected virtual CertificationPeriodDetailsDto GetDefaultCertificationPeriod(IList<CertificationPeriodDetailsDto> periods)
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

            var defaultPeriod = earliestCurrentNotSubmitted.FirstOrDefault() ??
                                earliestFutureNotSubmitted.FirstOrDefault() ??
                                latestExpiredNotSubmitted.FirstOrDefault() ??
                                new CertificationPeriodDetailsDto
                                {
                                    Id = 0,
                                    StartDate = DateTime.Now.AddYears(-10),
                                    EndDate = DateTime.Now.AddYears(10),
                                    RecertificationStatus = CertificationPeriodRecertificationStatus.None,
                                    CertificationPeriodStatus = CertificationPeriodStatus.Current

                                };
            defaultPeriod.IsDefault = true;
            defaultPeriod.IsCurrent = true;
            return defaultPeriod;


        }

        public IEnumerable<ProfessionalDevelopmentActivityDto> GetAllCertificationPeriodActivities(int naatiNumber, int certificationPeriodId)
        {
            var certificationPeriod = GetCertificationPeriodDetails(naatiNumber)
                .First(x => x.Id == certificationPeriodId);

            if (IsSubmittedPeriod(certificationPeriod))
            {
                return mLogbookQueryService.GetRecertificationAtivitiesForApplication(certificationPeriod.SubmittedRecertificationApplicationId.GetValueOrDefault());
            }

            var periodActivities = mLogbookQueryService.GetProfessionalDevelopmentActivities(
                new GetActivitiesRequest
                {
                    NaatiNumber = naatiNumber,
                }).List.ToList();

            return periodActivities;
        }

        private IEnumerable<ProfessionalDevelopmentActivityDto> GetCertificationPeriodActivities(int naatiNumber, CertificationPeriodDetailsDto certificationPeriod, int totalExistingPeriods)
        {

            if (IsSubmittedPeriod(certificationPeriod))
            {
                return mLogbookQueryService.GetRecertificationAtivitiesForApplication(certificationPeriod.SubmittedRecertificationApplicationId.GetValueOrDefault());
            }

            var minAcitivitiesStartDate = GetMinimumStartDateForActivities(certificationPeriod, naatiNumber, totalExistingPeriods);
            var periodActivities = mLogbookQueryService.GetProfessionalDevelopmentActivities(
                new GetActivitiesRequest
                {
                    NaatiNumber = naatiNumber,
                    StartDate = minAcitivitiesStartDate,
                    EndDate = certificationPeriod.EndDate
                }).List.ToList();

            return periodActivities;


        }

        public PdActivityPoints CaluculatePointsForDefaultPeriod(int naatiNumber)
        {
            var periods = GetAllCertificationPeriodDetails(naatiNumber);

            var defaultPeriod = GetDefaultCertificationPeriod(periods);

            var activities = GetCertificationPeriodActivities(naatiNumber, defaultPeriod, periods.Count(x => x.Id > 0));

            var activityPoints = GetActivityPoints(defaultPeriod, activities.ToList());

            return activityPoints;
        }

        public PdActivityPoints CaluculatePointsFor(int naatiNumber, int certificationPeriodId)
        {
            var periods = GetCertificationPeriodDetails(naatiNumber).ToList();
            if (!periods.Any())
            {
                throw new NullReferenceException("User doesnt have any certification Period");
            }
            var certificationPeriod = periods.First(x => x.Id == certificationPeriodId);
            var activities = GetCertificationPeriodActivities(naatiNumber, certificationPeriod, periods.Count(x => x.Id > 0));

            var activityPoints = GetActivityPoints(certificationPeriod, activities.ToList());
            return activityPoints;
        }


        private PdActivityPoints GetActivityPoints(CertificationPeriodDto certificationPeriod, IList<ProfessionalDevelopmentActivityDto> periodActivities)
        {
            var activitySections = new List<PdActivitySectionPoints>();
            var activityPoints = new PdActivityPoints
            {
                Points = 0,
                MinPoints = 0,
                Sections = activitySections,
                PreRequisites = Enumerable.Empty<PdPreRequisite>(),
                IncludedActivitiesIds = Enumerable.Empty<int>()
            };

            if (certificationPeriod == null || certificationPeriod.Id == 0)
            {
                return activityPoints;
            }

            activityPoints.IncludedActivitiesIds = periodActivities.Select(x => x.Id).ToList();
            activityPoints.Calculated = true;

            var pointsConfiguration = mLogbookQueryService.GetActivityPointsConfiguration();

            var policyEndDate = certificationPeriod.StartDate.GetLeapYearAdjustedEndDate(policyYears);
            if (DateTime.Compare(policyEndDate, certificationPeriod.OriginalEndDate) > 0)
            {
                policyEndDate = certificationPeriod.OriginalEndDate;
            }

            int years = 0;
            for (DateTime currentDate = certificationPeriod.StartDate; currentDate < policyEndDate; currentDate = currentDate.AddYears(1))
            {
                years++;
            }
            var periodYears = years;

            if (IsYearInPeriod(certificationPeriod.StartDate, certificationPeriod.OriginalEndDate, 2020))
            {
                periodYears--;
            }

            var minimumPointsForPeriod = (int)(periodYears * pointsConfiguration.RequiredPointsPerYear);
            activityPoints.MinPoints = minimumPointsForPeriod;
            foreach (var sectionConfiguration in pointsConfiguration.Sections)
            {
                activitySections.Add(GetSectionPoints(certificationPeriod.StartDate, periodYears, sectionConfiguration, periodActivities));
            }

            activityPoints.Points = activitySections.Sum(x => x.Points);

            if (activityPoints.MinPoints > 0)
            {
                activityPoints.PreRequisites = activityPoints.PreRequisites.Concat(new[]
                {
                    new PdPreRequisite
                    {
                        Completed =activityPoints.Points >= activityPoints.MinPoints,
                        Message =$"Minimum {activityPoints.MinPoints} points in total. Current Points: {activityPoints.Points}"
                    }
                });
            }

            activityPoints.Completed = activityPoints.PreRequisites.All(x => x.Completed) && activitySections.All(x => x.Completed);
            return activityPoints;
        }

        private int GetActivityPoints(ProfessionalDevelopmentActivityDto activity)
        {
            return activity.Points;
        }

        private PdCategoryPoints GetCategoryPoints(DateTime startPeriodDate, CategoryConfiguration categoryConfiguration, IList<ProfessionalDevelopmentActivityDto> activities)
        {
            var categoryPoints = new PdCategoryPoints
            {
                CategoryId = categoryConfiguration.CategoryId,
                CategoryName = categoryConfiguration.CategoryName,
                MinPoints = 0,
                PreRequisites = Enumerable.Empty<PdPreRequisite>(),
                Points = 0,
                Completed = true,
            };

            categoryPoints.Completed = categoryPoints.PreRequisites.All(x => x.Completed);

            switch (categoryConfiguration.PointsLismitTypeId.GetValueOrDefault())
            {
                case (int)PointsLimitTypeName.MaxPointsPerApplication:
                    categoryPoints.Points = activities.Sum(GetActivityPoints);
                    categoryPoints.Points = Math.Min(categoryPoints.Points, categoryConfiguration.PointsLimit.GetValueOrDefault());
                    break;
                case (int)PointsLimitTypeName.MaxPointsPerYear:
                    var activitiesByYear = GroupActiviesByYear(startPeriodDate, activities);
                    foreach (var groupedActivities in activitiesByYear)
                    {
                        var maxPoints = categoryConfiguration.PointsLimit.GetValueOrDefault();
                        categoryPoints.Points = categoryPoints.Points + Math.Min(groupedActivities.Sum(GetActivityPoints), maxPoints);
                    }
                    break;
                default:
                    categoryPoints.Points = activities.Sum(GetActivityPoints);
                    break;
            }

            return categoryPoints;
        }

        private IEnumerable<IGrouping<int, ProfessionalDevelopmentActivityDto>> GroupActiviesByYear(DateTime startPeriodDate, IList<ProfessionalDevelopmentActivityDto> activities)
        {
            var groups = activities.GroupBy(x => (int)(x.DateCompleted < startPeriodDate ? (x.DateCompleted - startPeriodDate).TotalDays / 365 - 1 : (x.DateCompleted - startPeriodDate).TotalDays / 365));
            return groups;
        }

        private PdActivitySectionPoints GetSectionPoints(DateTime startPeriodDate, int periodYears, SectionPointsConfiguration sectionConfiguration, IList<ProfessionalDevelopmentActivityDto> activities)
        {

            var categories = new List<PdCategoryPoints>();
            var section = new PdActivitySectionPoints
            {
                SectionId = sectionConfiguration.SectionId,
                MinPoints = (int)(sectionConfiguration.RequiredPointsPerYear * periodYears),
                SectionName = sectionConfiguration.SectionName,
                PreRequisites = Enumerable.Empty<PdPreRequisite>(),
                Categories = categories
            };

            foreach (var categoryConfiguration in sectionConfiguration.Categories)
            {
                var categoryActivities = activities
                    .Where(a => a.ProfessionalDevelopmentCategoryId == categoryConfiguration.CategoryId)
                    .ToList();

                var categoryPoints = GetCategoryPoints(startPeriodDate, categoryConfiguration, categoryActivities);
                section.Points += categoryPoints.Points;
                categories.Add(categoryPoints);
            }

            foreach (var categoryGroups in sectionConfiguration.Categories.GroupBy(c => c.CategoryGroupId).Where(cg => cg.Key.HasValue))
            {
                var groupActivities = activities.Where(x => x.ProfessionalDevelopmentCategoryGroupId == categoryGroups.Key);
                var minCategoryGroupPoints = (int)((categoryGroups.First().CategoryGroupRequiredPointsPerYear ?? 0) * periodYears);
                var groupActivityPoints = groupActivities.Sum(GetActivityPoints);
                if (minCategoryGroupPoints > 0)
                {
                    section.PreRequisites = section.PreRequisites.Concat(new[]
                    {
                        new PdPreRequisite
                        {
                            Completed =groupActivityPoints >=minCategoryGroupPoints,
                            Message =$"Minimum {minCategoryGroupPoints} points from {categoryGroups.First().CategoryGroupName}. Current Points: {groupActivityPoints}"
                        }
                    });
                }
            }

            if (section.MinPoints > 0)
            {
                section.PreRequisites = section.PreRequisites.Concat(new[]
                {
                    new PdPreRequisite
                    {
                        Completed =  section.Points >= section.MinPoints,
                        Message =$"Minimum {section.MinPoints} points from {section.SectionName}.  Current Points: {section.Points}"
                    }
                });
            }

            section.PreRequisites = section.PreRequisites.Concat(categories.SelectMany(x => x.PreRequisites).GroupBy(x => x.Message).Select(y => new PdPreRequisite { Completed = y.Any(z => z.Completed), Message = y.Key }));

            section.Completed = section.PreRequisites.All(x => x.Completed) && categories.All(x => x.Completed);
            return section;
        }


        private DateTime GetMinimumStartDateForActivities(CertificationPeriodDto certificationPeriod, int naatiNumber, int totalExistingPeriods)
        {

            var activeRecertificationApplication = new GetApplicationSearchRequest
            {
                Filters = new[]
                {
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.ApplicationReferenceString,
                        Values = new[] {$"APP{certificationPeriod.CredentialApplicationId}"}
                    },
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.ApplicationTypeCategoryIntList,
                        Values = new[] {((int) CredentialApplicationTypeCategoryName.Recertification).ToString()}
                    },

                },
                Take = 1
            };

            var recertificationApplication = mApplicationQueryService.SearchApplication(activeRecertificationApplication)
                .Results.OrderByDescending(x => x.EnteredDate)
                .FirstOrDefault();
            
    
            if (recertificationApplication != null)
            {
                return recertificationApplication.EnteredDate.Date;
            }

            var transitionApplicationRequest = new GetApplicationSearchRequest
            {
                Filters = new[]
                {
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.ApplicationReferenceString,
                        Values = new[] {$"APP{certificationPeriod.CredentialApplicationId}"}
                    },
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.ActiveApplicationBoolean,
                        Values = new[] {false.ToString()}
                    },
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.ApplicationTypeCategoryIntList,
                        Values = new[] {((int) CredentialApplicationTypeCategoryName.Transition).ToString()}
                    },
                    new ApplicationSearchCriteria
                    {
                        Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                        Values = new[] {((int) CredentialRequestStatusTypeName.CertificationIssued).ToString()}
                    },
                },
                Take = 1
            };

            var hasTransitionApplication = mApplicationQueryService.SearchApplication(transitionApplicationRequest).Results.Any();

            if (totalExistingPeriods == 1 && hasTransitionApplication)
            {
                var startDate = mSystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "PDTransitionStartDate" });
                return DateTime.Parse(startDate.Value, CultureInfo.InvariantCulture);
            }

            return certificationPeriod.StartDate;
        }
        private bool IsYearInPeriod(DateTime startDate, DateTime endDate, int year)
        {
            var yearStartDate = new DateTime(year, 1, 1);
            var yearEndDate = new DateTime(year, 12, 31);
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
