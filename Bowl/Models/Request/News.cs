namespace Bowl.Models.Request.News
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
        public string Content;
        public int Type;
    }

    public class AddSearchRequestBody
    {
        public string Word;
        public string Url;
    }

    public class UpdateSearchRequestBody
    {
        public int Id;
        public string Word;
        public string Url;
    }

    public class RemoveSearchRequestBody
    {
        public int Id;
    }
}
