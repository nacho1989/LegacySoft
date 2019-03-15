using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextDiffEngine.Abstract;
using TextDiffEngine.Enumerations;
using TextDiffEngine.Models;

namespace TextDiffEngine
{
    public class Engine
    {
        private ITextFile O;
        private ITextFile N;

        private List<ITextLine> OA;
        private List<ITextLine> NA;

        private List<ITextLine> table;

        public void ProcessDiff(ITextFile source, ITextFile dest)
        {
            O = source;
            N = dest;

            
            table = new List<ITextLine>();
            OA = new List<ITextLine>();
            NA = new List<ITextLine>();

            int sourceLineCount = O.Count();
            int destLineCount = N.Count();           

            ScanNewFile(destLineCount);
            ScanOldFile(sourceLineCount);
            FindUnchangedLines();
            ProcessLinesAsc();
            //ProcessLinesDesc(source_line_count, dest_line_count);
        }     

        private void ScanNewFile(int destLineCount)
        {
            for (int i = 0; i < destLineCount - 1; i++)
            {
                var line = N.GetByIndex(i);
                if (!table.Any(x => x.Line.Equals(line.Line)))
                {
                    line.NC = DiffCounter.One;
                    table.Add(line);
                }
                else
                {
                    line = table.SingleOrDefault(x => x.Line.Equals(line.Line));
                    if (line == null) continue;

                    if (line.NC == DiffCounter.One)
                    {
                        line.NC = DiffCounter.Two;
                    }
                    else
                    {
                        line.NC = DiffCounter.Many;
                    }
                }
                NA.Insert(i, line);
            }
        }

        private void ScanOldFile(int sourceLineCount)
        {
            for (int i = 0; i < sourceLineCount - 1; i++)
            {
                var line = O.GetByIndex(i);
                if (!table.Any(x=> x.Line.Equals(line.Line)))
                {
                    line.OC = DiffCounter.One;
                    table.Add(line);
                }
                else
                {
                    line = table.SingleOrDefault(x => x.Line.Equals(line.Line));
                    if (line == null) continue;

                    if (line.OC == DiffCounter.Zero)
                    {
                        line.OC = DiffCounter.One;
                    }
                    else if (line.OC == DiffCounter.One)
                    {
                        line.OC = DiffCounter.Two;
                    }
                    else
                    {
                        line.OC = DiffCounter.Many;
                    }
                }
                line.OLNO = i;

                OA.Insert(i, line);
            }
        }

        private void FindUnchangedLines()
        {
            for (int i = 0; i < NA.Count - 1; i++)
            {
                var na = NA[i];
                var oa = OA[i];

                if (NA[i].OC != DiffCounter.Zero
                    && NA[i].NC != DiffCounter.Zero)
                {
                    if (NA[i].OC == NA[i].NC
                        && NA[i].OC == DiffCounter.One 
                        && NA[i].NC == DiffCounter.One)
                    {
                        var olno = NA[i].OLNO ?? null;

                        if (olno != null)
                        {
                            NA[i].Index = olno.Value;
                            OA[olno.Value].Index = i;

                            //NA.Insert(olno.Value, line);
                            //NA.RemoveAt(i);

                            //OA.RemoveAt(olno.Value);
                            //OA.Insert(i, line);
                        }
                    }
                }
            }
        }

        private void ProcessLinesAsc()
        {
            for (int i = 0; i < NA.Count - 1; i++)
            {
                var currentLine = NA[i];
                var nextLine = NA[i + 1];

                for (int j = 0; j < OA.Count - 1; j++)
                {
                    if (NA[i].Line.Equals(OA[j].Line) 
                        && (table.Any(x=> x.Line == NA[i + 1].Line)
                        && table.Any(y=> y.Line == OA[j + 1].Line)))
                    {
                        var line = table.SingleOrDefault(x => x.Line.Equals(((TextLine)O.Lines[i + 1])?.Line));
                        OA[j + 1] = line;

                        var line2 = table.SingleOrDefault(x => x.Line.Equals(((TextLine)N.Lines[j + 1])?.Line));
                        NA[i + 1] = line2;
                    }
                }
            }
        }

        //private void ProcessLinesDesc(int source_line_count, int dest_line_count)
        //{
        //    for (int i = dest_line_count - 1; i >= 1; i--)
        //    {
        //        var destline = _destination.GetByIndex(i);
        //        var nextdestline = _destination.GetByIndex(i - 1);

        //        for (int j = source_line_count - 1; j >=1; j--)
        //        {
        //            var sourceline = _source.GetByIndex(i);
        //            var nextSourceline = _source.GetByIndex(i - 1);

        //            if (destline == sourceline && nextdestline == nextSourceline)
        //            {
        //                var source_table_ref = _matchlist[nextSourceline.Line] ?? null;
        //                var dest_table_ref = _matchlist[nextdestline.Line] ?? null;
        //                _source.ReplaceLine(i - 1, source_table_ref);
        //                _destination.ReplaceLine(i - 1, dest_table_ref);
        //            }
        //        }
        //    }
        //}

        //private void GenerateDiffResult(int dest_line_count)
        //{
        //    //NA points to an item in matchlist where line number in O is 0 then the line is new
        //    for (int i = 0; i < dest_line_count - 1; i++)
        //    {
        //        var line = _destination.GetByIndex(i);

        //        if (_matchlist.ContainsKey(line.Line) && line.line_number_in_o == null)
        //        {


        //        }
        //    }
        //}

        private void Replace(IList collection, int index, object obj)
        {
            collection.RemoveAt(index);
            collection.Insert(index, obj);
        }
    }
}
