using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDiffEngine.Abstract
{
    public interface ITextFile
    {
        IList Lines { get; set; }
        int Count();
        ITextLine GetByIndex(int index);
        void ReplaceLine(int index, ITextLine line);
    }
}
