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

        private List<DiffResult> diffResultsNA;
        private List<DiffResult> diffResultsOA;


        public void ProcessDiff(ITextFile source, ITextFile dest)
        {
            O = source;
            N = dest;

            
            table = new List<ITextLine>();
            OA = new List<ITextLine>();
            NA = new List<ITextLine>();

            diffResultsNA = new List<DiffResult>();
            diffResultsOA = new List<DiffResult>();

            int sourceLineCount = O.Count();
            int destLineCount = N.Count();           

            ScanNewFile(destLineCount);
            ScanOldFile(sourceLineCount);
            FindUnchangedLines();
            ProcessLinesAsc();
            ProcessLinesDesc();
            GenerateDiffResult();
        }     

        private void ScanNewFile(int destLineCount)
        {
            for (int i = 0; i < destLineCount; i++)
            {
                var line = N.GetByIndex(i);
                if (!IsInTable(line))
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
            for (int i = 0; i < sourceLineCount; i++)
            {
                var line = O.GetByIndex(i);
                if (!IsInTable(line))
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
            for (int i = 0; i < NA.Count; i++)
            {
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
                            if(OA.ElementAtOrDefault(olno.Value) != null)
                                OA[olno.Value].Index = i;                           
                        }
                    }
                }
            }
        }

        private void ProcessLinesAsc()
        {
            var smallestUpperBound = Math.Min(NA.Count, OA.Count);
            for (var i = 0; i < smallestUpperBound; i++)
            {
                // Do something with collection1[index] and collection2[index]
                if (NA[i].Line.Equals(OA[i].Line)
                            && (IsInTable(NA[i + 1])
                            && IsInTable(OA[i + 1])))
                {
                    var oline = ((List<ITextLine>)O.Lines).ElementAtOrDefault(i + 1);
                    if (oline != null && OA.ElementAtOrDefault(i + 1) != null)
                    {
                        var line = table.SingleOrDefault(x => x.Line.Equals(oline.Line));
                        OA[i + 1] = line;
                    }

                    var nline = ((List<ITextLine>)N.Lines).ElementAtOrDefault(i + 1);
                    if (nline != null && NA.ElementAtOrDefault(i + 1) != null)
                    {
                        var line = table.SingleOrDefault(x => x.Line.Equals(nline.Line));
                        NA[i + 1] = line;
                    }

                }
            }            
        }

        private void ProcessLinesDesc()
        {
            var smallestUpperBound = Math.Min(NA.Count, OA.Count);
            for (var i = smallestUpperBound - 1; i >=1; i--)
            {              
                    if (NA[i].Line.Equals(OA[i].Line)
                        && (IsInTable(NA[i - 1])
                        && IsInTable(OA[i - 1])))
                    {
                        var oline = ((List<ITextLine>)O.Lines).ElementAtOrDefault(i - 1);
                        if (oline != null && OA.ElementAtOrDefault(i - 1) != null)
                        {
                            var line = table.SingleOrDefault(x => x.Line.Equals(oline.Line));
                            OA[i - 1] = line;
                        }

                        var nline = ((List<ITextLine>)N.Lines).ElementAtOrDefault(i - 1);
                        if (nline != null && NA.ElementAtOrDefault(i - 1) != null)
                        {
                            var line = table.SingleOrDefault(x => x.Line.Equals(nline.Line));
                            NA[i - 1] = line;
                        }

                    }
            }
        }

        private void GenerateDiffResult()
        {
            //NA[i] = table[line] and NA[i].Olno = null then its a new Line in NA
            //if NA[i] == OA[i] but NA[i+1] != OA[i+1]  
            var size = Math.Max(NA.Count, OA.Count);
            var diffNA = new DiffResult[size];
            var diffOA = new DiffResult[size];
            int lastAddedIndex = 0;


            for (int i = 0; i < size; i++)
            {
                if (NA.ElementAtOrDefault(i) != null)
                {

                    var item = NA[i];

                    var diffO = new DiffResult
                    {
                        Line = item.Line,
                        Length = item.Line.Length,
                        NewIndex = item.Index ?? null,
                        OldIndex = item.OLNO ?? null
                    };

                    if (IsInTable(item) && item.OLNO == null)
                    {
                        diffO.Status = DiffResultStatus.Added;
                    }

                    if (item.Index != null && item.OLNO != null)
                    {
                        diffNA[item.OLNO.Value] = diffO;
                        lastAddedIndex = item.OLNO.Value;
                    }
                    else
                    {
                        lastAddedIndex = lastAddedIndex + 1;
                        diffNA[lastAddedIndex] = diffO;
                    }
                }
            }            


                diffResultsNA.AddRange(diffNA.ToList());
            //for (int i = 0; i < size; i++)
            //{
            //    if (OA.ElementAtOrDefault(i) != null)
            //    {
            //        var currentOALine = OA[i];

            //        var diffO = new DiffResult()
            //        {
            //            Line = currentOALine.Line,
            //            NewIndex = currentOALine.Index ?? null,
            //            OldIndex = currentOALine.OLNO ?? null,
            //            Length = currentOALine.Line.Length
            //        };


            //        if (IsInTable(currentOALine) && currentOALine.NC == DiffCounter.Zero)
            //        {
            //            diffO.Status = DiffResultStatus.ReMoved;
            //        }
            //        else
            //        {
            //            diffO.Status = DiffResultStatus.Unchanged;
            //        }

            //        diffOA[i] = diffO;
            //    }
            //}

            //for (int i = 0; i < size; i++)
            //{
            //    if (NA.ElementAtOrDefault(i) != null)
            //    {
            //        var currentNALine = NA[i];

            //        var diffN = new DiffResult()
            //        {
            //            Line = currentNALine.Line,
            //            NewIndex = currentNALine.Index ?? null,
            //            OldIndex = currentNALine.OLNO ?? null,
            //            Length = currentNALine.Line.Length
            //        };

            //        if (IsInTable(currentNALine) && currentNALine.OLNO == null)
            //        {
            //            diffN.Status = DiffResultStatus.Added;
            //        }
            //        else
            //        {
            //            diffN.Status = DiffResultStatus.Unchanged;
            //        }

            //        diffNA[i] = diffN;
            //    }
            //}



            //diffResultsNA.AddRange(diffNA.ToList());
            //diffResultsOA.AddRange(diffOA.ToList());
        }


        private void Replace(IList collection, int index, object obj)
        {
            collection.RemoveAt(index);
            collection.Insert(index, obj);
        }

        private bool IsInTable(ITextLine obj)
        {
            return table.Any(x => x.IsEqualTo(obj) == true);
        }
    }
}
