#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class Bouquet
    {
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Bouquet", AdditionalFields = nameof(Name))]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a name.")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Please enter a valid name.")]
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Bouquet", AdditionalFields = nameof(ID))]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string PhotoUrl1 { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string PhotoUrl2 { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        [DataType(DataType.MultilineText)]
        [StringLength(800, ErrorMessage = "Please enter a valid description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a launch date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Launch date")]
        public DateTime LaunchDate { get; set; }

        public int ColourId { get; set; }

        public virtual Colour Colour { get; set; }

        public int FlowerId { get; set; }

        public virtual Flower Flower { get; set; }

        public int OccasionId { get; set; }

        public virtual Occasion Occasion { get; set; }

        [Required(ErrorMessage = "Please enter an original price.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Orig.")]
        public decimal OriginalPrice { get; set; }

        public decimal Discount { get; set; }

        public int Stocks { get; set; }

        public int Sales { get; set; }
    } // end class Bouquet
} // end namespace NewEraFlowerStore.Data