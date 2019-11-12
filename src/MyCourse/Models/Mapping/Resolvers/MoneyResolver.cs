using System;
using System.Data;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class MoneyResolver : IMemberValueResolver<DataRow, object, string, Money>
    {
        public Money Resolve(DataRow source, object destination, string sourceMember, Money destMember, ResolutionContext context)
        {
            string currencyColumnName = $"{sourceMember}_Currency";
            string amountColumnName = $"{sourceMember}_Amount";
            Currency currency = Enum.Parse<Currency>(System.Convert.ToString(source[currencyColumnName]));
            decimal amount = System.Convert.ToDecimal(source[amountColumnName]);

            return new Money(currency, amount);
        }

        private static Lazy<MoneyResolver> instance = new Lazy<MoneyResolver>(() => new MoneyResolver());
        public static MoneyResolver Instance => instance.Value;
    }
}