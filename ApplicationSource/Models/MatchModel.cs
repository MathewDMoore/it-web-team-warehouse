namespace ApplicationSource.Models
{
    public class MatchModel
    {
        public int DocNumber { get; set; }
        public bool IsInternal { get; set; }
        public string SerialCode { get; set; }
        public int KitId { get; set; }
        public int KitCounter { get; set; }
    }
}