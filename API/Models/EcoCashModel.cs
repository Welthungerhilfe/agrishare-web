using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class EcoCashModel
    {
        public string ClientCorrelator { get; set; }

        public string TransactionOperationStatus { get; set; }
    }
}