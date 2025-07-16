using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class AdviseAvailabilityModel
    {
        [Display(Name = "Start Date")]
        [Required]
        public DateTime? StartDate { get; set; }
		[Display(Name = "End Date")]
		public DateTime? EndDate { get; set; }
        public int? Id { get; set; }
        public string Action { get; set; }
		public string RolePlayerAvailable { get; set; }
		public RolePlayerSettingsModel RolePlayerSettings { get; set; }
		public IEnumerable<SelectListItem> Locations { get; set; }

		public AdviseAvailabilityModel()
		{
			RolePlayerSettings = new RolePlayerSettingsModel();
		}
	}
}