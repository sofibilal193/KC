namespace KC.Domain.Common.Finance
{
    public enum TermFrequency
    {
        Monthly,
        SemiMonthly,
        BiWeekly,
        Weekly,
        Annually,
        Daily
    }

    public enum CalcMethod : byte
    {
        Thirty360 = 0,
        Actual365 = 1,
    }
}
