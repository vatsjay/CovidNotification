using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.AspNetCore.PushNotifications.Model
{

    public partial class Country
    {
        [JsonProperty("State Unassigned")]
        public State StateUnassigned { get; set; }

        [JsonProperty("Andaman and Nicobar Islands")]
        public State AndamanAndNicobarIslands { get; set; }

        [JsonProperty("Andhra Pradesh")]
        public State AndhraPradesh { get; set; }

        [JsonProperty("Arunachal Pradesh")]
        public State ArunachalPradesh { get; set; }

        [JsonProperty("Assam")]
        public State Assam { get; set; }

        [JsonProperty("Bihar")]
        public State Bihar { get; set; }

        [JsonProperty("Chandigarh")]
        public State Chandigarh { get; set; }

        [JsonProperty("Chhattisgarh")]
        public State Chhattisgarh { get; set; }

        [JsonProperty("Delhi")]
        public State Delhi { get; set; }

        [JsonProperty("Dadra and Nagar Haveli and Daman and Diu")]
        public State DadraAndNagarHaveliAndDamanAndDiu { get; set; }

        [JsonProperty("Goa")]
        public State Goa { get; set; }

        [JsonProperty("Gujarat")]
        public State Gujarat { get; set; }

        [JsonProperty("Himachal Pradesh")]
        public State HimachalPradesh { get; set; }

        [JsonProperty("Haryana")]
        public State Haryana { get; set; }

        [JsonProperty("Jharkhand")]
        public State Jharkhand { get; set; }

        [JsonProperty("Jammu and Kashmir")]
        public State JammuAndKashmir { get; set; }

        [JsonProperty("Karnataka")]
        public State Karnataka { get; set; }

        [JsonProperty("Kerala")]
        public State Kerala { get; set; }

        [JsonProperty("Ladakh")]
        public State Ladakh { get; set; }

        [JsonProperty("Lakshadweep")]
        public State Lakshadweep { get; set; }

        [JsonProperty("Maharashtra")]
        public State Maharashtra { get; set; }

        [JsonProperty("Meghalaya")]
        public State Meghalaya { get; set; }

        [JsonProperty("Manipur")]
        public State Manipur { get; set; }

        [JsonProperty("Madhya Pradesh")]
        public State MadhyaPradesh { get; set; }

        [JsonProperty("Mizoram")]
        public State Mizoram { get; set; }

        [JsonProperty("Nagaland")]
        public State Nagaland { get; set; }

        [JsonProperty("Odisha")]
        public State Odisha { get; set; }

        [JsonProperty("Punjab")]
        public State Punjab { get; set; }

        [JsonProperty("Puducherry")]
        public State Puducherry { get; set; }

        [JsonProperty("Rajasthan")]
        public State Rajasthan { get; set; }

        [JsonProperty("Sikkim")]
        public State Sikkim { get; set; }

        [JsonProperty("Telangana")]
        public State Telangana { get; set; }

        [JsonProperty("Tamil Nadu")]
        public State TamilNadu { get; set; }

        [JsonProperty("Tripura")]
        public State Tripura { get; set; }

        [JsonProperty("Uttar Pradesh")]
        public State UttarPradesh { get; set; }

        [JsonProperty("Uttarakhand")]
        public State Uttarakhand { get; set; }

        [JsonProperty("West Bengal")]
        public State WestBengal { get; set; }
    }

    public partial class District
    {
        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("active")]
        public long Active { get; set; }

        [JsonProperty("confirmed")]
        public long Confirmed { get; set; }

        [JsonProperty("deceased")]
        public long Deceased { get; set; }

        [JsonProperty("recovered")]
        public long Recovered { get; set; }

        [JsonProperty("delta")]
        public Delta Delta { get; set; }
    }

    public partial class Delta
    {
        [JsonProperty("confirmed")]
        public long Confirmed { get; set; }

        [JsonProperty("deceased")]
        public long Deceased { get; set; }

        [JsonProperty("recovered")]
        public long Recovered { get; set; }
    }

    public partial class State
    {
        [JsonProperty("districtData")]
        public Dictionary<string, District> DistrictData { get; set; }

        [JsonProperty("statecode")]
        public string Statecode { get; set; }
    }
}
