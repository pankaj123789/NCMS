namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetMaterialRequestsInfoRequest
    {
        public int[] CoordinatorNAATINumberIntList { get; set; }
        public int[] RoundStatusTypeIdIntList { get; set; }
        public int[] MaterialRequestIdIntList { get; set; }
    }
}