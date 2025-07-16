using System;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Stamp
{
    public class StampOrder
    {
        public string Skill { get; set; }
        public string Level { get; set; }
        public string Direction { get; set; }
        [UIHint("dateOnly")]
        public DateTime? Expiry { get; set; }

        [UIHint("integer")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Quantity of Rubber Stamps must not be negative.")]        
        public int QuantityRubber { get; set; }

        [UIHint("integer")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Quantity of Self Inking stamps must not be negative.")]        
        public int QuantitySelfInking { get; set; }
    }
}