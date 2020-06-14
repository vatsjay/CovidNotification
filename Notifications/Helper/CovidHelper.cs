using Demo.AspNetCore.PushNotifications.Model;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Demo.AspNetCore.PushNotifications.Helper
{
    public sealed class CovidHelper
    {
        #region

        private static CovidHelper _instance;

        private static class InnerClass
        {
            static InnerClass()
            {
            }

            internal static CovidHelper _instance = new CovidHelper();
        }

        #endregion

        private CovidHelper()
        {
        }

        public static CovidHelper Instance { get { return InnerClass._instance; } }

        internal long GetLatestCountForDistrict(string district, string url)
        {
            long count = 0;
            try
            {
                Dictionary<string, District> districtDataList = initializeDistrictDictionary(url).Result;
                var foundDistrict = districtDataList.FirstOrDefault(x => String.Equals(x.Key, district, StringComparison.OrdinalIgnoreCase));
                if (foundDistrict.Key != null)
                {
                    var foundValue = foundDistrict.Value;
                    count = foundValue.Confirmed;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return count;
        }

        private async Task<Dictionary<string, District>> initializeDistrictDictionary(string districtInfo)
        {
            string url = districtInfo;
            Dictionary<string, District> districtData = new Dictionary<string, District>();

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var info = response.Content;
                    var a = info.ReadAsStringAsync();
                    var countryObject = JsonConvert.DeserializeObject<Country>(await info.ReadAsStringAsync());

                    populateDistricDictionary(countryObject, districtData);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message : {exception.Message}, \n Stack Trace : {exception.StackTrace}");
            }

            return districtData;
        }

        private void populateDistricDictionary(Country countryObject, Dictionary<string, District> districtDataList)
        {
            try
            {
                var stateList = new List<State>() { countryObject.AndamanAndNicobarIslands, countryObject.AndhraPradesh, countryObject.ArunachalPradesh, countryObject.Assam, countryObject.Bihar, countryObject.Chandigarh, countryObject.Chhattisgarh, countryObject.DadraAndNagarHaveliAndDamanAndDiu, countryObject.Delhi, countryObject.Goa, countryObject.Gujarat, countryObject.Haryana, countryObject.HimachalPradesh, countryObject.JammuAndKashmir, countryObject.Jharkhand, countryObject.Karnataka, countryObject.Kerala, countryObject.Ladakh, countryObject.Lakshadweep, countryObject.MadhyaPradesh, countryObject.Maharashtra, countryObject.Manipur, countryObject.Meghalaya, countryObject.Mizoram, countryObject.Nagaland, countryObject.Odisha, countryObject.Puducherry, countryObject.Punjab, countryObject.Rajasthan, countryObject.Sikkim, countryObject.StateUnassigned, countryObject.TamilNadu, countryObject.Telangana, countryObject.Tripura, countryObject.Uttarakhand, countryObject.UttarPradesh, countryObject.WestBengal };

                foreach (var state in stateList)
                {
                    foreach (var individualDistrict in state.DistrictData)
                    {
                        districtDataList[individualDistrict.Key] = individualDistrict.Value;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"message : {exception.Message}  stack trace : {exception.StackTrace}");
            }
        }

        internal void NotifySubscribers(IPushNotificationsQueue pushNotificationsQueue, IPushSubscriptionStore subscriptionStore, string districtWiseCasesUrl)
        {
            subscriptionStore.ForEachSubscriptionAsync((subscription) =>
            {
                var currentDistrictCases = GetLatestCountForDistrict(subscription.District, districtWiseCasesUrl);
                if (currentDistrictCases > subscription.LatestConfirmedCase)
                {
                    pushNotificationsQueue.Enqueue(new Lib.Net.Http.WebPush.PushMessage($"Cases in your District {subscription.District} has increased the new number is {currentDistrictCases}")
                    {
                        Topic = "Covid 19 Alert",
                        Urgency = Lib.Net.Http.WebPush.PushMessageUrgency.High
                    });
                    subscription.LatestConfirmedCase = currentDistrictCases;

                    subscriptionStore.UpdateSubscriptionAsync(subscription);
                }
            });
        }
    }
}
