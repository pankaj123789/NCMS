using System.Collections.Generic;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public interface ILanguageService
    {
        IList<LanguageModel> List(string request);
        LanguageModel English();
    }
}
