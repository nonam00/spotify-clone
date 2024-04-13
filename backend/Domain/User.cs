namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string BillingAddress { get; set; }
        public string PaymentMethod { get; set; }
    }
}
