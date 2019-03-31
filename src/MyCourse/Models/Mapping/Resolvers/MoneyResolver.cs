using System;
using System.Data;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class MoneyResolver : IValueResolver<DataRow, object, Money>
    {
        private readonly string prefix;
        public MoneyResolver(string prefix)
        {
            this.prefix = prefix;
        }
        public Money Resolve(DataRow source, object destination, Money destMember, ResolutionContext context)
        {
            string currencyColumnName = $"{prefix}_Currency";
            string amountColumnName = $"{prefix}_Amount";

            Currency currency = Enum.Parse<Currency>(System.Convert.ToString(source[currencyColumnName]));
            decimal amount = System.Convert.ToDecimal(source[amountColumnName]);

            return new Money(currency, amount);
        }
    }
}