using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Saving;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Bl
{
    public abstract class DocumentTokenReplacerHelper<T> where T : class
    {
        protected ITokenReplacementService TokenReplacementService { get; }
        private readonly string _licenseName;

        protected DocumentTokenReplacerHelper(ITokenReplacementService tokenReplacementService, string licenseName)
        {
            TokenReplacementService = tokenReplacementService;
            _licenseName = licenseName;
        }

        public virtual Stream ReplaceDocumentTokens(T data, string filePath, SaveOptions saveOptions = null, SaveFormat? saveFormat = null)
        {
            var license = new License();
            license.SetLicense(_licenseName);
            Document document;
            using (var stream = File.OpenRead(filePath))
            {
                document = new Document(stream);
            }

            var tokens = GetTokens(data);
            var tokensReplaced = ReplaceTokens(data, document.Range.Text, (token, value) => ReplaceTokenInDocument(document, token, value), tokens);

            if (!tokensReplaced)
            {
                return null;
            }

            System.IO.Stream memoryStream = new MemoryStream();
            if (saveOptions != null)
            {
                document.Save(memoryStream, saveOptions);
            }
            else if(saveFormat.HasValue)
            {
                document.Save(memoryStream, saveFormat.Value);
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        protected abstract IDictionary<string, string> GetTokens(T data);

        protected virtual bool ReplaceTokens(T data, string text, Action<string, string> replacementAction, IDictionary<string, string> tokens)
        {
            TokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, tokens, true, out var errors);
            if (errors.Any())
            {
                return false;
            }

            return true;
        }

        protected virtual string GetTokenNameFor(TokenReplacementField token)
        {
            return typeof(TokenReplacementField)
                .GetMember(token.ToString())
                .First().GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        protected virtual void ReplaceTokenInDocument(Document document, string token, string value)
        {
            document.Range.Replace(token, SanitiseTokenValue(value) ?? string.Empty, new FindReplaceOptions());
        }

        private string SanitiseTokenValue(string tokenValue)
        {
            if (!string.IsNullOrEmpty(tokenValue))
            {
                // replace CRLF with something Aspose.Words understands, and remove excess whitespace and trailing commas
                var lines = tokenValue.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join("&l", lines.Select(x => x.Trim().Trim(',')));
            }
            return tokenValue;
        }
    }
}
