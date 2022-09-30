using Strings.ResourceGenerator.Generators.Data;
using System.Collections.Generic;

namespace Strings.ResourceGenerator.Helpers
{
    internal class ParameterComparer : IEqualityComparer<CapturedData>
    {
        public ParameterComparer()
        {
        }

        public bool Equals(CapturedData x, CapturedData y)
        {
            return x.PrefixedName == y.PrefixedName;
        }

        public int GetHashCode(CapturedData obj)
        {
            return obj.PrefixedName.GetHashCode();
        }
    }
}
