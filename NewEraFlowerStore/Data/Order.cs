// csharp file that contains data properties for an order

#region Using Directives
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="Order"/> contains data properties for an order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// ID of an order.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// ID of the user who creates the order.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)] // the length should be equal to that of the column "Id" in the Identity user table
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        /// <summary>
        /// The user's first name.
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name.
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        /// <summary>
        /// A datailed address.
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(300)] // the relevant input model needs updating after modifying the length
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }
        /// <summary>
        /// A zip/postal code.
        /// </summary>
        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip/postal code")]
        public string ZipOrPostalCode { get; set; }
        /// <summary>
        /// A phone number.
        /// </summary>
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// ID of the order status.
        /// </summary>
        public int OrderStatusId { get; set; }
        /// <summary>
        /// The postage of an order.
        /// </summary>
        [DataType(DataType.Currency)]
        public decimal Postage { get; set;}
        /// <summary>
        /// The order time.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Order time")]
        public DateTimeOffset OrderTime { get; set; }
        /// <summary>
        /// The payment time.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Payment time")]
        public DateTimeOffset? PaymentTime { get; set; }
        /// <summary>
        /// The delivery time.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Delivery time")]
        public DateTimeOffset? DeliveryTime { get; set; }
        /// <summary>
        /// The completion time.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Completion time")]
        public DateTimeOffset? CompletionTime { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    } // end class Order
} // end namespace NewEraFlowerStore.Data