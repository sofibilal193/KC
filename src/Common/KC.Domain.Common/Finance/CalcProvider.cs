using System;

namespace KC.Domain.Common.Finance
{
    public class CalcProvider : ICalcProvider
    {
        #region ICalcProvider Methods

        public CalcResponse Calculate(CalcRequest request)
        {
            CalcResponse response = new CalcResponse
            {
                FirstPaymentDate = request.ContractDate.AddDays(request.DaysToFirstPayment),
                APR = request.Rate // assuming no pre-paid finance charges
            };
            response.FinalPaymentDate = CalculateFinalPaymentDate(response.FirstPaymentDate, request.NumPayments, request.PaymentFrequency);

            if (request.CalcMethod == CalcMethod.Actual365)
            {
                response.PaymentAmount = CalculateActual365Payment(request.Rate, request.Amount,
                    request.DaysToFirstPayment, response.FirstPaymentDate, request.NumPayments, request.PaymentFrequency);
            }
            else
            {
                response.PaymentAmount = CalculateThirty360Payment(request.Rate, request.Amount,
                    request.DaysToFirstPayment, request.NumPayments, request.PaymentFrequency);
            }

            response.FinalPaymentAmount = response.PaymentAmount; // assume even payments
            response.TotalOfPaymentsAmount = FinanceUtils.BankersRounding(response.PaymentAmount * request.NumPayments, 2); // calculate total of payments amount
            response.RegZAmount = request.Amount; // set reg z amount financed
            response.TotalFinanceChargeAmount = response.TotalOfPaymentsAmount - request.Amount; // set total finance charge amount

            // Final Payment Fix for 0% deals
            if (request.Rate == 0 && response.TotalOfPaymentsAmount != request.Amount)
            {
                response.FinalPaymentAmount = response.PaymentAmount - (response.TotalOfPaymentsAmount - request.Amount);
                response.TotalOfPaymentsAmount = request.Amount;
                response.TotalFinanceChargeAmount = 0;
            }

            return response;
        }

        #endregion

        #region Private Methods

        private static DateTime CalculateFinalPaymentDate(DateTime firstPaymentDate, short numPayments, TermFrequency paymentFrequency)
        {
            DateTime finalPaymentDate = firstPaymentDate;

            // determine final payment date
            switch (paymentFrequency)
            {
                case TermFrequency.Monthly:
                    finalPaymentDate = firstPaymentDate.AddMonths(numPayments - 1);
                    break;
                case TermFrequency.SemiMonthly:
                    var days = 0;
                    int months;
                    if ((numPayments - 1) % 2 == 0)
                    {
                        months = (numPayments - 1) / 2;
                    }
                    else
                    {
                        months = (numPayments - 2) / 2;
                        days = 15;
                    }
                    finalPaymentDate = firstPaymentDate.AddMonths(months);
                    if (days > 0)
                        finalPaymentDate = finalPaymentDate.AddDays(days);
                    break;
                case TermFrequency.BiWeekly:
                    finalPaymentDate = firstPaymentDate.AddDays((numPayments - 1) * 14);
                    break;
                case TermFrequency.Weekly:
                    finalPaymentDate = firstPaymentDate.AddDays((numPayments - 1) * 7);
                    break;
                case TermFrequency.Annually:
                    finalPaymentDate = firstPaymentDate.AddYears(numPayments - 1);
                    break;
                case TermFrequency.Daily:
                    finalPaymentDate = firstPaymentDate.AddDays(numPayments - 1);
                    break;
            }

            return finalPaymentDate;
        }

