using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    public static class DimensionSetMapper
    {
        public static DimensionSetLine ToDimensionSetLine(this DimensionSetType setType, string value)
        {
            return new DimensionSetLine()
            {
                Code = setType.ToString(),
                ValueCode = value
            };
        }
    }

    public enum DimensionSetType
    {
        Category,
        Activity
    }
}
