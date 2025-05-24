namespace KC.Domain.Common.Finance
{
    public interface ICalcProvider
    {
        CalcResponse Calculate(CalcRequest request);
    }
}
