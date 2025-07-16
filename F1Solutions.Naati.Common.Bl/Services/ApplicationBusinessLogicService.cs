using System;
using System.Linq;
using AutoMapper;
//using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.Properties;
using F1Solutions.Naati.Common.Contracts.Bl.ExtensionHelpers;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.Services
{
    public class ApplicationBusinessLogicService : IApplicationBusinessLogicService
    {
        readonly IApplicationQueryService _applicationService;
        readonly ISystemQueryService _systemService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ApplicationBusinessLogicService(IApplicationQueryService applicationService, ISystemQueryService systemService, IAutoMapperHelper autoMapperHelper)
        {
            _applicationService = applicationService;
            _systemService = systemService;
            _autoMapperHelper = autoMapperHelper;
        }

        public RecertificationStatus CalculateCredentialRecertificationStatus(int credentialId, bool checkSkillAvailability = true)
        {
            RecertificationRequestStatusResponse credentialStatus = _applicationService.GetRecertificationRequestStatus(credentialId);

            if (credentialStatus.CertificationPeriod == null ||(checkSkillAvailability && !credentialStatus.AllowRecertification))
            {
                return RecertificationStatus.None;
            }

            var systemValueResponse = _systemService.GetSystemValue(new GetSystemValueRequest { ValueKey = "CertificationPeriodRecertifyExpiry" });
            var recertifyExpiryMonths = int.Parse(systemValueResponse.Value);

            return CalculateCredentialRecertificationStatus(credentialStatus.SubmitedRecertificationRequest, 
                credentialStatus.CredentialTerminationDate,
                credentialStatus.CertificationPeriod,
                credentialStatus.RecertificationInfo,
               recertifyExpiryMonths);
        }

        /// <summary>
        /// Implements UC-BackOffice-3001.10 BR9
        /// </summary>
        private RecertificationStatus CalculateCredentialRecertificationStatus(
            CredentialRequestInfoDto submittedCredentialRequest,
            DateTime? credentialTerminationDate,
            CertificationPeriodDto period, RecertificationDto recertificationApplication, int recertifyExpiryMonths)
        {
            

            if (submittedCredentialRequest?.CredentialRequestStatusType == CredentialRequestStatusTypeName.AssessmentFailed)
            {
                return RecertificationStatus.Failed;
            }

            if (submittedCredentialRequest != null)
            {
                return RecertificationStatus.BeingAssessed;
            }

            bool todayIsInsideRecertificationPeriod =
                DateTime.Today >= period.OriginalEndDate.AddMonths(-recertifyExpiryMonths) &&
                DateTime.Today <= period.EndDate.AddMonths(recertifyExpiryMonths);

            if (todayIsInsideRecertificationPeriod)
            {
                var aRecertificatonAplicationExists = recertificationApplication != null;

                // a terminated credential cannot be eligible for recertification
                if (!(credentialTerminationDate <= DateTime.Today))
                {
                    if (aRecertificatonAplicationExists)
                    {
                        // this credential is eligible for recertification, but this is already a recertification application in flight for this 
                        // certification period, so this credential can be added to it
                        return RecertificationStatus.EligibleForExisting;
                    }

                    // this credential is eligible for recertification, and there are no in-flight recertification applications, 
                    // so a new one can be created
                    return RecertificationStatus.EligibleForNew;
                }
            }

            return RecertificationStatus.None;
        }

        public CertificationPeriodRecertificationStatus CalculateCertificationPeriodRecertificationStatus(int certificationPeriodId)
        {
            var periodDetails = _applicationService.GetCertificationPeriodDetails(certificationPeriodId);

            if (periodDetails.Recertifications.Any(x => x.CredentialApplicationStatus == CredentialApplicationStatusTypeName.Completed))
            {
                return CertificationPeriodRecertificationStatus.Completed;
            }

            if (periodDetails.Recertifications.Any(x => x.CredentialApplicationStatus.IsActiveStatus()))
            {
                return CertificationPeriodRecertificationStatus.BeingAssessed;
            }

            return CertificationPeriodRecertificationStatus.None;
        }

        public ProductSpecificationModel GetProductSpecificationFee(int applicationTypeId, int credentialTypeId, FeeTypeName feeTypeName)
        {

            var productFeeValidationWithCredentialTypeMessage = feeTypeName == FeeTypeName.Test ? Resources.ProductFeeValidationWithCredentialTypeForTest : Resources.ProductFeeValidationWithCredentialTypeForSupplementaryTest;
            var productFeeValidationWithNoCredentialTypeMessage = feeTypeName == FeeTypeName.Test ? Resources.ProductFeeValidationWithNoCredentialTypeForTest : Resources.ProductFeeValidationWithNoCredentialTypeForSupplementaryTest;

            var feesResponse = _applicationService.GetApplicationTypeFees(applicationTypeId, feeTypeName);

            if (feesResponse.FeeProducts == null)
            {
                return null;
            }

            if (feesResponse.FeeProducts.Any())
            {
                var feeProductsWithNoCredentialType = feesResponse.FeeProducts.Where(x => x.CredentialTypeId == null)
                    .OrderByDescending(x => x.ProductSpecification.CostPerUnit).ToList();

                feeProductsWithNoCredentialType.Requires(x => x.Select(y => y.ProductSpecification).Count() <= 1, productFeeValidationWithNoCredentialTypeMessage);

                if (!feeProductsWithNoCredentialType.Any())
                {
                    var feeProductsWithCredentialType = feesResponse.FeeProducts.Where(x => x.CredentialTypeId.HasValue && x.CredentialTypeId.Value == credentialTypeId)
                        .ToList();

                    feeProductsWithCredentialType.Requires(x => x.Select(y => y.ProductSpecification).Count() <= 1, productFeeValidationWithCredentialTypeMessage);

                    var feeProduct = _autoMapperHelper.Mapper.Map<ProductSpecificationModel>(feeProductsWithCredentialType.FirstOrDefault().ProductSpecification);
                    feeProduct.CredentialFeeProductId = feeProductsWithCredentialType.FirstOrDefault().Id;
                    return feeProduct;
                }

                var feeProductWithNoType = _autoMapperHelper.Mapper.Map<ProductSpecificationModel>(feeProductsWithNoCredentialType.FirstOrDefault().ProductSpecification);
                feeProductWithNoType.CredentialFeeProductId = feeProductsWithNoCredentialType.FirstOrDefault().Id;
                return feeProductWithNoType;
            }

            return null;
        }


        public CredentialRequestDto GetCredentialRequestForCredential(int credentialId)
        {
            throw new NotImplementedException();
        }
    }
}
