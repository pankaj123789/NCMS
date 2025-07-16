using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using F1Solutions.Naati.Common.Bl.ProfessionalDevelopment;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using NSubstitute;
using Xunit;

namespace MyNaati.Test.F1Solutions.Naati.Common.ProfessionalDevelopment
{
    public class MyNaatiActivityPointsCalculatorHelperTest
    {
        private RequirementConfiguration GetBasicRequirementConfiguration(int requirementId)
        {
            return new RequirementConfiguration
            {
                CategoryRequirementId = requirementId + 1000,
                Points = 0,
                RequirementId = requirementId
            };
        }

        private CategoryConfiguration GetBasicCategoryConfiguation(int categoryId)
        {
            return new CategoryConfiguration
            {
                CategoryId = categoryId,
                CategoryGroupId = 1,
                CategoryGroupName = "Category Group 1",
                CategoryGroupRequiredPointsPerYear = 20,
                CategoryName = "Category " + categoryId,
                PointsLimit = 0,
                PointsLismitTypeId = null,
                Requirements = new List<RequirementConfiguration>
                {
                    GetBasicRequirementConfiguration(1)
                },


            };
        }


        private SectionPointsConfiguration GetBasicSectionConfiguration(int sectionId)
        {
            return new SectionPointsConfiguration
            {
                SectionId = sectionId,
                SectionName = "Section" + sectionId,
                RequiredPointsPerYear = 11,
                Categories = new List<CategoryConfiguration>
                {
                    GetBasicCategoryConfiguation(1),
                    GetBasicCategoryConfiguation(2)
                },

            };
        }

        ActivityPointsConfigurationResponse GetBasicActivityPointsConfigurationResponse()
        {
            return new ActivityPointsConfigurationResponse
            {
                RequiredPointsPerYear = 0,
                Sections = new List<SectionPointsConfiguration>
                {
                    GetBasicSectionConfiguration(1),
                    GetBasicSectionConfiguration(2)
                }
            };
        }

        ProfessionalDevelopmentActivityDto GetBasicProfessionalDevelopmentActivityResponse(int id,
            DateTime dateCompleted, int categoryId)
        {
            return new ProfessionalDevelopmentActivityDto
            {
                Id = id,
                Points = 0,
                SectionIds = new List<int>(),

                DateCompleted = dateCompleted,
                Description = string.Empty,
                Notes = string.Empty,
                ProfessionalDevelopmentCategory = GetProfessionalDevelopmentCategoryResponse(categoryId),
                ProfessionalDevelopmentCategoryGroupId = null,
                ProfessionalDevelopmentCategoryGroupName = null,
                ProfessionalDevelopmentCategoryId = categoryId,
                ProfessionalDevelopmentCategoryName = string.Empty,
            };
        }

        ProfessionalDevelopmentCategoryResponse GetProfessionalDevelopmentCategoryResponse(int id)
        {
            return new ProfessionalDevelopmentCategoryResponse
            {
                Description = String.Empty,
                Id = id,
                Name = String.Empty,
                ProfessionalDevelopmentCategoryGroup = null

            };
        }


