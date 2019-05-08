#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        public string AvatarUrl { get; set; }

        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        public string FirstName { get; set; }

        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)] // the relevant input model needs updating after modifying the length
        public string LastName { get; set; }

        [ProtectedPersonalData]
        public int? GenderId { get; set; }

        [ProtectedPersonalData]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } // DOB is the abbreviation of "Date of Birth"

        [PersonalData]
        [DataType(DataType.DateTime)]
        public DateTimeOffset RegistrationTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<AddressBook> AddressBooks { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    } // end class ApplicationUser
} // end namespace NewEraFlowerStore.Data