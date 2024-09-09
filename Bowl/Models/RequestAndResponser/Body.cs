namespace Bowl.Models.Request.News
{
    // Request
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

    // Response
    public class ResponseNews
    {
        public string Hash { get; set; }
        public string Link { get; set; }
        public int Medium { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public int Status { get; set; }
        public int[] Tags { get; set; }
        public string Keyword { get; set; }
    }

    public class ResponseBossNews
    {
        public string Hash { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string Date { get; set; }
        public int Type { get; set; }
    }
}
