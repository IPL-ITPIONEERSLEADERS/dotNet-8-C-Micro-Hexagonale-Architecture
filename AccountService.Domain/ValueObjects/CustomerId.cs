using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.ValueObjects
{
    public record CustomerId(Guid Value)
    {
        public static CustomerId New() => new(Guid.NewGuid());
    }
}
