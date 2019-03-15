using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDiffEngine.Enumerations
{
    public enum DiffStatus
    {
        Matched,
        UnMatched,
        Unknown
    }

    public enum DiffCounter
    {
        Zero,
        One,
        Two,
        Many
    }
}
