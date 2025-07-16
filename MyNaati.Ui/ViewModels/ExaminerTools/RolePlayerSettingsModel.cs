using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.ExaminerTools
{
    public class RolePlayerSettingsModel
    {
        [Required]
        public int[] RolePlayLocations { get; set; }
		[Required]
		[Display(Name = "Maximum Sessions")]
		public int MaximumRolePlaySessions { get; set; }

		public RolePlayerSettingsModel()
		{
			RolePlayLocations = new int[] { };
		}
    }
}