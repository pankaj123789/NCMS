using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface IPersonEmailVerificationCodeDalService : IQueryService
    {
        /// <summary>
        /// Get ExpireStart Date
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns></returns>
        GenericResponse<DateTime?>  GetEmailCodeExpireStartDate(int naatiNumber);



        /// <summary>
        /// Get last code generated. 0 if not used before
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns></returns>
        GenericResponse<int> GetLastEmailCode(int personId);

        /// <summary>
        /// Set Last Email Code
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>Eamil Address</returns>
        GenericResponse<string> SetLastEmailCode(int naatiNumber, int emailCode);


    }
}