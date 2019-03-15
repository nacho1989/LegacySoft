using System;
using System.Collections;
using System.IO;
using TextDiffEngine.Abstract;
using IComparable = TextDiffEngine.Abstract.IComparable;

namespace TextDiffEngine.Models
{
    public class TextFile : ITextFile
    {
        private string _text = string.Empty;
        public IList Lines { get; set; }

        public TextFile(string text)
        {
            _text = text;
            Lines = new ArrayList();
            using (StringReader sr = new StringReader(text))
            {
                String line;
                int index = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    Lines.Add(new TextLine(line, index));
                    index++;
                }
            }

        }

        public int Count()
        {
            return Lines.Count;
        }

        public ITextLine GetByIndex(int index)
        {
            return (TextLine)Lines[index];
        }

        public void ReplaceLine(int index, ITextLine line)
        { 
            Lines.RemoveAt(index);
            Lines.Insert(index, line);
        }
    }
}
