namespace Titan_BugTracker.Models.ViewModels
{
    public class DonutViewModel
    {
        public string[] labels { get; set; }
        public SubData[] datasets { get; set; }
    }

    public class SubData
    {
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }
    }
}