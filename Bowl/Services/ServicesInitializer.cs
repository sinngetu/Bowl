using Bowl.Services.Business;

namespace Bowl.Services
{
    public class ServicesInitializer
    {
        public static void Initialize(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IMediaService, MediaService>();
            builder.Services.AddScoped<IKeywordService, KeywordService>();
            builder.Services.AddScoped<IPlatformService, PlatformService>();
            builder.Services.AddScoped<IHotlistService, HotlistService>();
            builder.Services.AddScoped<INewsService, NewsService>();
        }
    }
}
