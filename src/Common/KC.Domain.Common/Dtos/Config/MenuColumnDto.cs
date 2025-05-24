using ProtoBuf;

namespace KC.Domain.Common.Config
{
    [Serializable]
    [ProtoContract]
    public record MenuColumnDto
    {
        /// <summary>
        /// Order of menu column.
        /// </summary>
        [ProtoMember(1)]
        public byte Order { get; init; }

        /// <summary>
        /// Name of menu column.
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; init; } = "";

        /// <summary>
        /// Whether the menu column is enabled.
        /// </summary>
        [ProtoMember(3)]
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Whether the menu column is recommended.
        /// </summary>
        [ProtoMember(4)]
        public bool IsRecommended { get; init; }

        /// <summary>
        /// List of products in menu column.
        /// </summary>
        [ProtoMember(5)]
        public List<MenuColumnProductDto> Products { get; set; } = new();

        public void AddProducts(List<MenuColumnProductDto> products)
        {
            var missingProducts = products.Where(sp => !Products.Exists(dp => dp.ProviderId == sp.ProviderId
                && dp.ProgramId == sp.ProgramId && dp.ProductId == sp.ProductId))?.ToList();
            if (missingProducts?.Count > 0)
            {
                Products = Products.OrderBy(p => p.Order).ToList();
                byte order = Products.Count > 0 ? Products.Max(p => p.Order) : (byte)0;
                for (int i = 0; i < missingProducts.Count; i++)
                {
                    order++;
                    missingProducts[i].SetOrder(order);
                    Products.Add(missingProducts[i]);
                }
            }
        }
    }

    [Serializable]
    [ProtoContract]
    public record MenuColumnProductDto
    {
        /// <summary>
        /// Order of product.
        /// </summary>
        [ProtoMember(1)]
        public byte Order { get; set; }

        /// <summary>
        /// Id of OrgProviderProduct.
        /// </summary>
        [ProtoMember(2)]
        public int Id { get; init; }

        /// <summary>
        /// ProviderId of product.
        /// </summary>
        [ProtoMember(3)]
        public int? ProviderId { get; init; }

        /// <summary>
        /// Provider name of product.
        /// </summary>
        [ProtoMember(4)]
        public string ProviderName { get; private set; } = "";

        /// <summary>
        /// ProgramId of product.
        /// </summary>
        [ProtoMember(5)]
        public string? ProgramId { get; init; }

        /// <summary>
        /// Program name of product.
        /// </summary>
        [ProtoMember(6)]
        public string? ProgramName { get; init; }

        /// <summary>
        /// ProductId of product.
        /// </summary>
        [ProtoMember(7)]
        public string? ProductId { get; init; }

        /// <summary>
        /// Product name of product.
        /// </summary>
        [ProtoMember(8)]
        public string ProductName { get; private set; } = "";

        /// <summary>
        /// Whether the product is enabled.
        /// </summary>
        [ProtoMember(9)]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Description of Product.
        /// </summary>
        [ProtoMember(10)]
        public string? Description { get; set; }

        /// <summary>
        /// Url for more information on Product.
        /// </summary>
        [ProtoMember(11)]
        public string? InfoUrl { get; set; }

        /// <summary>
        /// Whether the product is rated.
        /// </summary>
        public bool IsRated
        {
            get
            {
                return !string.IsNullOrEmpty(ProgramName);
            }
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public void Set(string? desc, string? infoUrl, string providerName, string productName)
        {
            Description = desc;
            InfoUrl = infoUrl;
            ProviderName = providerName;
            ProductName = productName;
        }

        public void SetOrder(byte order)
        {
            Order = order;
        }
    }
}
