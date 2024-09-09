using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Bowl.Models.Entities;
using Bowl.Models.Request.News;
using Bowl.Common;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("news")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly INewsService _newsService;
        private readonly IKeywordService _keywordService;

        public NewsController(
            ILogger<NewsController> logger,
            INewsService newsService,
            IKeywordService keywordService
        )
        {
            _logger = logger;
            _newsService = newsService;
            _keywordService = keywordService;
        }

        [HttpGet]
        public IActionResult GetNews(
            [FromQuery] string? title,
            [FromQuery] int[]? tags,
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] int? status
        )
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var parameters = new GetNewsParameters
            {
                Title = title ?? string.Empty,
                Tags = tags ?? Array.Empty<int>(),
                Start = (start ?? DateTime.Now.AddMinutes(-30)).ToLocalTime(),
                End = (end ?? DateTime.Now).ToLocalTime(),
                Status = status ?? 0
            };

            var (err, data) = _newsService.GetNews(parameters);

            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPut("tag/attach")]
        public IActionResult AttachTagToNews([FromBody] AttachTagToNewsRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var tags = string.Join(string.Empty, body.Tags);
            var (err, data) = _newsService.UpdateNews(body.Hash, new News { Tags = tags });

            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPost("tag")]
        public IActionResult AddTag([FromBody] AddTagRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            try
            {
                var extend = JsonSerializer.Serialize(
                    body.Color,
                    new JsonSerializerOptions{ DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }
                );

                var keyword = new Keyword
                {
                    Word = body.Content,
                    Type = (int)KeywordType.Tag,
                    Extend = extend,
                };

                var (err, id) = _keywordService.AddKeyword(keyword);

                return Ok(Utils.ErrorHandle(err, id));
            }
            catch (Exception ex) {
                // JSON parse error
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{body}", body);
                return Ok(Errors.Dict[ErrorType.InvalidArgument]);
            }
        }

        [HttpDelete("tag")]
        public IActionResult RemoveTag([FromQuery] int id)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, isSuccess) = _keywordService.RemoveKeyword(id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpPost("keyword")]
        public IActionResult AddKeyword([FromBody] AddKeywordRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var keyword = new Keyword { Word = body.Content, Type = body.Type };
            var (err, id) = _keywordService.AddKeyword(keyword);

            return Ok(Utils.ErrorHandle(err, id));
        }

        [HttpDelete("keyword")]
        public IActionResult RemoveKeyword([FromQuery] int id)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, isSuccess) = _keywordService.RemoveKeyword(id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpPost("search")]
        public IActionResult AddSearch([FromBody] AddSearchRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var keyword = new Keyword { Word = body.Word, Type = (int)KeywordType.Search, Extend = body.Url };
            var (err, id) = _keywordService.AddKeyword(keyword);

            return Ok(Utils.ErrorHandle(err, id));
        }

        [HttpPut("search")]
        public IActionResult UpdateSearch([FromBody] UpdateSearchRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var keyword = new Keyword { Word = body.Word, Extend = body.Url };
            var (err, isSuccess) = _keywordService.UpdateKeyword(body.Id, keyword);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpDelete("search")]
        public IActionResult RemoveSearch([FromQuery] int id)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());
            
            var (err, isSuccess) = _keywordService.RemoveKeyword(id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpGet("boss")]
        public IActionResult GetBossNews([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, data) = _newsService.GetBossNewsByDate(start.ToLocalTime(), end.ToLocalTime());

            return Ok(Utils.ErrorHandle(err, data));
        }
    }
}
