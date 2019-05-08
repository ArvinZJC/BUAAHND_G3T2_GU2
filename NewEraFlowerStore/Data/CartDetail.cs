using System.ComponentModel.DataAnnotations;

namespace NewEraFlowerStore.Data
{
    public class CartDetail
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a user ID.")]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = "Please enter a valid user ID.")] // the length should be equal to that of the column "Id" in the Identity user table
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int BouquetId { get; set; }

        public virtual Bouquet Bouquet { get; set; }

        [Display(Name = "QTY")]
        public int Quantity { get; set; }
    } // end class CartDetail
} // end namespace NewEraFlowerStore.Data