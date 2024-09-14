using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bowl.Models.LocalSocket
{
    public class Information
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("data")]
        public JsonElement? Data { get; set; }

        public static class ActionType
        {
            public const string SetWeiboHotlist = "set_weibo";
            public const string AddBossNews = "add_boss_news";
            public const string BigNews = "big_news";
        }
    }
}
