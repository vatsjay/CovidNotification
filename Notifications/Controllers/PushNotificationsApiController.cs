using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Model;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using Microsoft.Extensions.Options;
using Demo.AspNetCore.PushNotifications.Helper;

namespace Demo.AspNetCore.PushNotifications.Controllers
{
    [Route("push-notifications-api")]
    public class PushNotificationsApiController : Controller
    {
        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly Miscellenous _miscellenous;

        public PushNotificationsApiController(IPushSubscriptionStore subscriptionStore, IPushNotificationService notificationService, IPushNotificationsQueue pushNotificationsQueue, IOptions<Miscellenous> misc)
        {
            _subscriptionStore = subscriptionStore;
            _notificationService = notificationService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _miscellenous = misc.Value;
        }

        // GET push-notifications-api/public-key
        [HttpGet("public-key")]
        public ContentResult GetPublicKey()
        {
            return Content(_notificationService.PublicKey, "text/plain");
        }

        // POST push-notifications-api/subscriptions
        [HttpPost("subscriptions")]
        public async Task<IActionResult> StoreSubscription([FromBody]DistrictSubscription subscription)
        {
            try {
            
#if DEBUG
                subscription.LatestConfirmedCase = 0;
#else
                subscription.LatestConfirmedCase = CovidHelper.Instance.GetLatestCountForDistrict(subscription.District, _miscellenous.DistrictCovidApi);
#endif
                await _subscriptionStore.StoreSubscriptionAsync(subscription);
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }

            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DiscardSubscription(string endpoint)
        {
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);

            return NoContent();
        }

        // POST push-notifications-api/notifications
        [HttpPost("notifications")]
        public IActionResult SendNotification([FromBody]PushMessageViewModel message)
        {
            _pushNotificationsQueue.Enqueue(new PushMessage(message.Notification)
            {
                Topic = message.Topic,
                Urgency = message.Urgency
            });

            return NoContent();
        }

        // GET push-notifications-api/notify-subscriber
        [HttpGet("notify-subscriber")]
        public IActionResult NotifySubscriber()
        {
            CovidHelper.Instance.NotifySubscribers(_pushNotificationsQueue, _subscriptionStore, _miscellenous.DistrictCovidApi);
            return NoContent();
        }
    }
}
