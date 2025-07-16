using System.Linq;
using System.Web.Mvc;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.Controllers
{
    public class AutocompleteController : Controller
    {
        private ILookupProvider mLookupProvider;
        private const int mMaxResults = 10;

        public AutocompleteController(ILookupProvider lookupProvider)
        {
            mLookupProvider = lookupProvider;
        }

        public JsonResult Suburbs(string term)
        {
            var termToLower = term.ToLower();
            var results = mLookupProvider.Postcodes
                .Where(p => p.DisplayText.ToLower().StartsWith(termToLower))
                .Take(mMaxResults)
                .Select(p => new { p.SamId, value = p.DisplayText, p.State, p.Suburb, p.Code })
                .ToArray();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Countries(string term)
        {
            var termToLower = term.ToLower();
            var results = mLookupProvider.Countries
                .Where(c => c.DisplayText.ToLower().StartsWith(termToLower))
                .Where(c => c.IsHomeCountry == false)
                .Take(mMaxResults)
                .Select(c => new { c.SamId, value = c.DisplayText })
                .ToArray();

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}