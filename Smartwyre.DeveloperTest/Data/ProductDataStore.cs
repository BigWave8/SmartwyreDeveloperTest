using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Data;

internal class ProductDataStore : IProductDataStore
{
    public Product GetProduct(string productIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return new Product()
        {
            Id = 1,
            Identifier = "product",
            Price = 999.9m,
            Uom = "",
            SupportedIncentives = new HashSet<IncentiveType>() { 
                IncentiveType.FixedRateRebate,
                IncentiveType.AmountPerUom,
                IncentiveType.FixedCashAmount
            }
        };
    }
}
