namespace MyNaati.Ui.ViewModels.Shared
{
    public class WizardStep
    {
        public string Name { get; set; }
        public string Action { get; set; }

        /// <summary>
        /// Can this step be clicked on?
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool AllowDirectNavigation { get; set; }

        /// <summary>
        /// Should this step be visited by Next/Previous clicks?
        /// </summary>
        public bool Skipped { get; set; }

        public bool Completed { get; set; }
        
        /// <summary>
        /// Is this step visible in the list of steps?
        /// </summary>
        public bool HideInStepList { get; set; }

        public WizardStep()
        {
            AllowDirectNavigation = false;
            Skipped = false;
        }
    }
}