using System;

namespace SolStandard.Utility.Exceptions
{
    public class VendorMisconfiguredException : Exception
    {
        public VendorMisconfiguredException(string message, int itemCount, int priceCount, int quantityCount)
        {
            throw new Exception(message + Environment.NewLine + ItemCounts(itemCount, priceCount, quantityCount));
        }

        private static string ItemCounts(int itemCount, int priceCount, int quantityCount)
        {
            return string.Format(
                "Items: {0}" + Environment.NewLine +
                "Prices: {1}" + Environment.NewLine +
                "Quantities: {2}",
                itemCount, priceCount, quantityCount
            );
        }
    }
}