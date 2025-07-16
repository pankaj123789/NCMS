namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public class PortalStatistic
    {
        public PortalStatistic(string productService, int paymentCount, int downloadCount)
        {
            ProductService = productService;
            PaymentCount = paymentCount;
            DownloadCount = downloadCount;
        }


        public string ProductService { get; private set; }


        public int PaymentCount { get; private set; }


        public int DownloadCount { get; private set; }
    }
}
