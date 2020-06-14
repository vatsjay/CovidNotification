using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.AspNetCore.PushNotifications.Services.Abstractions
{
    public class DistrictSubscription : PushSubscription
    {
        public string District { get; set; }
        public long LatestConfirmedCase { get; set; }
    }
}
