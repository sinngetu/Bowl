using System.Text.Json.Serialization;

namespace Bowl.Models.Request
{
    public class GetNewsParameters
    {
        public int[] Medium { get; set; } = Array.Empty<int>();
        public string Title { get; set; } = "";
        public int Status { get; set; }
        public int[] Tags { get; set; } = Array.Empty<int>();
        public DateTime Start { get; set; } = DateTime.Now.AddMinutes(-30);
        public DateTime End { get; set; } = DateTime.Now;
    }

    public class AttachTagToNewsRequestBody
    {
        public string Hash { get; set; }
        public int[] Tags { get; set; }
    }

    public class AddTagRequestBody
    {
        public string Content { get; set; }
        public ColorDetail Color { get; set; }

        public class ColorDetail
        {
            public string Dominate { get; set; }
            public string? Light { get; set; }
        }
    }

    public class AddKeywordRequestBody
    {
        public string Content { get; set; }
        public int Type { get; set; }
    }

    public class AddSearchRequestBody
    {
        public string Word { get; set; }
        public string Url { get; set; }
    }

    public class UpdateSearchRequestBody
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string Url { get; set; }
    }

    public class RawWeiboHotlist
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("icon_desc")]
        public string Icon { get; set; }
    }
}
