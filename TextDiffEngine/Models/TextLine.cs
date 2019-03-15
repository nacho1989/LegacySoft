using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDiffEngine.Abstract;
using TextDiffEngine.Enumerations;

namespace TextDiffEngine.Models
{
    public class TextLine : ITextLine
    {
        public string Line { get; set; }
        public int Index { get; set; }
        public DiffCounter OC { get; set; }
        public DiffCounter NC { get; set; }
        public int? OLNO { get; set; }

        public TextLine(string str, int index)
        {
            Line = str.Replace("\t", "    ");
            Index = index;
        }

        public TextLine() { }
        public int CompareTo(object obj)
        {
            return Index.CompareTo(((TextLine)obj).Index);
        }

        public bool IsEqualTo(object obj)
        {
            if(Line.CompareTo(((TextLine)obj).Line) != 0)
            {
                return false;
            }
            else if (Index.CompareTo(((TextLine)obj).Index) != 0)
            {
                return false;
            }
            else if (NC.CompareTo(((TextLine)obj).NC) != 0)
            {
                return false;
            }
            else if (OC.CompareTo(((TextLine)obj).OC) != 0)
            {
                return false;
            }
            else if (OLNO.Value.CompareTo(((TextLine)obj).OLNO) != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
