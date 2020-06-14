using Demo.AspNetCore.PushNotifications.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.AspNetCore.PushNotifications.Cron
{
    public class Notifier : AbstractCronService
    {
        private string url;

        public Notifier(IScheduleConfig<Notifier> scheduleConfig)
            : base(scheduleConfig.CronExpression, scheduleConfig.TimeZoneInfo)
        {
            url = scheduleConfig.HostApi;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("CronJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                HttpClient http = new HttpClient();
                string api = url;

                http.GetAsync(url);

                Console.WriteLine("Request sent");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception occured {exception.Message}");
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
