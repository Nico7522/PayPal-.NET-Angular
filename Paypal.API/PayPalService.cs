using PayPal.Api;

namespace Paypal.API
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;
        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Payment CreatePayment(IEnumerable<ItemDto> items, string baseUrl)
        {
            var subtotal = 0M;
            var itemList = new ItemList
            {
                items = items.Select(x =>
                {
                    subtotal += x.Price * x.Quantity;
                    return new Item { name = x.Name, currency = "USD", price = x.Price.ToString(), quantity = x.Quantity.ToString() };
                }).ToList()
            };
            var shipping = 0M;
            var tax = 0M;
            var transactions = new List<Transaction>()
            {
                new()
                {
                description = "Shopping Cart purchase",
                item_list = itemList,
                amount = new()
                {
                    currency = "USD",
                    details = new () { 
                        shipping = shipping.ToString(),
                        tax = tax.ToString(),
                        subtotal = subtotal.ToString(),
                    },
                    total = (shipping + tax + subtotal).ToString()  
                }

                }
            };
            var payment = new Payment() { 
                intent = "sale",
                payer = new Payer { payment_method = "paypal"},
                transactions = transactions,
                redirect_urls = new()
                {
                    cancel_url = "/",
                    return_url = $"/{baseUrl}/ExecutePayment",
                }
            };

            return payment.Create(GetContext());
        }


        public Payment ExecutePayment(ExecutePaymentDto dto)
        {
            var paymentExecution = new PaymentExecution { payer_id = dto.PayerId };
            var payment = new Payment() { id = dto.PaymentId};

            return payment.Execute(GetContext(), paymentExecution);
        }

        private APIContext GetContext() => new(GetAccessToken()) {Config = GetConfig() };

        private string GetAccessToken() => new OAuthTokenCredential(GetConfig()).GetAccessToken();

        private Dictionary<string, string> GetConfig() => new()
        {
            {
                "mode", _configuration["PayPal:Mode"]
            },
             {
                "clientId", _configuration["PayPal:ClientId"]
            },
              {
                "clientSecret", _configuration["PayPal:ClientSecret"]
            }
        };
    }
}
