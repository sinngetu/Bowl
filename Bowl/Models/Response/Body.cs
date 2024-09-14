namespace Bowl.Models.Response
{
    public class NewsResponse
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

    public class BossNewsResponse
    {
        public string Hash { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string Date { get; set; }
        public int Type { get; set; }
    }

    public class HotlistResponse
    {
        public string Hash { get; set; }
        public string Content { get; set; }
        public int Platform { get; set; }
        public string Date { get; set; }
        public string Link { get; set; }
    }
}
