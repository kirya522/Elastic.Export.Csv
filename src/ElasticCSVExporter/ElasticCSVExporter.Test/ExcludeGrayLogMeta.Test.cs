using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ElasticCSVExporter.Test
{
    [TestFixture]
    public class ExcludeGrayLogMetaTest
    {
        //[Test]
        public async Task IsElasticAlive()
        {
            var url = "http://192.168.1.152:9200";
            var elasticLogGetter= new ElasticLogGetter(url);
            var res = await elasticLogGetter.GetLogs(new JObject());
            Assert.True(res!=null);
        }
        
        [Test]
        public void SourceExistsAppendExcludes()
        {
            var obj = @"{
                        query:{},
                        _source:{excludes:['jora']},
                        }";
            var jObject = JObject.Parse(obj);
            jObject = JsonPropertyEditor.ExcludeGrayLogMeta(jObject, JsonPropertyEditor.BASIC_TOKENS_TO_IGNORE);
            Assert.True(((JArray)jObject["_source"]["excludes"]).Count > 1);
        }
        
        [Test]
        public void SourceExistsCreateExludes()
        {
            var obj = @"{
                        query:{},
                        _source:{},
                        }";
            var jObject = JObject.Parse(obj); 
            jObject = JsonPropertyEditor.ExcludeGrayLogMeta(jObject, JsonPropertyEditor.BASIC_TOKENS_TO_IGNORE);
            Assert.True(((JArray)jObject["_source"]["excludes"]).Count > 0);
        }

        [Test]
        public void SourceDoNotExists()
        {
            var obj = @"{
                        query:{}
                        }";
            var jObject = JObject.Parse(obj); 
            jObject = JsonPropertyEditor.ExcludeGrayLogMeta(jObject, JsonPropertyEditor.BASIC_TOKENS_TO_IGNORE);
            Assert.True(jObject["_source"] != null && ((JArray)jObject["_source"]["excludes"]).Count > 0);
        }
    }
}