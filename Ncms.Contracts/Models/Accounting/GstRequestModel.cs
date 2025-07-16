namespace Ncms.Contracts.Models.Accounting
{
    public class GstRequestModel
    {
        public int ProductSpecificationId { get; set; }
        public double UnitPrice { get; set; }
        public bool GSTApplies { get; set; }
    }
}
