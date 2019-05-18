// csharp file that contains data properties for a user of the application

#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// Extending from the class <see cref="IdentityUser"/>, the class <see cref="ApplicationUser"/> contains data properties for a user of the application.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The URL of an avatar file.
        /// </summary>
        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        public string AvatarUrl { get; set; }
        /// <summary>
        /// The user's first name.
        /// </summary>
        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name.
        /// </summary>
        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        public string LastName { get; set; }
        /// <summary>
        /// ID of a gender.
        /// </summary>
        [ProtectedPersonalData]
        public int? GenderId { get; set; }
        /// <summary>
        /// The user's date of birth.
        /// </summary>
        [ProtectedPersonalData]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } // DOB is the abbreviation of "Date of Birth"
        /// <summary>
        /// The registration time.
        /// </summary>
        [PersonalData]
        [DataType(DataType.DateTime)]
        public DateTimeOffset RegistrationTime { get; set; }
        [JsonIgnore]
        public virtual ICollection<AddressBook> AddressBooks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    } // end class ApplicationUser
} // end namespace NewEraFlowerStore.Data