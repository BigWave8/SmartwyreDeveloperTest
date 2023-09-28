using System;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

internal class RebateDataStore : IRebateDataStore
{
    public Rebate GetRebate(string rebateIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return new Rebate()
        {
            Identifier = "rebate",
            Incentive = IncentiveType.FixedRateRebate,
            Amount = 404.04m,
            Percentage = 333.34m
        };
    }

    public void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        // Update account in database, code removed for brevity
        Console.WriteLine("rebateAmount: " + rebateAmount);
    }
}
