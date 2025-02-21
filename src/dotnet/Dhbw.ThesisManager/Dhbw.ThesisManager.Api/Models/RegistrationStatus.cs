using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dhbw.ThesisManager.Api.Models
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class RegistrationStatus
    {
        [JsonProperty("pending", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Pending { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }
}
