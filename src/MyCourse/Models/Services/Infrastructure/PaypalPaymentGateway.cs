using System.Globalization;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Infrastructure;
using MyCourse.Models.InputModels.Courses;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using HttpResponse = PayPalHttp.HttpResponse;

namespace MyCourse.Models.Services.Infrastructure
{
    public class PaypalPaymentGateway : IPaymentGateway
    {
        private readonly IOptionsMonitor<PaypalOptions> options;

        public PaypalPaymentGateway(IOptionsMonitor<PaypalOptions> options)
        {
            this.options = options;
        }

        public async Task<string> GetPaymentUrlAsync(CoursePayInputModel inputModel)
        {
            OrderRequest order = new()
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = inputModel.ReturnUrl,
                    CancelUrl = inputModel.CancelUrl,
                    BrandName = options.CurrentValue.BrandName,
                    ShippingPreference = "NO_SHIPPING"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        CustomId = $"{inputModel.CourseId}/{inputModel.UserId}",
                        Description = inputModel.Description,
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = inputModel.Price.Currency.ToString(),
                            Value = inputModel.Price.Amount.ToString(CultureInfo.InvariantCulture) // 14.50
                        }
                    }
                }
            };

            PayPalEnvironment env = GetPayPalEnvironment(options.CurrentValue);
            PayPalHttpClient client = new PayPalHttpClient(env);

            OrdersCreateRequest request = new();
            request.RequestBody(order);
            request.Prefer("return=representation");

            HttpResponse response = await client.Execute(request);
            Order result = response.Result<Order>();

            LinkDescription link = result.Links.Single(link => link.Rel == "approve");
            return link.Href;
        }

        public async Task<CourseSubscribeInputModel> CapturePaymentAsync(string token)
        {
            PayPalEnvironment env = GetPayPalEnvironment(options.CurrentValue);
            PayPalHttpClient client = new PayPalHttpClient(env);

            OrdersCaptureRequest request = new(token);
            request.RequestBody(new OrderActionRequest());
            request.Prefer("return=representation");

            try
            {
                HttpResponse response = await client.Execute(request);
                Order result = response.Result<Order>();

                PurchaseUnit purchaseUnit = result.PurchaseUnits.First();
                Capture capture = purchaseUnit.Payments.Captures.First();

                // $"{inputModel.CourseId}/{inputModel.UserId}"
                string[] customIdParts = purchaseUnit.CustomId.Split('/');
                int courseId = int.Parse(customIdParts[0]);
                string userId = customIdParts[1];

                return new CourseSubscribeInputModel
                {
                    CourseId = courseId,
                    UserId = userId,
                    Paid = new(Enum.Parse<Currency>(capture.Amount.CurrencyCode), decimal.Parse(capture.Amount.Value, CultureInfo.InvariantCulture)),
                    TransactionId = capture.Id,
                    PaymentDate = DateTime.Parse(capture.CreateTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
                    PaymentType = "Paypal"
                };

            }
            catch (Exception exc)
            {
                throw new PaymentGatewayException(exc);
            }
        }

        private PayPalEnvironment GetPayPalEnvironment(PaypalOptions options)
        {
            string clientId = options.ClientId;
            string clientSecret = options.ClientSecret;

            return options.IsSandbox ? new SandboxEnvironment(clientId, clientSecret) :
                                       new LiveEnvironment(clientId, clientSecret);
        }
    }
}