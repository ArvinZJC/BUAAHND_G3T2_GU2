// csharp file that performs as a decimal helper

namespace NewEraFlowerStore.Extensions
{
    /// <summary>
    /// The class <see cref="DecimalHelper"/> formats a specified decimal number.
    /// </summary>
    public static class DecimalHelper
    {
        /// <summary>
        /// Format a specified decimal number to satisfy the price format.
        /// </summary>
        /// <param name="decimalNumber">a specified decimal number to format</param>
        /// <returns>a decimal in the price format</returns>
        public static decimal ToPriceFormat(decimal decimalNumber)
        {
            var decimalNumberString = decimalNumber.ToString();
            var decimalPointIndex = decimalNumberString.IndexOf(".");

            if (decimalPointIndex == -1 || decimalNumberString.Length < decimalPointIndex + 2 + 1)
                decimalNumberString = string.Format("{0:F" + 2 + "}", decimalNumber); // force to keep 2 decimal places
            else
                decimalNumberString = decimalNumberString.Substring(0, decimalPointIndex + 2 + 1); // truncate the decimal to keep 2 decimal places

            return decimal.Parse(decimalNumberString);
        } // end method ToPriceFormat
    } // end static class DecimalHelper
} // end namespace NewEraFlowerStore.Extensions