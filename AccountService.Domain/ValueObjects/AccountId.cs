using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.ValueObjects
{
    public record AccountId(Guid Value)
    {
        public static AccountId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
