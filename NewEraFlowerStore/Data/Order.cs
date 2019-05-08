#region Using Directives
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)] // the length should be equal to that of the column "Id" in the Identity user table
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(300)] // the relevant input model needs updating after modifying the length
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip/postal code")]
        public string ZipOrPostalCode { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public int OrderStatusId { get; set; }

        [DataType(DataType.Currency)]
        public decimal Postage { get; set;}
        
        [DataType(DataType.DateTime)]
        [Display(Name = "Order time")]
        public DateTimeOffset OrderTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Payment time")]
        public DateTimeOffset? PaymentTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Delivery time")]
        public DateTimeOffset? DeliveryTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Completion time")]
        public DateTimeOffset? CompletionTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    } // end class Order
} // end namespace NewEraFlowerStore.Data