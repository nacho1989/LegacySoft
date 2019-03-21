using TextDiffEngine.Enumerations;

namespace TextDiffEngine.Models
{
    public class DiffResult
    {
        public int? OldIndex { get; set; }
        public int? NewIndex { get; set; }
        public int Length { get; set; }
        public DiffResultStatus Status { get; set; }
        public string Line { get; set; }     
    }
}
