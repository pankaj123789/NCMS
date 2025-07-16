using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class PanelManagementModel
    {
        public List<TestModel> TestList { get; set; }

        public List<SelectListItem> Panels { get; set; }

        public List<SelectListItem> Statuses { get; set; }

        public List<SelectListItem> People { get; set; }

        [DisplayName("Panel name")]
        public string PanelName { get; set; }

        public int[] PanelId { get; set; }

        [DisplayName("Examiner name")]
        public string ExaminerName { get; set; }

        public int[] NAATINumber { get; set; }

        [DisplayName("Date due")]
        public DateTime? DateDueFrom { get; set; }

        [DisplayName("to")]
        public DateTime? DateDueTo { get; set; }

        [DisplayName("Date allocated from")]
        public DateTime? DateAllocatedFrom { get; set; }

        [DisplayName("to")]
        public DateTime? DateAllocatedTo { get; set; }

        [DisplayName("Test status")]
        public string[] TestStatus { get; set; }
    }
}
