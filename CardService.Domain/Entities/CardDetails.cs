using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.Entities
{
    public class CardDetails
    {
        public string CardNumber { get; }
        public DateOnly ExpiryDate { get; }
        public string Cvv { get; }

        public CardDetails(string cardNumber, DateOnly expiryDate, string cvv)
        {
            ValidateCardNumber(cardNumber);
            ValidateCvv(cvv);
            if (expiryDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Expiry date must be in the future.");

            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            Cvv = cvv;
        }

        public bool IsExpired() => ExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow);

        private static void ValidateCardNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number) || number.Length < 13 || number.Length > 19)
                throw new ArgumentException("Invalid card number length.");
        }

        private static void ValidateCvv(string cvv)
        {
            if (string.IsNullOrWhiteSpace(cvv) || cvv.Length is not (3 or 4))
                throw new ArgumentException("Invalid CVV format.");
        }
    }
}
