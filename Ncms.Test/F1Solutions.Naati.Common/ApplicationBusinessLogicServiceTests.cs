using System;
using System.Collections.Generic;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using NSubstitute;
using Xunit;

namespace Ncms.Test.F1Solutions.Naati.Common
{
    public class ApplicationBusinessLogicServiceTests
    {
        [Theory]
        [MemberData(nameof(CreateCalculateRecertificationStatusTestData))]
        public void CalculateCredentialRecertificationStatus_WhenMethodIsCalled_CorrectStatusIsReturned(int testId,  RecertificationRequestStatusResponse input, int recertEligibilityMonths,
            RecertificationStatus expectedResult)
        {
            // arrange
            var appSvc = Substitute.For<IApplicationQueryService>();
            appSvc.GetRecertificationRequestStatus(1).Returns(input);

            var sysSvc = Substitute.For<ISystemQueryService>();
            sysSvc.GetSystemValue(null).ReturnsForAnyArgs(new GetSystemValueResponse
                                                          {
                                                              Value = recertEligibilityMonths.ToString(),
                                                          });
            var assemblies = new[]
            {
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var service = new ApplicationBusinessLogicService(appSvc, sysSvc, autoMapperHelper);

            // act
            var result = service.CalculateCredentialRecertificationStatus(1);

            // assert
            Assert.True(expectedResult == result, $"TestId :{testId}");
        }

        public static IEnumerable<object[]> CreateCalculateRecertificationStatusTestData()
        {
            int recertEligibilityMonths = 3;

            // original end date in future
            var eligiblePeriod1 = new CertificationPeriodDto
                                  {
                                      OriginalEndDate = DateTime.Today.AddMonths(2),
                                      EndDate = DateTime.Today.AddMonths(4),
                                  };

            // original end date in past
            var eligiblePeriod2 = new CertificationPeriodDto
                                  {
                                      OriginalEndDate = DateTime.Today.AddMonths(-6),
                                      EndDate = DateTime.Today.AddMonths(6),
                                  };

            var ineligiblePeriod = new CertificationPeriodDto
                                   {
                                       OriginalEndDate = DateTime.Today.AddMonths(4),
                                       EndDate = DateTime.Today.AddMonths(4),
                                   };

            var futurePeriod = new CertificationPeriodDto
            {
                StartDate = DateTime.Today.AddMonths(2),
                OriginalEndDate = DateTime.Today.AddYears(3),
                EndDate = DateTime.Today.AddYears(3),
            };

            return new[]
                   {
                       // ineligible period
                       new object[]
                       {
                           1,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = ineligiblePeriod,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.None
                       },

                       // eligible period
                       new object[]
                       {   2,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1, // original end date in future
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.EligibleForNew
                       },

                       // eligible period 2
                       new object[]
                       {
                           3,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod2,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.EligibleForNew
                       },

                       // eligible period, terminated credential
                       new object[]
                       {
                           4,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               CredentialTerminationDate = DateTime.Today.AddMonths(-1),
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.None
                       },
                       

                       // eligible period, existing recert application containing same credential, failed assessment
                       new object[]
                       {
                           5,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = eligiblePeriod1.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.Completed,
                               },
                               SubmitedRecertificationRequest = new CredentialRequestInfoDto()
                               {
                                   CredentialRequestId = 1,
                                   CredentialRequestStatusType = CredentialRequestStatusTypeName.AssessmentFailed
                               },
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.Failed
                       },

                       // eligible period, existing recert application containing same credential, in progress
                       new object[]
                       {
                           6,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = eligiblePeriod1.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.InProgress,

                               },
                               SubmitedRecertificationRequest = new CredentialRequestInfoDto()
                               {
                                   CredentialRequestId = 1,
                                   CredentialRequestStatusType = CredentialRequestStatusTypeName.AwaitingApplicationPayment
                               },
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.BeingAssessed
                       },

                       // eligible period, existing recert application containing same credential, Entered
                       new object[]
                       {
                           7,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = eligiblePeriod1.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.Entered,

                               },
                               SubmitedRecertificationRequest = new CredentialRequestInfoDto()
                               {
                                   CredentialRequestId = 1,
                                   CredentialRequestStatusType = CredentialRequestStatusTypeName.RequestEntered
                               },
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.BeingAssessed
                       },

                       // eligible period, existing recert application containing same credential, completed successfully
                       new object[]
                       {
                           8,
                           new RecertificationRequestStatusResponse
                           {
                               
                               CertificationPeriod = futurePeriod,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = futurePeriod.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.Completed,
                               },
                               SubmitedRecertificationRequest = null,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.None
                       },

                       // eligible period, existing recert application containing same credential, cancelled
                       new object[]
                       {
                           9,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = eligiblePeriod1.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.InProgress,
                               },
                               SubmitedRecertificationRequest = null,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.EligibleForExisting
                       },

                       // eligible period, existing recert application containing same credential, application unsuccessful
                       new object[]
                       {
                           10,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = null,
                               SubmitedRecertificationRequest = null,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.EligibleForNew
                       },

                       // eligible period, existing recert application NOT containing same credential, completed
                       new object[]
                       {
                           11,
                           new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = eligiblePeriod1,
                               RecertificationInfo = new RecertificationDto()
                               {
                                   ApplicationId = 1,
                                   CertificationPeriodId = eligiblePeriod1.Id,
                                   CredentialApplicationStatus = CredentialApplicationStatusTypeName.Completed,
                               },
                               AllowRecertification = true,
                               SubmitedRecertificationRequest = null,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.EligibleForExisting // will have to reopen completed application and add it to that
                       },

                       // recertification not allowed due to skill type
                       new object[]
                       {
                           12,
                            new RecertificationRequestStatusResponse
                           {
                               CertificationPeriod = ineligiblePeriod,
                               AllowRecertification = true,
                           },
                           recertEligibilityMonths,
                           RecertificationStatus.None
                      },
            };
        }
    }
}