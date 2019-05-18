// csharp file that contains data properties for a sales record

#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    /// <summary>
    /// The class <see cref="SalesRecord"/> contains data properties for a sales record.
    /// </summary>
    public class SalesRecord
    {
        /// <summary>
        /// ID of a sales record.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The sales amount recorded in a sales record.
        /// </summary>
        [DataType(DataType.Currency)]
        public decimal SalesAmount { get; set; }
        /// <summary>
        /// The creation time of a sales record.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTimeOffset CreationTime { get; set; }
    } // end class SalesRecord
} // end namespace NewEraFlowerStore.Data