namespace ShredClasses
{
    public class transfere_request
    {
        public transfere_request()
        {

        }
        public transfere_request(int sender_Id, int target_Id, decimal amount, bool isThisASend)
        {
            this.sender_Id = sender_Id;
            this.target_Id = target_Id;
            _amount = amount;
            this.IsThisASend = isThisASend;
        }

        public int sender_Id { get; set; }
        public int target_Id { get; set; }
        public decimal _amount { get; set; }

        public bool IsThisASend { get; set; }


    }
}
