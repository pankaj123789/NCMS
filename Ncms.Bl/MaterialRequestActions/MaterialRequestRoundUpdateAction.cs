using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundUpdateAction : MaterialRequestRoundAction
    {

        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.SentForDevelopment };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => CurrentRoundEntryState;
        private readonly IList<string> _notes = new List<string>();
        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                };

                return actions;
            }
        }

        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                {
                    UpdateRoundDetails,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }


        private void UpdateRoundDetails()
        {
            var dueDate = WizardModel.RoundDetails.DueDate;

            if (CheckPropertyChange(() => dueDate, () => RoundModel.DueDate, Naati.Resources.MaterialRequest.DueDate))
            {
                RoundModel.DueDate = dueDate;
            }
           
        }


        private bool CheckPropertyChange<T>(Func<T> wizardProperty, Func<T> modelProperty, string propertyName)
        {
            if (!(wizardProperty()?.Equals(modelProperty()) ?? false))
            {
                var note = string.Format(Naati.Resources.MaterialRequest.MaterialRequestRoundUpdated, propertyName,
                    modelProperty(), wizardProperty(), RoundModel.RoundNumber);
                _notes.Add(note);
                return true;
            }

            return false;
        }
        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            TryInsertToken(tokenDictionary, TokenReplacementField.RoundDueDate, () => WizardModel.RoundDetails.DueDate.ToShortDateString());

            base.GetEmailTokens(tokenDictionary);
        }
        protected override string GetNote()
        {
            return string.Join(". ", _notes);
        }

      
    }
}
