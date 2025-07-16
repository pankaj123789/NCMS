using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Logbook
{
	public class ActivityModel
	{
		public DateTime DateCompleted { get; set; }
		public string Description { get; set; }
		public string Notes { get; set; }
		public int ProfessionalDevelopmentCategoryId { get; set; }
		public int ProfessionalDevelopmentRequirementId { get; set; }
		public int Id { get; set; }
		public string ProfessionalDevelopmentCategoryName { get; set; }
		public string ProfessionalDevelopmentCategoryGroupName { get; set; }
		public int Points { get; set; }
		public RequirementModel ProfessionalDevelopmentRequirement { get; set; }
		public CategoryModel ProfessionalDevelopmentCategory { get; set; }
		public IEnumerable<int> SectionIds { get; set; }
	}
}
