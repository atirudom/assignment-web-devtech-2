﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApi.Models;
using AdminApi.Models.Analyzer;
using AdminApi.Models.DataManager;
using AdminApi.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionManager _repo;

        public TransactionsController(TransactionManager repo)
        {
            _repo = repo;
        }

        // GET: api/Transactions
        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            return _repo.GetAll();
        }

        // POST: api/Transactions/GetAnsDailyData
        [HttpPost("GetAnsDailyData")]
        public List<TransactionDateAns> GetAnsDailyData([FromBody] RAnsTransactionReqFormDto bodyDto)
        {
            var allTransactions = bodyDto.CustomerID != null ?
                _repo.GetFromCustomerID((int)bodyDto.CustomerID).ToList() : _repo.GetAll().ToList();
            TransactionAnalyzer transactionAnalyzer = new TransactionAnalyzer(allTransactions);
            var result = (List<TransactionDateAns>)transactionAnalyzer.GenerateResults(bodyDto.FromDate, bodyDto.ToDate, "dailyResult");
            return result;
        }

        // POST: api/Transactions/GetAnsDailyData
        [HttpPost("GetAnsAmtType")]
        public List<TransactionPerTypeAns> GetAnsAmtType([FromBody] RAnsTransactionReqFormDto bodyDto)
        {
            var allTransactions = bodyDto.CustomerID != null ?
                _repo.GetFromCustomerID((int)bodyDto.CustomerID).ToList() : _repo.GetAll().ToList();
            TransactionAnalyzer transactionAnalyzer = new TransactionAnalyzer(allTransactions);
            var result = (List<TransactionPerTypeAns>)transactionAnalyzer.GenerateResults(bodyDto.FromDate, bodyDto.ToDate, "amountPerType");
            return result;
        }
    }
}
