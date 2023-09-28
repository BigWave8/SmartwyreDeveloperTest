namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool Success { get; set; }

    public CalculateRebateResult(bool success)
    {
        Success = success;
    }

    public CalculateRebateResult()
    {
    }
}
