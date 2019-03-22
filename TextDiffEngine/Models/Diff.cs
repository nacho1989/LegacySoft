using TextDiffEngine.Enumerations;

namespace TextDiffEngine.Models
{
    public class Diff
    {
        public int? OldIndex { get; set; }
        public int? NewIndex { get; set; }
        public int Length { get; set; }
        public DiffResultStatus Status { get; set; }
        public string Line { get; set; }     
    }

    public class DiffResult
    {
        public Diff[] LeftSideDiff { get; set; }
        public Diff[] RightSideDiff { get; set; }
    }

}
