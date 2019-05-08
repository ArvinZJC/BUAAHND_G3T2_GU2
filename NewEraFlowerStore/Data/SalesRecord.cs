#region Using Directives
using System;
using System.ComponentModel.DataAnnotations;
#endregion Using Directives

namespace NewEraFlowerStore.Data
{
    public class SalesRecord
    {
        public int ID { get; set; }

        [DataType(DataType.Currency)]
        public decimal SalesAmount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset CreationTime { get; set; }
    } // end class SalesRecord
} // end namespace NewEraFlowerStore.Data