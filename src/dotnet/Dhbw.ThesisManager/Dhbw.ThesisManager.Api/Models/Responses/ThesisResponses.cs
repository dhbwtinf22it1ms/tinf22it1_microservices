using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dhbw.ThesisManager.Api.Models.Responses
{
    public partial class ThesisResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Thesis Ok { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }

    public partial class ThesisSummaryListResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ThesisSummary> Ok { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }

    public partial class CommentResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Comment Ok { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }

    public partial class CommentListResponse
    {
        [JsonProperty("ok", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Comment> Ok { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }
}
