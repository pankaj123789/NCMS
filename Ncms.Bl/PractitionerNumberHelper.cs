using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;

namespace Ncms.Bl
{
    public class PractitionerNumberHelper
    {
        public PractitionerNumberHelper(ISystemService systemService, IPersonQueryService personQueryService)
        {
            _systemService = systemService;
            _personQueryService = personQueryService;
        }

        private readonly ISystemService _systemService;
        private readonly IPersonQueryService _personQueryService;
        private readonly Random _random = new Random();

        private List<string> _blackList;

        private IList<string> BlackList
        {
            get
            {
                if (_blackList == null)
                {
                    _blackList = new List<string>();
                    var blacklist = _systemService.GetSystemValue("PractitionerNumberBlackList");
                    if (!String.IsNullOrEmpty(blacklist))
                    {
                        _blackList.AddRange(blacklist.ToUpper().Split(','));
                    }
                }
                return _blackList;
            }
        }

        private bool ValidateNewPractionerNumber(string newPn)
        {
            bool valid = !BlackList.Any(newPn.Contains)
                         && _personQueryService.TestPractitionerNumberUniqueness(newPn);

            return valid;
        }

        private string GeneratePractitionerNumber()
        {
            // UC-Backoffice-5021 BR5

            // fixed part
            const string prefix = "CPN";
            // numeric parts
            var number1 = _random.Next(0, 10);
            var number2 = _random.Next(0, 100);
            // alphabetic parts
            var char1 = (char)_random.Next(65, 91);
            var char2 = (char)_random.Next(65, 91);
            var char3 = (char)_random.Next(65, 91);

            return $"{prefix}{number1}{char1}{char2}{number2:D2}{char3}";
        }

        public string GetNewPractitionerNumber()
        {
            while (true)
            {
                var newPn = GeneratePractitionerNumber();

                if (ValidateNewPractionerNumber(newPn))
                {
                    return newPn;
                }
            }
        }
    }
}