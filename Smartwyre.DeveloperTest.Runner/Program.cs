using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        IRebateDataStore rebateDataStore = new RebateDataStore();
        IProductDataStore productDataStore = new ProductDataStore();

        RebateService rebateService = new (rebateDataStore, productDataStore);

        Console.WriteLine("Enter rebate identifier: ");
        string rebateIdentifier = Console.ReadLine();

        Console.WriteLine("Enter product identifier: ");
        string productIdentifier = Console.ReadLine();

        Console.WriteLine("Enter volume: ");
        bool isVolumeCorrect = decimal.TryParse(Console.ReadLine(), out decimal volume);
        if (!isVolumeCorrect)
        {
            Console.WriteLine("You entered the wrong volume");
            Console.WriteLine("Try a comma separated number");
        }

        CalculateRebateRequest request = new()
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        CalculateRebateResult result = rebateService.Calculate(request);
        Console.WriteLine("Success: " + result.Success);
    }
}
