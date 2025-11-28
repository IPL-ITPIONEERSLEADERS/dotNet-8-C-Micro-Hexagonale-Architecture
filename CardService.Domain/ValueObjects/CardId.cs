using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.ValueObjects
{
    public record CardId(Guid Value)
    {
        public static CardId New() => new(Guid.NewGuid());
    }
}
