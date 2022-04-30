namespace MyCourse.Models.ValueObjects;

public record Money
{
    public Money() : this(Currency.EUR, 0.00m)
    {
    }
    public Money(Currency currency, decimal amount)
    {
        Amount = amount;
        Currency = currency;
    }
    private decimal amount = 0;
    public decimal Amount
    {
        get
        {
            return amount;
        }
        init
        {
            if (value < 0)
            {
                throw new InvalidOperationException("The amount cannot be negative");
            }
            amount = value;
        }
    }
    public Currency Currency
    {
        get; init;
    }

    public override string ToString()
    {
        return $"{Currency} {Amount:0.00}";
    }
}
