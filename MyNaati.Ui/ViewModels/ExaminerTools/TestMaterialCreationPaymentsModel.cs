using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class TestMaterialCreationPaymentsModel
    {
        public List<SelectListItem> Statuses { get; set; }

        [DisplayName("Test Material ID")]
        public string TestMaterialID { get; set; }

        [DisplayName("Submitted")]
        public DateTime? SubmittedFrom { get; set; }

        [DisplayName("to")]
        public DateTime? SubmittedTo { get; set; }

        public string[] PaymentStatus { get; set; }
    }
}
