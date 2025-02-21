using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dhbw.ThesisManager.Api.Models
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class Thesis
    {
        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("topic", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Topic { get; set; }

        [JsonProperty("student", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Student Student { get; set; }

        [JsonProperty("preparationPeriod", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public PreparationPeriod PreparationPeriod { get; set; }

        [JsonProperty("partnerCompany", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public PartnerCompany PartnerCompany { get; set; }

        [JsonProperty("operationalLocation", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public OperationalLocation OperationalLocation { get; set; }

        [JsonProperty("inCompanySupervisor", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public InCompanySupervisor InCompanySupervisor { get; set; }

        [JsonProperty("excludeSupervisorsFromCompanies", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExcludeSupervisorsFromCompanies { get; set; }

        private IDictionary<string, object> _additionalProperties;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }
    }
}
