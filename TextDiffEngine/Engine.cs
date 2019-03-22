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

        private Hashtable table;

        private List<Diff> diffResultsNA;
        private List<Diff> diffResultsOA;

        public Engine()
        {
            table = new Hashtable();
            OA = new List<ITextLine>();
            NA = new List<ITextLine>();

            diffResultsNA = new List<Diff>();
            diffResultsOA = new List<Diff>();
        }


        public DiffResult ProcessDiff(ITextFile source, ITextFile dest)
        {
            O = source;
            N = dest;
            
            return GenerateDiffResult();
        }   
        
        private DiffResult ProcessDiff()
        {
            for (int i = 0; i < N.Count(); i++)
            {
                var line = N.GetByIndex(i);
                if (!IsInTable(line))
                {
                    line.NC = DiffCounter.One;
                    table.Add(line.Line, line);
                }
                else
                {
                    line = (ITextLine)table[line.Line];
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

            for (int i = 0; i < O.Count(); i++)
            {
                var line = O.GetByIndex(i);
                if (!IsInTable(line))
                {
                    line.OC = DiffCounter.One;
                    table.Add(line.Line, line);
                }
                else
                {
                    line = (ITextLine)table[line.Line];
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

            return GenerateDiffResult();
        }

        private DiffResult GenerateDiffResult()
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
                            if (OA.ElementAtOrDefault(olno.Value) != null)
                                OA[olno.Value].Index = i;
                        }
                    }
                }
            }


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
                        var line = (ITextLine)table[oline.Line];
                        OA[i + 1] = line;
                    }

                    var nline = ((List<ITextLine>)N.Lines).ElementAtOrDefault(i + 1);
                    if (nline != null && NA.ElementAtOrDefault(i + 1) != null)
                    {
                        var line = (ITextLine)table[nline.Line];
                        NA[i + 1] = line;
                    }

                }
            }

            for (var i = smallestUpperBound - 1; i >= 1; i--)
            {
                if (NA[i].Line.Equals(OA[i].Line)
                    && (IsInTable(NA[i - 1])
                    && IsInTable(OA[i - 1])))
                {
                    var oline = ((List<ITextLine>)O.Lines).ElementAtOrDefault(i - 1);
                    if (oline != null && OA.ElementAtOrDefault(i - 1) != null)
                    {
                        var line = (ITextLine)table[oline.Line];
                        OA[i - 1] = line;
                    }

                    var nline = ((List<ITextLine>)N.Lines).ElementAtOrDefault(i - 1);
                    if (nline != null && NA.ElementAtOrDefault(i - 1) != null)
                    {
                        var line = (ITextLine)table[nline.Line];
                        NA[i - 1] = line;
                    }

                }
            }
            //NA[i] = table[line] and NA[i].Olno = null then its a new Line in NA
            //if NA[i] == OA[i] but NA[i+1] != OA[i+1]  
            var diffNA = new Diff[table.Count];
            var diffOA = new Diff[table.Count];

            return null;
        }
        

        private bool IsInTable(ITextLine obj)
        {
            var item = (ITextLine)table[obj.Line];

            if (item != null)
            {
                return item.IsEqualTo(obj);
            }
            return false;
        }
    }
}
