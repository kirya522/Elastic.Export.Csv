using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChoETL;
using Elasticsearch.Net;
using Newtonsoft.Json.Linq;

namespace ElasticCSVExporter
{
    public class ElasticLogGetter
    {
        private const string BASIC_ELASTIC_URL = "http://localhost:9200";
        private const string BASIC_INDEX_PATTERN = "graylog_*";

        private readonly ElasticLowLevelClient client;

        public ElasticLogGetter(string url = BASIC_ELASTIC_URL, List<string> tokensToIgnore = null)
        {
            client = new ElasticLowLevelClient(new ConnectionConfiguration(new SingleNodeConnectionPool(new Uri(url))));
        }

        public async Task<Stream> GetLogs(JObject query, string indexPattern = BASIC_INDEX_PATTERN,
            List<string> tokensToIgnore = null)
        {
            tokensToIgnore = tokensToIgnore ?? JsonPropertyEditor.BASIC_TOKENS_TO_IGNORE;
            try
            {
                var data = await client.SearchAsync<StringResponse>(indexPattern,
                    JsonPropertyEditor.ExcludeGrayLogMeta(query, tokensToIgnore).ToString());
                var ms = new MemoryStream();
                var json = JObject.Parse(data.Body);

                //elastic specification and graylog
                using (var r = new ChoJSONReader(json["hits"]["hits"].Select(x => x["_source"])))
                {
                    using (var w = new ChoCSVWriter(ms, new ChoCSVRecordConfiguration {HasExcelSeparator = true})
                        .WithFirstLineHeader())
                    {
                        w.Write(r);
                    }
                }

                ms.Position = 0;
                return ms;
            }
            catch (Exception e)
            {
                throw new Exception("ElasticSearch error!", e);
            }
        }
    }
}