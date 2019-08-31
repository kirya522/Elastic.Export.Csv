using System.Collections.Generic;
using System.Linq;
using ChoETL;
using Newtonsoft.Json.Linq;

namespace ElasticCSVExporter
{
    public class JsonPropertyEditor
    {
        public static readonly List<string> BASIC_TOKENS_TO_IGNORE = new List<string>
            {"gl2_remote_ip", "gl2_remote_port", "streams", "gl2_source_input", "gl2_source_node"};
        public static JObject ExcludeGrayLogMeta(JObject query, List<string> tokensToIgnore)
        {
            if (query.ContainsKey("_source"))
            {
                if (query["_source"].Children().Select(x => ((JProperty) x).Name).Contains("excludes"))
                {
                    tokensToIgnore.AddRange(((JArray) query["_source"]["excludes"]).ToObject<List<string>>());
                    tokensToIgnore = tokensToIgnore.ToHashSet().ToList();
                    (query["_source"] as JObject).Property("excludes").Replace(new JProperty("excludes",
                        JArray.FromObject(
                            tokensToIgnore)));
                }
                else
                {
                    ((JObject) query["_source"]).Add(new JProperty("excludes",
                        JArray.FromObject(
                            tokensToIgnore)));
                }
            }
            else
            {
                query.Add(new JProperty("_source",
                    new JObject {new JProperty("excludes", JArray.FromObject(tokensToIgnore))}));
            }
            return query;
        }
    }
}