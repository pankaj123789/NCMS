using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl.MaterialRequest
{
    public class MaterialRequestDocumentHelper : DocumentTokenReplacerHelper<TestMaterialRequestSearchDto>
    {
        public MaterialRequestDocumentHelper(ITokenReplacementService tokenReplacementService, string licenseName) : base(tokenReplacementService, licenseName)
        {
        }

        protected override IDictionary<string, string> GetTokens(TestMaterialRequestSearchDto data)
        {
            var skillToken = GetTokenNameFor(TokenReplacementField.Skill);
            var testMaterialToken = GetTokenNameFor(TokenReplacementField.TestMaterialId);
            var panelLanguage = GetTokenNameFor(TokenReplacementField.PanelLanguage);
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { skillToken, data.Language },
                { testMaterialToken, data.OutputMaterialId.ToString() },
                { panelLanguage, data.PanelLanguageName }
            };
        }
    }
}
