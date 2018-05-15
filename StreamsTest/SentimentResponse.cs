using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamsTest
{

    public class SentimentResponse
    {
        [JsonProperty(PropertyName = "documents")]
        public List<Sentiment> Sentiments { get; set; }
        [JsonProperty(PropertyName = "errors")]
        public object Errors { get; set; }
    }

    public class Sentiment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }

}
