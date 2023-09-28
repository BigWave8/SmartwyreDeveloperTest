using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services.Strategy;
using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Services;

internal class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    readonly Dictionary<IncentiveType, IRebateCalculationStrategy> RebateCalculationByIncentive;

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        RebateCalculationByIncentive = new()
        {
            { IncentiveType.FixedCashAmount, new FixedCashAmountStrategy() },
            { IncentiveType.FixedRateRebate, new FixedRateRebateStrategy() },
            { IncentiveType.AmountPerUom,    new AmountPerUomStrategy() }
        };
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate is not null &&
           IsProductSupportRebateIncentiveType(product, rebate))
        {
            decimal rebateAmount = RebateCalculationByIncentive[rebate.Incentive].CalculateRebate(rebate, product, request);
            if (rebateAmount != 0)
            {
                _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
                return new CalculateRebateResult(true);
            }
        }
        return new CalculateRebateResult(false);
    }

    private static bool IsProductSupportRebateIncentiveType(Product product, Rebate rebate) =>
        product?.SupportedIncentives.Contains(rebate.Incentive) ?? false;
}
