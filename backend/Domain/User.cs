using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        [Column(TypeName = "jsonb")]
        public string BillingAddress { get; set; }
        [Column(TypeName = "jsonb")]
        public string PaymentMethod { get; set; }
        public IList<LikedSong> LikedSongs { get; } = new List<LikedSong>();
    }
}
