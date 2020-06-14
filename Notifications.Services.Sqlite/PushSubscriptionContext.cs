using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPush = Lib.Net.Http.WebPush;

namespace Demo.AspNetCore.PushNotifications.Services.Sqlite
{
    internal class PushSubscriptionContext : DbContext
    {
        public class PushSubscription : DistrictSubscription
        {
            public string P256DH
            {
                get { return GetKey(WebPush.PushEncryptionKeyName.P256DH); }

                set { SetKey(WebPush.PushEncryptionKeyName.P256DH, value); }
            }

            public string Auth
            {
                get { return GetKey(WebPush.PushEncryptionKeyName.Auth); }

                set { SetKey(WebPush.PushEncryptionKeyName.Auth, value); }
            }

            //public string District { get; set; }

            public PushSubscription()
            { }

            public PushSubscription(DistrictSubscription subscription)
            {
                try
                {
                    Endpoint = subscription.Endpoint;
                    Keys = subscription.Keys;
                    District = subscription.District;
                    LatestConfirmedCase = subscription.LatestConfirmedCase;
                }
                catch (System.Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                    throw;
                }
            }
        }

        public DbSet<PushSubscription> Subscriptions { get; set; }

        public PushSubscriptionContext(DbContextOptions<PushSubscriptionContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                EntityTypeBuilder<PushSubscription> pushSubscriptionEntityTypeBuilder = modelBuilder.Entity<PushSubscription>();
                pushSubscriptionEntityTypeBuilder.HasKey(e => e.Endpoint);
                pushSubscriptionEntityTypeBuilder.Ignore(p => p.Keys);
                pushSubscriptionEntityTypeBuilder.Property(e => e.District);
                pushSubscriptionEntityTypeBuilder.Property(e => e.LatestConfirmedCase);
            }
            catch (System.Exception exception)
            {
                System.Console.WriteLine(exception.Message);
            }
        }
    }
}
