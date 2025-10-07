namespace Treazr_Backend.Models
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }          
        public Product Product { get; set; }       

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }   
        public bool IsMain { get; set; } = false;   
    }
}
