using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNaati.Ui.ViewModels.Shared
{
    public class BaseWizardModel
    {
        public List<WizardStep> Steps { get; set; }

        public int CompletedStepsCount
        {
            get
            {
                if (Steps == null)
                {
                    return 0;
                }
                return Steps.Count(s => s.Completed);
            }
        }

        public int UnskippedStepsCount
        {
            get { return UnskippedSteps.Count; }
        }

        public Guid WizardId { get; set; }
        public bool IsLockedToReadOnly { get; set; }

        public void EnableStep(string actionName)
        {
            Steps.Single(s => s.Action.Equals(actionName)).AllowDirectNavigation = true;
        }

        public void DisableStep(string actionName)
        {
            Steps.Single(s => s.Action.Equals(actionName)).AllowDirectNavigation = false;
        }

        public string GetNextStep(string actionName)
        {
            for (int i = 0; i < UnskippedSteps.Count - 1; i++)
            {
                if (UnskippedSteps[i].Action == actionName)
                    return UnskippedSteps[i + 1].Action;
            }
            return null;
        }

        public string GetPreviousStep(string actionName)
        {
            for (int i = 1; i < UnskippedSteps.Count; i++)
            {
                if (UnskippedSteps[i].Action == actionName)
                    return UnskippedSteps[i - 1].Action;
            }
            return null;
        }

        public bool IsStepSkipped(string actionName)
        {
            return Steps.Single(s => s.Action.Equals(actionName)).Skipped;
        }

        private IList<WizardStep> UnskippedSteps
        {
            get
            {
                if (Steps == null)
                {
                    return new List<WizardStep>();
                }
                return Steps.Where(s => !s.Skipped).ToList();
            }
        }

        public void SetStepSkipped(string actionName, bool isSkipped)
        {
            var step = Steps.Single(s => s.Action == actionName);
            step.Skipped = isSkipped;
        }

        public void SetStepCompleted(string actionName, bool isComplete)
        {
            var step = Steps.SingleOrDefault(s => s.Action == actionName);

            if (step == null)
            {
                return;
            }

            step.Completed = isComplete;
        }
    }
}