        private static decimal CalculateThirty360Payment(decimal rate, decimal amount,
            short daysToFirstPayment, short numPayments, TermFrequency paymentFrequency)
        {
            decimal retval;
            if (numPayments <= 0)
            {
                retval = 0;
            }
            else
            {
                if (rate == 0)
                {
                    retval = FinanceUtils.BankersRounding(Convert.ToDecimal(amount / numPayments), 2);
                }
                else
                {
                    var period = 12;
                    var initTerm = 30;

                    // determine period based on payment frequency
                    switch (paymentFrequency)
                    {
                        case TermFrequency.Monthly:
                            period = 12;
                            initTerm = 30;
                            break;
                        case TermFrequency.SemiMonthly:
                            period = 24;
                            initTerm = 15;
                            break;
                        case TermFrequency.BiWeekly:
                            period = 26;
                            initTerm = 14;
                            break;
                        case TermFrequency.Weekly:
                            period = 52;
                            initTerm = 7;
                            break;
                        case TermFrequency.Annually:
                            period = 1;
                            initTerm = 365;
                            break;
                        case TermFrequency.Daily:
                            period = 365;
                            initTerm = 1;
                            break;
                    }

                    var pir = Convert.ToDecimal(rate / period / 100); //  periodic interest rate
                    var ain = Convert.ToDecimal((1 - (1 / Math.Pow(Convert.ToDouble(1 + pir), numPayments)))
                        / Convert.ToDouble(pir) / (1 + (Convert.ToDouble(pir * (daysToFirstPayment - initTerm)) / initTerm))); //  annuity factor

                    retval = FinanceUtils.BankersRounding(amount / ain, 2);

                    if (Convert.ToString(retval) == "NaN")
                    {
                        retval = 0;
                    }
                }
            }
            return retval;
        }

        private static decimal CalculateActual365Payment(decimal rate, decimal amount, short daysToFirstPayment,
            DateTime firstPaymentDate, short numPayments, TermFrequency paymentFrequency)
        {
            decimal retval;
            if (numPayments <= 0)
            {
                retval = 0;
            }
            else
            {
                if (rate == 0)
                {
                    retval = FinanceUtils.BankersRounding(amount / numPayments, 2);
                }
                else
                {
                    const int daysYear = 365;

                    var dr = rate / daysYear / 100; //  daily interest rate
                    var z1 = 1m;
                    var w = 0m;  // ODS (John Fox) method annuity factor

                    // interest charged days starts out as days to 1st pmt
                    int intChargedDays = daysToFirstPayment;

                    // pmtDate and nextPmtDate only used for monthly & semi-monthly schedules
                    DateTime pmtDate = firstPaymentDate;
                    for (int currPeriod = 1; currPeriod <= numPayments; currPeriod++)
                    {
                        // Include this period's interest charged days into annuity factor
                        z1 = (1 + (intChargedDays * dr)) * z1;
                        w = (1 / z1) + w;

                        DateTime nextPmtDate;
                        // get # of interest charged days in next period
                        switch (paymentFrequency)
                        {
                            case TermFrequency.Monthly:
                                nextPmtDate = pmtDate.AddMonths(1);
                                intChargedDays = (nextPmtDate - pmtDate).Days;
                                pmtDate = nextPmtDate;
                                break;

                            case TermFrequency.SemiMonthly:
                                // Odd periods are 15 days long, even periods are a month (ie: actual days)
                                if ((currPeriod % 2) == 1)
                                {
                                    intChargedDays = 15;
                                }
                                else
                                {
                                    nextPmtDate = pmtDate.AddMonths(1);
                                    intChargedDays = (nextPmtDate - pmtDate).Days;
                                    pmtDate = nextPmtDate;
                                }
                                break;

                            case TermFrequency.BiWeekly:
                                intChargedDays = 14;
                                break;

                            case TermFrequency.Weekly:
                                intChargedDays = 7;
                                break;
                            case TermFrequency.Annually:
                                intChargedDays = 365;
                                break;
                            case TermFrequency.Daily:
                                intChargedDays = 1;
                                break;
                        }
                    }

                    // Payment is Amount Financed / Annuity Factor
                    retval = FinanceUtils.BankersRounding(amount / w, 2);

                    if (Convert.ToString(retval) == "NaN")
                    {
                        retval = 0;
                    }
                }
            }
            return retval;
        }

        #endregion
    }
}
