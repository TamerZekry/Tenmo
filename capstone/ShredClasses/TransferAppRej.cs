namespace shared
{
    public class TransferAppRej
    {
        public TransferAppRej()
        {

        }
        public TransferAppRej(int trans_id, int action_id, int sender_id)
        {
            Trans_id = trans_id;
            Action_id = action_id;
            SenderId = sender_id;
        }

        public int Trans_id { get; set; }
        public int Action_id { get; set; }
        public int SenderId { get; set; }


    }
}
