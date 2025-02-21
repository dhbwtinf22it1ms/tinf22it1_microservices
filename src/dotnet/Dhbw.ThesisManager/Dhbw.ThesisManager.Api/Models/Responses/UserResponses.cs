using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dhbw.ThesisManager.Api.Models.Responses
{
    public class UserListResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<User> Ok { get; set; }
    }

    public class UserIdListResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<long> Ok { get; set; }
    }

    public class UserResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public User Ok { get; set; }
    }

    public class EmptyResponse
    {
        [JsonProperty("ok", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object Ok { get; set; }
    }

    public class RegistrationTokenResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Ok { get; set; }
    }
}