        [Fact]
        public void CaluculatePointsFor_WhenUserDoesntHaveAValidCertificationPeriod_ThenZeroPointsAreAssignedForEachPDCategory()
        {
            var naatiNumber = 12345;
            var certificationPeriodId = 0;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId).Returns(CertificationPeriodRecertificationStatus.None);
            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse() { List = Enumerable.Empty<ProfessionalDevelopmentActivityDto>() });


            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = Enumerable.Empty<CertificationPeriodDto>()
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });
            var systemQueryService = Substitute.For<ISystemQueryService>();


            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);



            //Act
            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            Assert.Equal(0, result.Sections.Count());
            Assert.Equal(0, result.Points);
            


        }

        [Fact]
        public void CaluculatePointsFor_WhenUserDoesntHaveActivities_ThenZeroPointsAreAssignedForEachPDCategory()
        {
            var naatiNumber = 12345;

            var startCertficationPeriod = new DateTime(2019,5,28).AddDays(-10);
            var endCertficationPeriod = new DateTime(2019,5,28).AddDays(10);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId).Returns(CertificationPeriodRecertificationStatus.None);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse() { List = Enumerable.Empty<ProfessionalDevelopmentActivityDto>() });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>()
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();


            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            Assert.Equal(pointsConfiguration.Sections.Count(), result.Sections.Count());
            Assert.Equal(0, result.Points);
        }


        [Fact]
        public void CalculatePointsFor_WhenUserHasMultipleActivities_ThenPointsAreCalculated()
        {
            var naatiNumber = 12345;
            var pDTransitionStartDate = new DateTime(2019,5,28).AddYears(-3);
            var startCertficationPeriod = new DateTime(2019,5,28).AddDays(-10);
            var endCertficationPeriod = new DateTime(2019,5,28).AddDays(10);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId).Returns(CertificationPeriodRecertificationStatus.None);
            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var firstSection = pointsConfiguration.Sections.First();
            var firstCategory = firstSection.Categories.First();

            var firstActivity =
                GetBasicProfessionalDevelopmentActivityResponse(1, new DateTime(2019,5,28), firstCategory.CategoryId);
            firstActivity.Points = 10;

            var secondActivity =
                GetBasicProfessionalDevelopmentActivityResponse(2, new DateTime(2019,5,28), firstCategory.CategoryId);
            secondActivity.Points = 15;

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse
                {
                    List = new[]
                    {
                        firstActivity,
                        secondActivity
                    }
                });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>()
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "PDTransitionStartDate"))
                .Returns(new GetSystemValueResponse
                {
                    Value = pDTransitionStartDate.ToString(CultureInfo.InvariantCulture)
                });

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            var expectedPoints = firstActivity.Points + secondActivity.Points;
            Assert.Equal(expectedPoints, result.Sections.First(x => x.SectionId == firstSection.SectionId).Points);

        }
        

        [Fact]
        public void CaluculatePointsFor_WhenCategoryHasMaxPointsPerApplicationLimit_ThenPointsLimitAreApplied()
        {
            var naatiNumber = 12345;
            var pDTransitionStartDate = new DateTime(2019,5,28).AddYears(-3);
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddYears(2);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);


            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var firstSection = pointsConfiguration.Sections.First();
            var firstCategory = firstSection.Categories.First();
            firstCategory.PointsLismitTypeId = (int)PointsLimitTypeName.MaxPointsPerApplication;
            firstCategory.PointsLimit = 20;
            var years = 2;


            var activities = new List<ProfessionalDevelopmentActivityDto>();
            for (var i = 1; i < 3; i++)
            {
                // Activites before certification period
                var activity = GetBasicProfessionalDevelopmentActivityResponse(i, startCertficationPeriod.AddDays(-10),
                    firstCategory.CategoryId);
                activity.Points = 10;
                activities.Add(activity);
            }

            for (var i = 1; i < years * 365; i = i + 100)
            {
                // activities during certificatin period
                var activity = GetBasicProfessionalDevelopmentActivityResponse(i, startCertficationPeriod.AddDays(i),
                    firstCategory.CategoryId);
                activity.Points = 10;
                activities.Add(activity);
            }

            for (var i = 1; i < 3; i++)
            {
                // activites after cter
                var activity =
                    GetBasicProfessionalDevelopmentActivityResponse(i, startCertficationPeriod,
                        firstCategory.CategoryId);
                activity.Points = 10;
                activities.Add(activity);
            }

            logbookQueryService
                .GetProfessionalDevelopmentActivities(
                    Arg.Is<GetActivitiesRequest>(x => x.StartDate == startCertficationPeriod &&
                                                      x.EndDate == endCertficationPeriod))
                .Returns(new ProfessionalDevelopmentActivityResponse { List = activities });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "PDTransitionStartDate"))
                .Returns(new GetSystemValueResponse
                {
                    Value = pDTransitionStartDate.ToString(CultureInfo.InvariantCulture)
                });

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            var expectedPoints = firstCategory.PointsLimit;
            Assert.Equal(expectedPoints, result.Sections.First(x => x.SectionId == firstSection.SectionId).Points);

        }

        [Fact]
        public void CaluculatePointsFor_WhenCategoryHasMaxPointsPerYearLimit_ThenPointsLimitAreApplied()
        {
            var naatiNumber = 12345;
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddYears(2);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);
            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var firstSection = pointsConfiguration.Sections.First();
            var firstCategory = firstSection.Categories.First();
            firstCategory.PointsLismitTypeId = (int)PointsLimitTypeName.MaxPointsPerYear;
            firstCategory.PointsLimit = 20;
            var years = 2;


            var activities = new List<ProfessionalDevelopmentActivityDto>();


            for (var i = 4; i < years * 365; i = i + 30)
            {
                // activities during certificatin period
                var activity = GetBasicProfessionalDevelopmentActivityResponse(i, startCertficationPeriod.AddDays(i),
                    firstCategory.CategoryId);
                activity.Points = 10;
                activities.Add(activity);
            }

            logbookQueryService
                .GetProfessionalDevelopmentActivities(
                    Arg.Is<GetActivitiesRequest>(x => x.StartDate == startCertficationPeriod &&
                                                      x.EndDate == endCertficationPeriod))
                .Returns(new ProfessionalDevelopmentActivityResponse { List = activities });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            var expectedPoints = firstCategory.PointsLimit * years;
            Assert.Equal(expectedPoints, result.Sections.First(x => x.SectionId == firstSection.SectionId).Points);

        }

        [Fact]
        public void CaluculatePointsFor_WhenUserHasRecertificationApplication_ThenActiviesFromRecertificationEnteredDateAreRequested()
        {
            var naatiNumber = 12345;
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddYears(2);

            var recertificationEnteredDate = new DateTime(2019,5,28).Date.AddDays(-30);

            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse() { List = Enumerable.Empty<ProfessionalDevelopmentActivityDto>() });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });
            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Is<GetApplicationSearchRequest>(x => x.Filters.Any(y => y.Values.Any(v => v == ((int)CredentialApplicationTypeCategoryName.Recertification).ToString()))))
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = new List<ApplicationSearchDto>() { new ApplicationSearchDto() { EnteredDate = recertificationEnteredDate } }
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            logbookQueryService.Received()
                .GetProfessionalDevelopmentActivities(
                    Arg.Is<GetActivitiesRequest>(x => x.NaatiNumber == naatiNumber &&
                                                      x.StartDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) ==
                                                      recertificationEnteredDate.ToString(CultureInfo.InvariantCulture)));

        }

        [Fact]
        public void CaluculatePointsFor_WhenCertificationPeriodHasASubmittedRecertification_ThenAllAttachedActivitiesAreUsedToCalculateThePoints()
        {
            var naatiNumber = 12345;
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddYears(2);
            var recertificationApplicationId = 12458;

            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.BeingAssessed);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            logbookQueryService
                .GetSubmittedRecertificationApplicationForPeriod(Arg.Is<int>(x => x == recertificationApplicationId))
                .Returns(new RecertificationDto() {ApplicationId = recertificationApplicationId});

            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);
            logbookQueryService.GetSubmittedRecertificationApplicationForPeriod(Arg.Any<int>()).Returns(new RecertificationDto(){ ApplicationId = recertificationApplicationId });

            var firstSection = pointsConfiguration.Sections.First();
            firstSection.RequiredPointsPerYear = 10;
            var firstCategory = firstSection.Categories.First();

            var firstActivity =
                GetBasicProfessionalDevelopmentActivityResponse(1, new DateTime(2019,5,28).AddDays(-3000), firstCategory.CategoryId);
            firstActivity.Points = 10;

            var secondActivity =
                GetBasicProfessionalDevelopmentActivityResponse(2, new DateTime(2019,5,28), firstCategory.CategoryId);
            secondActivity.Points = 10;

            var thirdActivity =
                GetBasicProfessionalDevelopmentActivityResponse(2, new DateTime(2019,5,28).AddDays(1000), firstCategory.CategoryId);
            secondActivity.Points = 10;

            logbookQueryService.GetRecertificationAtivitiesForApplication(Arg.Is<int>(x => x == recertificationApplicationId))
                .Returns(new[]
                {
                    firstActivity,
                    secondActivity,
                    thirdActivity
                });
         
            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod,
                        }
                    }
                });
            var applicationQueryService = Substitute.For<IApplicationQueryService>();

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert
            var expectedPoints = firstActivity.Points + secondActivity.Points + thirdActivity.Points;
            Assert.Equal(expectedPoints, result.Sections.First(x => x.SectionId == firstSection.SectionId).Points);

        }

        [Fact]
        public void CaluculatePointsFor_WhenUserHasTransitionApplication_ThenActiviesFromPdTransitionStartDateAreRequested()
        {
            var naatiNumber = 12345;
            var pDTransitionStartDate = new DateTime(2019,5,28).AddYears(-3);
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddYears(2);

            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse() { List = Enumerable.Empty<ProfessionalDevelopmentActivityDto>() });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = endCertficationPeriod
                        }
                    }
                });
            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Is<GetApplicationSearchRequest>(x => x.Filters.Any(y => y.Values.Any(v => v == ((int)CredentialApplicationTypeCategoryName.Recertification).ToString()))))
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });


            applicationQueryService.SearchApplication(Arg.Is<GetApplicationSearchRequest>(x => x.Filters.Any(y => y.Values.Any(v => v == ((int)CredentialApplicationTypeCategoryName.Transition).ToString()))))
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = new List<ApplicationSearchDto>() { new ApplicationSearchDto() }
                });



            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "PDTransitionStartDate"))
                .Returns(new GetSystemValueResponse
                {
                    Value = pDTransitionStartDate.ToString(CultureInfo.InvariantCulture)
                });

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            logbookQueryService.Received()
                .GetProfessionalDevelopmentActivities(
                    Arg.Is<GetActivitiesRequest>(x => x.NaatiNumber == naatiNumber &&
                                                      x.StartDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) ==
                                                      pDTransitionStartDate.ToString(CultureInfo.InvariantCulture)));

        }
        [Fact]
        public void CaluculatePointsFor_WhenCertificationPeriodTouch2020_ThenMinimumPointsExcludesOneYear()
        {

            var naatiNumber = 12345;
            var startCertificationPeriod = new DateTime(2019, 5, 28).AddYears(-2);
            var endCertificationPeriod = new DateTime(2022, 5, 28).AddDays(2);
            var orignalCertificationPeriodEnd = endCertificationPeriod.AddMonths(1);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var firstSection = pointsConfiguration.Sections.First();
            firstSection.RequiredPointsPerYear = 10;
            var firstCategory = firstSection.Categories.First();

            var firstActivity =
                GetBasicProfessionalDevelopmentActivityResponse(1, orignalCertificationPeriodEnd, firstCategory.CategoryId);
            firstActivity.Points = 10;

            var secondActivity =
                GetBasicProfessionalDevelopmentActivityResponse(2, orignalCertificationPeriodEnd, firstCategory.CategoryId);
            secondActivity.Points = 15;

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse
                {
                    List = new[]
                    {
                        firstActivity,
                        secondActivity
                    }
                });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>()
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertificationPeriod,
                            EndDate = endCertificationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = orignalCertificationPeriodEnd
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            var sectionResult = result.Sections.First(x => x.SectionId == firstSection.SectionId);
            var expectedPoints = firstActivity.Points + secondActivity.Points;
            var expectedMinumSectionPoints = (int)(firstSection.RequiredPointsPerYear * (int)(((orignalCertificationPeriodEnd - startCertificationPeriod).TotalDays -365) / 365));

            Assert.Equal(expectedPoints, sectionResult.Points);
            Assert.Equal(expectedMinumSectionPoints, sectionResult.MinPoints);

            logbookQueryService.Received()
                .GetProfessionalDevelopmentActivities(Arg.Is<GetActivitiesRequest>(x => x.NaatiNumber == naatiNumber && x.EndDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) == endCertificationPeriod.ToString(CultureInfo.InvariantCulture)));
        }

        [Fact]
        public void CaluculatePointsFor_WhenEndDateIsDifferentToOriginalDate_ThenActiviesFromEndDateAreRequestedAndMinimumPointsAreCalcualatedWithEndDate()
        {

            var naatiNumber = 12345;
            var startCertficationPeriod = new DateTime(2019,5,28).AddYears(-2);
            var endCertficationPeriod = new DateTime(2019,5,28).AddDays(2);
            var orignalCertificationPeriodEnd = endCertficationPeriod.AddMonths(1);
            var certificationPeriodId = 1;
            var businessService = Substitute.For<IApplicationBusinessLogicService>();
            businessService.CalculateCertificationPeriodRecertificationStatus(certificationPeriodId)
                .Returns(CertificationPeriodRecertificationStatus.None);

            var logbookQueryService = Substitute.For<ILogbookQueryService>();
            var pointsConfiguration = GetBasicActivityPointsConfigurationResponse();
            logbookQueryService.GetActivityPointsConfiguration().Returns(pointsConfiguration);

            var firstSection = pointsConfiguration.Sections.First();
            firstSection.RequiredPointsPerYear = 10;
            var firstCategory = firstSection.Categories.First();

            var firstActivity =
                GetBasicProfessionalDevelopmentActivityResponse(1, orignalCertificationPeriodEnd, firstCategory.CategoryId);
            firstActivity.Points = 10;

            var secondActivity =
                GetBasicProfessionalDevelopmentActivityResponse(2, orignalCertificationPeriodEnd, firstCategory.CategoryId);
            secondActivity.Points = 15;

            logbookQueryService.GetProfessionalDevelopmentActivities(Arg.Any<GetActivitiesRequest>())
                .Returns(new ProfessionalDevelopmentActivityResponse
                {
                    List = new[]
                    {
                        firstActivity,
                        secondActivity
                    }
                });

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Any<GetCertificationPeriodsRequest>())
                .Returns(new GetCertificationPeriodsResponse
                {
                    Results = new List<CertificationPeriodDto>()
                    {
                        new CertificationPeriodDto
                        {
                            StartDate = startCertficationPeriod,
                            EndDate = endCertficationPeriod,
                            Id = certificationPeriodId,
                            OriginalEndDate = orignalCertificationPeriodEnd
                        }
                    }
                });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            applicationQueryService.SearchApplication(Arg.Any<GetApplicationSearchRequest>())
                .Returns(new ApplicationSearchResultResponse()
                {
                    Results = Enumerable.Empty<ApplicationSearchDto>()
                });

            var systemQueryService = Substitute.For<ISystemQueryService>();

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse
                {
                    Value = 3.ToString()
                });

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logbookQueryService, personQueryService,
                applicationQueryService, systemQueryService, businessService);

            //Act

            var result = calculator.CaluculatePointsFor(naatiNumber, certificationPeriodId);

            //Assert

            var sectionResult = result.Sections.First(x => x.SectionId == firstSection.SectionId);
            var expectedPoints = firstActivity.Points + secondActivity.Points;
            var expectedMinumSectionPoints = (int)(firstSection.RequiredPointsPerYear * (int)((orignalCertificationPeriodEnd - startCertficationPeriod).TotalDays / 365));

            Assert.Equal(expectedPoints, sectionResult.Points);
            Assert.Equal(expectedMinumSectionPoints, sectionResult.MinPoints);

            logbookQueryService.Received()
                .GetProfessionalDevelopmentActivities(Arg.Is<GetActivitiesRequest>(x => x.NaatiNumber == naatiNumber && x.EndDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) == endCertficationPeriod.ToString(CultureInfo.InvariantCulture)));
        }

        private CertificationPeriodDto CreatePeriod(int id, DateTime startDate, DateTime endDate, CertificationPeriodStatus status)
        {
            return new CertificationPeriodDto
            {
                Id = id,
                StartDate = startDate,
                OriginalEndDate = endDate,
                EndDate = endDate,
                CertificationPeriodStatus = status
            };
        }

        [Theory]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.None, true, true, true)]
        [InlineData(CertificationPeriodStatus.Future, CertificationPeriodRecertificationStatus.None, true, true, true)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.None, true, true, true)]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.BeingAssessed, false, false, false)]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.Completed, false, false, false)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.BeingAssessed, false, false, false)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.Completed, false, false, false)]

        public void GetCertificationPeriodDetails_WhenCalled_ThenTheCertificationPeriodWithTheProperStatusAreReturned(CertificationPeriodStatus status, CertificationPeriodRecertificationStatus recertificationStatus, bool expectedDefault, bool expectedEditable, bool expectedCurrent)
        {
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    CreatePeriod(validPeriodId,certificationPeriodStartDate, certificationPeriodEndDate, status)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService
                .GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var applicationQueryService = Substitute.For<IApplicationQueryService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(recertificationStatus);

            var calculator = new MyNaatiActivityPointsCalculatorHelper(logBookService, personQueryService, applicationQueryService, systemQueryService, businessLogicService);

            var periods = calculator.GetCertificationPeriodDetails(naatiNumber).ToList();

            var foundPeriod = periods.First(x => x.Id == validPeriodId);
            Assert.Equal(expectedDefault, foundPeriod.IsDefault);
            Assert.Equal(expectedEditable, foundPeriod.Editable);
            Assert.Equal(expectedCurrent, foundPeriod.IsCurrent);
        }
    }
}
