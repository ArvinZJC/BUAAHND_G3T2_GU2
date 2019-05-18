// csharp file that contains data properties for an order detail

using System.ComponentModel.DataAnnotations;

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="OrderDetail"/> contains data properties for an order detail.
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// ID of an order detail.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// ID of the order related to an order detail.
        /// </summary>
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// ID of a specified bouquet.
        /// </summary>
        public int BouquetId { get; set; }
        /// <summary>
        /// A name of a specified bouquet.
        /// </summary>
        [Required(ErrorMessage = "Please enter a bouquet name.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Please enter a valid bouquet name.")] // the length should be equal to that of the column "Name" in the bouquet table
        [Display(Name = "Bouquet name")]
        public string BouquetName { get; set; }
        /// <summary>
        /// A price recorded in an order detail.
        /// </summary>
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity recorded in an order detail.
        /// </summary>
        [Display(Name = "QTY")]
        public int Quantity { get; set; }
    } // end class OrderDetail
} // end namespace NewEraFlowerStore.Data