using System;

namespace F1Solutions.Naati.Common.Contracts.Bl.Attributes
{
    public class LegalCharactersAttribute : Attribute
    {
        public char[] Values { get; set; }

        public LegalCharactersAttribute(params char[] values)
        {
            Values = values;
        }
    }

    public class IllegalInputCharacterException : Exception
    {
        public IllegalInputCharacterException(string message) : base(message) { }
    }
}
