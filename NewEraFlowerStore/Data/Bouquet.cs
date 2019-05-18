// csharp file that contains data properties for a bouquet

#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="Bouquet"/> contains data properties for a bouquet.
    /// </summary>
    public class Bouquet
    {
        /// <summary>
        /// ID of a bouquet.
        /// </summary>
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Bouquet", AdditionalFields = nameof(Name))]
        public int ID { get; set; }
        /// <summary>
        /// A bouquet name.
        /// </summary>
        [Required(ErrorMessage = "Please enter a name.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Please enter a valid name.")]
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Bouquet", AdditionalFields = nameof(ID))]
        public string Name { get; set; }
        /// <summary>
        /// The URL of Bouquet Photo 1.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        public string PhotoUrl1 { get; set; }
        /// <summary>
        /// The URL of Bouquet Photo 2.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        public string PhotoUrl2 { get; set; }
        /// <summary>
        /// A bouquet description.
        /// </summary>
        [Required(ErrorMessage = "Please enter a description.")]
        [DataType(DataType.MultilineText)]
        [StringLength(800, ErrorMessage = "Please enter a valid description.")]
        public string Description { get; set; }
        /// <summary>
        /// A launch date.
        /// </summary>
        [Required(ErrorMessage = "Please select a launch date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Launch date")]
        public DateTime LaunchDate { get; set; }
        /// <summary>
        /// ID of a colour.
        /// </summary>
        public int ColourId { get; set; }
        public virtual Colour Colour { get; set; }
        /// <summary>
        /// ID of a flower.
        /// </summary>
        public int FlowerId { get; set; }
        public virtual Flower Flower { get; set; }
        /// <summary>
        /// ID of an occasion.
        /// </summary>
        public int OccasionId { get; set; }
        public virtual Occasion Occasion { get; set; }
        /// <summary>
        /// An original price of a bouquet.
        /// </summary>
        [Required(ErrorMessage = "Please enter an original price.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Orig.")]
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// A discount of a bouquet.
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// Stocks of a bouquet.
        /// </summary>
        public int Stocks { get; set; }
        /// <summary>
        /// Sales of a bouquet.
        /// </summary>
        public int Sales { get; set; }
    } // end class Bouquet
} // end namespace NewEraFlowerStore.Data