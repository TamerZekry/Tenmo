namespace TenmoClient.Models
{
    public class Transfer
    {
        public Transfer()
        {

        }
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }
}
