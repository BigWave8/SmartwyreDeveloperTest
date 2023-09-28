using FluentAssertions;
using Moq;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private Mock<IRebateDataStore> rebateDataStoreMock;
    private Mock<IProductDataStore> productDataStoreMock;
    private RebateService rebateService;

    [Fact]
    public void Calculate_NullRebate_ReturnsFalse()
    {
        // Arrange
        GeneralMockCreation(null, new());
        CalculateRebateRequest request = new();

        // Act
        CalculateRebateResult result = rebateService.Calculate(request);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom)]
    public void Calculate_AllIncentiveTypeWithNullProduct_ReturnsFalse(IncentiveType incentiveType)
    {
        // Arrange
        GeneralMockCreation(new() { Incentive = incentiveType }, null);
        CalculateRebateRequest request = new();

        // Act
        CalculateRebateResult result = rebateService.Calculate(request);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(IncentiveType.FixedRateRebate, "")]
    [InlineData(IncentiveType.FixedRateRebate, "1")]
    [InlineData(IncentiveType.FixedRateRebate, "2")]
    [InlineData(IncentiveType.FixedRateRebate, "1,2")]
    [InlineData(IncentiveType.FixedCashAmount, "")]
    [InlineData(IncentiveType.FixedCashAmount, "0")]
    [InlineData(IncentiveType.FixedCashAmount, "1")]
    [InlineData(IncentiveType.FixedCashAmount, "0,1")]
    [InlineData(IncentiveType.AmountPerUom,    "")]
    [InlineData(IncentiveType.AmountPerUom,    "0")]
    [InlineData(IncentiveType.AmountPerUom,    "2")]
    [InlineData(IncentiveType.AmountPerUom,    "0,2")]
    public void Calculate_ProductNotSupportRebateIncentiveType_ReturnsFalse(IncentiveType incentiveType, string supportedIncentivesString)
    {
        // Arrange
        GeneralMockCreation(new Rebate() { Incentive = incentiveType }, 
                            new Product() { SupportedIncentives = StringToSupportedIncentives(supportedIncentivesString) });
        CalculateRebateRequest request = new();

        // Act
        CalculateRebateResult result = rebateService.Calculate(request);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount, 0, 0, 0, 0)]
    [InlineData(IncentiveType.FixedCashAmount, 0, 20, 30, 40)]
    [InlineData(IncentiveType.FixedCashAmount, 0, 20, 0, 40)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0, 0, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 20, 30, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 20, 0, 40)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 0, 30, 40)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 0, 0, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 20, 0, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0, 30, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0, 0, 40)]
    [InlineData(IncentiveType.AmountPerUom, 0, 0, 0, 0)]
    [InlineData(IncentiveType.AmountPerUom, 0, 20, 30, 0)]
    [InlineData(IncentiveType.AmountPerUom, 10, 0, 0, 0)]
    [InlineData(IncentiveType.AmountPerUom, 0, 0, 0, 40)]
    [InlineData(IncentiveType.AmountPerUom, 10, 20, 30, 0)]
    [InlineData(IncentiveType.AmountPerUom, 0, 20, 30, 40)]
    public void Calculate_ValueIsZero_ReturnsFalse(IncentiveType incentiveType, decimal amount, decimal price, decimal percentage, decimal volume)
    {
        // Arrange
        GeneralMockCreation(
            new Rebate() { Amount = amount, Percentage = percentage, Incentive = incentiveType },
            new Product() { Price = price, SupportedIncentives = new HashSet<IncentiveType>() { incentiveType } });

        CalculateRebateRequest request = new()
        {
            Volume = volume
        };

        // Act
        CalculateRebateResult result = rebateService.Calculate(request);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount, 10, 20, 30, 40)]
    [InlineData(IncentiveType.FixedCashAmount, 10, 20, 0, 40)]
    [InlineData(IncentiveType.FixedCashAmount, 10, 0, 0, 40)]
    [InlineData(IncentiveType.FixedCashAmount, 10, 0, 0, 0)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 20, 30, 40)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 20, 30, 40)]
    [InlineData(IncentiveType.FixedRateRebate, 10, 10, 100, 1000)]
    [InlineData(IncentiveType.AmountPerUom, 10, 20, 30, 40)]
    [InlineData(IncentiveType.AmountPerUom, 10, 0, 0, 40)]
    [InlineData(IncentiveType.AmountPerUom, 10, 20, 0, 40)]
    [InlineData(IncentiveType.AmountPerUom, 10, 0, 30, 40)]
    [InlineData(IncentiveType.AmountPerUom, 1, 0, 0, 1)]
    [InlineData(IncentiveType.AmountPerUom, 1000, 0, 0, 1000)]
    public void Calculate_ValidData_ReturnsTrue(IncentiveType incentiveType, decimal amount, decimal price, decimal percentage, decimal volume)
    {
        // Arrange
        GeneralMockCreation(
            new Rebate() { Amount = amount, Percentage = percentage, Incentive = incentiveType },
            new Product() { Price = price, SupportedIncentives = new HashSet<IncentiveType>() { incentiveType } });

        CalculateRebateRequest request = new()
        {
            Volume = volume
        };

        // Act
        CalculateRebateResult result = rebateService.Calculate(request);

        // Assert
        result.Success.Should().BeTrue();
    }

    #region PrivateMethods
    private void GeneralMockCreation(Rebate r, Product p)
    {
        MockDataStoreAndRebateService();
        Rebate rebate = r;
        Product product = p;
        SetupMockGetRebateAndGetProduct(rebate, product);
    }

    private void MockDataStoreAndRebateService()
    {
        rebateDataStoreMock = new();
        productDataStoreMock = new();
        rebateService = new(rebateDataStoreMock.Object, productDataStoreMock.Object);
    }

    private void SetupMockGetRebateAndGetProduct(Rebate rebate, Product product)
    {
        rebateDataStoreMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);
        productDataStoreMock.Setup(r => r.GetProduct(It.IsAny<string>())).Returns(product);
    }

    private HashSet<IncentiveType> StringToSupportedIncentives(string supportedIncentivesString) =>
        string.IsNullOrWhiteSpace(supportedIncentivesString) ?
            new HashSet<IncentiveType>() :
            new(Array.ConvertAll(supportedIncentivesString.Split(','), int.Parse).Select(n => (IncentiveType)n));
    #endregion
}
