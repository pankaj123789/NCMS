using System.IO;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace MyNaati.Contracts.BackOffice.MaterialRequest
{
    
    public interface IMaterialRequestService : IInterceptableservice
    {
        
        Stream ReplaceDocumentTokens(string filePath, int materialRoundId);
    }
    public class GetMaterialRequestsInfoResponse
    {
        public MaterialRequestInfoDto[] MaterialRequests { get; set; }
    }
    public class GetMaterialRequestsInfoRequest
    {
        public int[] CoordinatorNAATINumberIntList { get; set; }
    }
}
