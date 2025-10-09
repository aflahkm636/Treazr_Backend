namespace Treazr_Backend.Models
{
   
    public class User : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public Roles Role { get; set; } = Roles.user;
        public bool IsBlocked { get; set; } = false;

        public Cart? Cart { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public enum Roles
    {
        user,
        admin
    }
}
