namespace NewEraFlowerStore.Extensions
{
    public static class DecimalHelper
    {
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