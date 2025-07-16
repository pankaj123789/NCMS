using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal.Security
{
    public class PasswordGenerator
    {
        private IConfigurationService _ConfigurationService;
        public IConfigurationService ConfigurationService => _ConfigurationService ?? (_ConfigurationService = ServiceLocator.Resolve<IConfigurationService>());
        //excludes difficult-to-find or easily-confused chars
        private static char[] lowerCase = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static char[] upperCase = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static char[] numeral = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] special = new[] { '!', '?', '@', '$' };

        //This is static so passwords generated in quick succession aren't identical (in the same AppDomain, at least)
        //Assume that its pseudo-randomness is good enough, and that no attacker will have access to this code, the app 
        //start datetime, and knowledge of how many passwords have been generated.
        private static Random mRandom = new Random();

        public string GetNewEPortalPassword()
        {
            var randomOrderedCharacterSets = RandomSort(lowerCase,
                lowerCase,
                lowerCase,
                upperCase,
                upperCase,
                upperCase,
                numeral,
                special);

            return GetRandomString(randomOrderedCharacterSets);
        }

        private IEnumerable<T> RandomSort<T>(params T[] items)
        {
            
            var randomAssigned = items.Select(i => new { Item = i, Order = GetRandomNumber(1000) });
            var count = ConfigurationService.MinimumPasswordLength() - items.Length;
            var result = randomAssigned.OrderBy(ri => ri.Order).Select(ri => ri.Item).ToList();
            
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                    result.Add(result.First());
            }
            return result;
        }

        private string GetRandomString(IEnumerable<char[]> charLists)
        {
            return new string(charLists.Select(PickChar).ToArray());
        }

        private char PickChar(char[] input)
        {
            var randomIndex = GetRandomNumber(input.Length);
            return input[randomIndex];
        }

        private int GetRandomNumber(int exclusiveMax)
        {
            return mRandom.Next(0, exclusiveMax);
        }
    }
}
