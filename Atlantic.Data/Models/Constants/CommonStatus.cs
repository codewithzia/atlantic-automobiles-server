namespace Atlantic.Data.Models.Constants
{
    public class CommonStatus
    {
        public string Value { get; private set; }
        public CommonStatus(string value)
        {
            Value = value;
        }
        public static CommonStatus Active { get { return new CommonStatus("Active"); } }
        public static CommonStatus Inactive { get { return new CommonStatus("Inactive"); } }
        public static CommonStatus Blocked { get { return new CommonStatus("Blocked"); } }
        public static CommonStatus BlockedByFinance { get { return new CommonStatus("BlockedByFinance"); } }
        public static CommonStatus Pending { get { return new CommonStatus("Pending"); } }
        public static CommonStatus InProgress { get { return new CommonStatus("InProgress"); } }
        public static CommonStatus OnHold { get { return new CommonStatus("OnHold"); } }
        public static CommonStatus Approved { get { return new CommonStatus("Approved"); } }
        public static CommonStatus Success { get { return new CommonStatus("Success"); } }
        public static CommonStatus Rejected { get { return new CommonStatus("Rejected"); } }
        public static CommonStatus Cancelled { get { return new CommonStatus("Cancelled"); } }
    }
}
