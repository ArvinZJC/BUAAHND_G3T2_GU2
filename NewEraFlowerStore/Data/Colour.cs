// csharp file that contains data properties for a colour

#region Using Directives
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="Colour"/> contains data properties for a colour.
    /// </summary>
    public class Colour
    {
        /// <summary>
        /// ID of a colour.
        /// </summary>
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Colour", AdditionalFields = nameof(Name))]
        public int ID { get; set; }
        /// <summary>
        /// A colour name.
        /// </summary>
        [Required(ErrorMessage = "Please enter a name.")]
        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "Please enter a valid name.")] // the relevant regular expression and tooltip need updating after modifying the length
        /* 
         * the length here should be equal to the relevant attribute;
         * at max 25 characters long, with a digit or an uppercase letter as the 1st character;
         * only "&", "-", ".", and spaces are non-alphanumeric characters allowed
         */
        [RegularExpression(@"^[0-9A-Z][&-.\s0-9A-Za-z]{0,24}$", ErrorMessage = "Please enter a valid name.")]
        [Remote(action: "VerifyNameNotInUseAsync", controller: "Colour", AdditionalFields = nameof(ID))]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bouquet> Bouquets { get; set; }
    } // end class Colour
} // end namespace NewEraFlowerStore.Data