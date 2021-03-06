﻿using AdminApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApi.Models.Analyzer
{
    public class TransactionAnalyzer
    {
        private IEnumerable<Transaction> _transactions;
        private List<TransactionDateAns> resultTransactionDateAns;

        private DateTime ThresholdDate = DateTime.UtcNow.AddYears(-200);

        public TransactionAnalyzer(List<Transaction> transactions)
        {
            _transactions = transactions;
            resultTransactionDateAns = new List<TransactionDateAns>();
        }

        public object GenerateResults(DateTime from, DateTime to, string analyzeType)
        {
            from = GetFromDate(from);
            to = GetToDate(from, to);

            int dayDiff = Convert.ToInt32((to - from).TotalDays);
            dayDiff += 1; // For counting its own day
            if (analyzeType == "dailyResult")
                return GenerateDailyResults(from, dayDiff);
            else
                return GenerateAmountPerType(from, to);
        }

        public DateTime GetFromDate(DateTime from)
        {
            // if from is not set -> set to first transaction
            if (from < ThresholdDate)
            {
                var orderedTrans = _transactions.OrderBy(t => t.TransactionTimeUtc);
                from = new DateTime(orderedTrans.ToList()[0].TransactionTimeUtc.Ticks);
            }
            return from;
        }

        public DateTime GetToDate(DateTime from, DateTime to)
        {
            if (to < ThresholdDate)
            {
                to = DateTime.UtcNow;
            }
            return to;
        }

        public IEnumerable<Transaction> GetTransactionsBetween(DateTime from, DateTime to)
        {
            DateTime newFrom = GetFromDate(from);
            DateTime newTo = GetToDate(from, to);
            var result = _transactions
                .Where(t => t.TransactionTimeUtc.Date >= newFrom.Date && t.TransactionTimeUtc.Date <= newTo.Date)
                .OrderByDescending(t => t.TransactionTimeUtc);

            return result;
        }

        public List<TransactionPerTypeAns> GenerateAmountPerType(DateTime from, DateTime to)
        {
            IEnumerable<Transaction> tmpTrans = GetTransactionsBetween(from, to);
            var result = new List<TransactionPerTypeAns>();
            foreach (TransactionType type in Enum.GetValues(typeof(TransactionType)))
            {
                if (type == TransactionType.BillPay) continue;
                decimal amountPerType = tmpTrans.Where(t => t.TransactionType == type).Sum(t => t.Amount);
                result.Add(new TransactionPerTypeAns()
                {
                    TotalAmount = amountPerType,
                    Type = type.ToString()
                });
            }

            return result;
        }

        public List<TransactionDateAns> GenerateDailyResults(DateTime from, int dayDiff)
        {
            var result = new List<TransactionDateAns>(new TransactionDateAns[dayDiff]);
            var date = from;   // If outside of duration = get today
            for (int i = 0; i < dayDiff; i++, date = date.AddDays(1))
            {
                IEnumerable<Transaction> tmpTrans = _transactions.Where(t => t.TransactionTimeUtc.Date == date.Date);
                decimal totalAmount = 0;
                foreach (var t in tmpTrans)
                {
                    totalAmount += t.Amount;
                }
                result[i] = new TransactionDateAns
                {
                    Date = date.ToString("dd/MM/yyyy"),
                    TransactionCount = tmpTrans.Count(),
                    TotalAmount = totalAmount
                };
            }
            return result;
        }
    }
}
