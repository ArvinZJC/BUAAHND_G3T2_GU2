using System.ComponentModel.DataAnnotations;

namespace NewEraFlowerStore.Data
{
    public class OrderDetail
    {
        public int ID { get; set; }

        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int BouquetId { get; set; }

        [Required(ErrorMessage = "Please enter a bouquet name.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Please enter a valid bouquet name.")] // the length should be equal to that of the column "Name" in the bouquet table
        [Display(Name = "Bouquet name")]
        public string BouquetName { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "QTY")]
        public int Quantity { get; set; }
    } // end class OrderDetail
} // end namespace NewEraFlowerStore.Data