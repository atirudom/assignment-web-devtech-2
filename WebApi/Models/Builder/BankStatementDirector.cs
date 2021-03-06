﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApi.Models.Builder
{
    public class BankStatementDirector
    {
        private BankStatementBuilder BankStatementBuilder;

        public BankStatementDirector(BankStatementBuilder bankStatementBuilder)
        {
            BankStatementBuilder = bankStatementBuilder;
        }

        public void ConstructStatement()
        {
            BankStatementBuilder.CreateNewBankStatement();

            // Sort transaction descending from most recent date
            BankStatementBuilder.SortTransactionsByDateDESC();
        }

        public List<Transaction> GetBankStatementTransactions()
        {
            return BankStatementBuilder.GetBankStatementTransactions();
        }

        public List<Account> GetMatchedAccounts()
        {
            return BankStatementBuilder.GetMatchedAccounts();
        }
    }
}
