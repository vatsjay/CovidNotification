using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;

namespace Demo.AspNetCore.PushNotifications.Services.Abstractions
{
    public interface IPushSubscriptionStore
    {
        Task StoreSubscriptionAsync(DistrictSubscription subscription);

        Task UpdateSubscriptionAsync(DistrictSubscription subscription);

        Task DiscardSubscriptionAsync(string endpoint);

        Task ForEachSubscriptionAsync(Action<DistrictSubscription> action);

        Task ForEachSubscriptionAsync(Action<DistrictSubscription> action, CancellationToken cancellationToken);
    }
}
