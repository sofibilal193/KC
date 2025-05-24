using System;

namespace KC.Domain.Common.Finance
{
    public static class FinanceUtils
    {
        /// <summary>
        /// Rounds a decimal using banker's rounding
        /// </summary>
        /// <param name="value">Value to round.</param>
        /// <param name="decimalPlaces">Number of decimal places to round.</param>
        /// <returns>Value rounded using banker's rounding</returns>
        public static decimal BankersRounding(decimal value, int decimalPlaces = 2)
        {
            var nFactor = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Convert.ToDecimal(Math.Floor((value * nFactor) + Convert.ToDecimal(0.5)) / nFactor);
        }

        /// <summary>
        /// Rounds a decimal using banker's rounding
        /// </summary>
        /// <param name="value">Value to round.</param>
        /// <param name="decimalPlaces">Number of decimal places to round.</param>
        /// <returns>Value rounded using banker's rounding</returns>
        public static decimal? BankersRounding(decimal? value, int decimalPlaces = 2)
        {
            if (value is null)
            {
                return null;
            }
            return BankersRounding(value.Value, decimalPlaces);
        }
    }
}
