using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class MarkingPaymentStatusModel
    {
        public List<SelectListItem> Statuses { get; set; }

        [DisplayName("Attendance ID")]
        public string AttendanceID { get; set; }

        [DisplayName("Submitted")]
        public DateTime? SubmittedFrom { get; set; }

        [DisplayName("to")]
        public DateTime? SubmittedTo { get; set; }

        public string[] PayrollStatus { get; set; }
    }
}
