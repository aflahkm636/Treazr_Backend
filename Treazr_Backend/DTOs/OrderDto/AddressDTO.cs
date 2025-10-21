namespace Treazr_Backend.DTOs.OrderDto
{
    public class AddressDTO

    {
        public int Id { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Zip { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool IsDefaultBilling { get; set; } = false;
    }
}
