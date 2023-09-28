﻿using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Services.Strategy
{
    internal interface IRebateCalculationStrategy
    {
        decimal CalculateRebate(Rebate rebate, Product product, CalculateRebateRequest request);
    }
}
