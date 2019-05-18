// csharp file that contains data properties for a cart detail

using System.ComponentModel.DataAnnotations;

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="CartDetail"/> contains data properties for a cart detail.
    /// </summary>
    public class CartDetail
    {
        /// <summary>
        /// ID of a cart detail.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// ID of the user who owns the cart detail.
        /// </summary>
        [Required(ErrorMessage = "Please enter a user ID.")]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = "Please enter a valid user ID.")] // the length should be equal to that of the column "Id" in the Identity user table
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        /// <summary>
        /// ID of a bouquet.
        /// </summary>
        public int BouquetId { get; set; }
        public virtual Bouquet Bouquet { get; set; }
        /// <summary>
        /// The quantity of a cart detail recorded.
        /// </summary>
        [Display(Name = "QTY")]
        public int Quantity { get; set; }
    } // end class CartDetail
} // end namespace NewEraFlowerStore.Data