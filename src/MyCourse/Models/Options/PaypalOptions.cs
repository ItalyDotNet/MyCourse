namespace MyCourse.Models.Options
{
    public class PaypalOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsSandbox { get; set; }
        public string BrandName { get; set; }
    }
}
