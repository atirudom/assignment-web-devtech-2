﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3,
        ServiceCharge = 4,
        BillPay = 5
    }

    public class Transaction
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Range(0, 9999)]
        public int TransactionID { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Range(0,9999), Required]
        public int AccountNumber { get; set; }

        [ForeignKey("AccountNumber")]
        [InverseProperty("Transactions")]
        public virtual Account Account { get; set; }

        [Range(0, 9999)]
        public int? DestinationAccountNumber { get; set; }

        [ForeignKey("DestinationAccountNumber")]
        [InverseProperty("ReceivingTransactions")]
        public virtual Account DestinationAccount { get; set; }

        [Column(TypeName = "money"), Range(0, 99999999)]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        public DateTime TransactionTimeUtc { get; set; }
    }
}