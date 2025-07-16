using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Orders
{
    public class PortalStatisticsModel
    {
        [DisplayName("From")]
        [Required]
        [DateComparison(OtherProperty = "EndDate", ErrorMessage = "From date cannot be later than to date.")]
        public DateTime StartDate { get; set; }

        [DisplayName("To")]
        [Required]
        public DateTime EndDate { get; set; }

    }
}
