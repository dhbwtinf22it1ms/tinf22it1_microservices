using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dhbw.ThesisManager.Api.Models
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class PreparationPeriod
    {
        /// <summary>
        /// ISO-8601 timestamp
        /// </summary>
        [JsonProperty("from", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; set; }

        /// <summary>
        /// ISO-8601 timestamp
        /// </summary>
        [JsonProperty("to", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }
}
