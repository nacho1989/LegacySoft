using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDiffEngine.Enumerations;

namespace TextDiffEngine.Abstract
{
    public interface ITextLine : IComparable
    {
        DiffCounter OC { get; set; }
        DiffCounter NC { get; set; }
        string Line { get; set; }
        int Index { get; set; }
        int? OLNO { get; set; }

        bool IsEqualTo(object obj);
    }
}
