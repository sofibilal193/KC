namespace KC.Domain.Common.Application
{
    public record CustomerUnitCommand
    {
        /// <summary>
        /// Inventory stock number of unit.
        /// </summary>
        public string? StockNo { get; init; }

        /// <summary>
        /// Identification Number of unit (e.g. VIN, HIN, Serial Number, etc.).
        /// </summary>
        public string? VIN { get; init; }

        /// <summary>
        /// Year of unit.
        /// </summary>
        public short? Year { get; init; }

        /// <summary>
        /// Make of unit.
        /// </summary>
        public string? Make { get; init; } = "";

        /// <summary>
        /// Model of unit.
        /// </summary>
        public string? Model { get; init; } = "";

        /// <summary>
        /// Indicates that this is a trade in or not.
        /// </summary>
        public bool IsTradeIn { get; init; }

        /// <summary>
        /// Name of Lien holder if there is a lien on trade-in.
        /// </summary>
        public string? LienHolderName { get; init; } = "";

        /// <summary>
        /// Balance amount owed on the trade-in unit.
        /// </summary>
        public decimal? BalanceOwedAmount { get; init; }

        /// <summary>
        /// Source of the unit.
        /// </summary>
        public string? Source { get; init; } = "";
    }
}
