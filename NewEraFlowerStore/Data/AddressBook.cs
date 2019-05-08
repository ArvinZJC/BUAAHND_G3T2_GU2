#region Using Directives
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class AddressBook
    {
        [Remote(action: "VerifyAddressBookNameAsync", controller: "ApplicationUser", AdditionalFields = nameof(BookName))]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter an address book name.")]
        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "Please enter a valid address book name.")] // the relevant tooltip needs updating after modifying the length
        [Remote(action: "VerifyAddressBookNameAsync", controller: "ApplicationUser", AdditionalFields = nameof(ID))]
        [Display(Name = "Address book name")]
        public string BookName { get; set; }

        [Required(ErrorMessage = "Please enter a user ID.")]
        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = "Please enter a valid user ID.")] // the length should be equal to that of the column "Id" in the Identity user table
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Please enter a first name.")]
        [DataType(DataType.Text)]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid first name.")] // the relevant regular expression and tooltips need updating after modifying the length
        /* 
         * the minimum and maximum length here should be equal to the relevant attributes;
         * at least 2 and at max 25 letters long, with only the 1st letter uppercase
         */
        [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid first name.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        [DataType(DataType.Text)]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "Please enter a valid last name.")] // the relevant regular expression and tooltips need updating after modifying the length
        /* 
         * the minimum and maximum length here should be equal to the relevant attributes;
         * at least 2 and at max 25 letters long, with only the 1st letter uppercase
         */
        [RegularExpression(@"^[A-Z][a-z]{1,24}$", ErrorMessage = "Please enter a valid last name.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a detailed address.")]
        [DataType(DataType.Text)]
        [StringLength(300, ErrorMessage = "Please enter a valid detailed address.")] // the relevant regular expression and tooltips need updating after modifying the length
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }

        [Required(ErrorMessage = "Please enter a zip/postal code.")]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid zip/postal code.")]
        [Display(Name = "Zip/postal code")]
        public string ZipOrPostalCode { get; set; }

        [Required(ErrorMessage = "Please enter a phone number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    } // end class AddressBook
} // end namespace NewEraFlowerStore.Data