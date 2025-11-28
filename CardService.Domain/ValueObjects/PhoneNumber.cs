using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardService.Domain.ValueObjects
{
    public record PhoneNumber(string Value)
    {
        private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

        // ✅ Factory method (recommandé en DDD)
        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number is required.");

            value = value.Trim().Replace(" ", "");
            if (!PhoneRegex.IsMatch(value))
                throw new ArgumentException("Invalid phone number format (E.164 recommended).");

            return new PhoneNumber(value);
        }

        // Si vous voulez un constructeur public, il doit appeler this()
        // Mais en DDD, on préfère Create() pour la validation
    }
}
