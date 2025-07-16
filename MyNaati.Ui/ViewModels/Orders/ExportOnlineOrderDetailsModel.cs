using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Orders
{
    public class ExportOnlineOrderDetailsModel
    {

        [DisplayName("From")]
        [Required(ErrorMessage = "You must provide a From date.")]
        [DateComparison(OtherProperty = "EndDate", ErrorMessage = "To date must be later than from date.")]
        public DateTime StartDate { get; set; }
        [DisplayName("To")]
        [Required(ErrorMessage = "You must provide a To date.")]
        public DateTime EndDate { get; set; }


    }


   
}