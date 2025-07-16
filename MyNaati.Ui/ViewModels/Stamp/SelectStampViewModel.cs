using System.Collections.Generic;

namespace MyNaati.Ui.ViewModels.Stamp
{
    public class SelectStampViewModel
    {
        public List<StampOrder> StampOrders { get; set; }

        public int TotalRubber { get; set; }
        
        public int TotalSelfInking { get; set; }
    }
}