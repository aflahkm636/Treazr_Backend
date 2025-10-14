//using Microsoft.Extensions.Options;
//using Razorpay.Api;
//using System.Collections.Generic;
//using Treazr_Backend.Models;

//public class RazorpayService
//{
//    private readonly RazorpaySettings _settings;

//    public RazorpayService(IOptions<RazorpaySettings> options)
//    {
//        _settings = options.Value;
//    }

//    public Order CreateOrder(int amount, string currency = "INR")
//    {
//        var client = new RazorpayClient(_settings.Key, _settings.Secret);

//        var options = new Dictionary<string, object>
//        {
//            { "amount", amount * 100 }, // Amount in paise
//            { "currency", currency },
//            { "payment_capture", 1 }
//        };

//        var order = client.Order.Create(options);
//        return order;
//    }
//}
