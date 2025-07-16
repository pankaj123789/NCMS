using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface IApplicationBusinessLogicService
    {
        RecertificationStatus CalculateCredentialRecertificationStatus(int credentialId, bool checkSkillAvailability = true);
        CertificationPeriodRecertificationStatus CalculateCertificationPeriodRecertificationStatus(int certificationPeriodId);
        ProductSpecificationModel GetProductSpecificationFee(int applicationTypeId, int credentialTypeId, FeeTypeName supplementaryTest);
    }
}