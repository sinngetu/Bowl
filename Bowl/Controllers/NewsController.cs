using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Bowl.Models.Entities;
using Bowl.Models.Request.News;
using Bowl.Common;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            Utils.Log(_logger.LogTrace);

            var parameters = new GetNewsParameters
            {
                Title = title ?? string.Empty,
                Tags = tags ?? Array.Empty<int>(),
                Start = start ?? DateTime.Now.AddMinutes(-30),
                End = end ?? DateTime.Now,
                Status = status ?? 0
            };

            var (err, data) = _newsService.GetNews(parameters);

            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPut("tag/attach")]
        public IActionResult AttachTagToNews([FromBody] AttachTagToNewsRequestBody body)
        {
            Utils.Log( _logger.LogTrace);

            var tags = string.Join(string.Empty, body.Tags);
            var (err, data) = _newsService.UpdateNews(body.Hash, new News { Tags = tags });

            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPost("tag")]
        public IActionResult AddTag([FromBody] AddTagRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            try
            {
                var extend = JsonSerializer.Serialize(body.Color);

                var keyword = new Keyword
                {
                    Word = body.Content,
                    Type = (int)KeywordType.Tag,
                    Extend = extend,
                };

                var (err, data) = _keywordService.AddKeyword(keyword);

                return Ok(Utils.ErrorHandle(err, data));
            }
            catch (Exception ex) {
                // JSON parse error
                Utils.Log(_logger.LogError, body, ex);
                return Ok(Errors.Dict[ErrorType.InvalidArgument]);
            }
        }

        [HttpDelete("tag")]
        public IActionResult RemoveTag([FromBody] RemoveTagRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var (err, isSuccess) = _keywordService.RemoveKeyword(body.Id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpPost("keyword")]
        public IActionResult AddKeyword([FromBody] AddKeywordRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var keyword = new Keyword { Word = body.Content, Type = body.Type };
            var (err, isSuccess) = _keywordService.AddKeyword(keyword);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpDelete("keyword")]
        public IActionResult RemoveKeyword([FromBody] RemoveKeywordRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var (err, isSuccess) = _keywordService.RemoveKeyword(body.Id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpPost("search")]
        public IActionResult AddSearch([FromBody] AddSearchRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var keyword = new Keyword { Word = body.Word, Type = (int)KeywordType.Search, Extend = body.Url };
            var (err, isSuccess) = _keywordService.AddKeyword(keyword);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpPut("search")]
        public IActionResult UpdateSearch([FromBody] UpdateSearchRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var keyword = new Keyword { Word = body.Word, Extend = body.Url };
            var (err, isSuccess) = _keywordService.UpdateKeyword(body.Id, keyword);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpDelete("search")]
        public IActionResult RemoveSearch([FromBody] RemoveSearchRequestBody body)
        {
            Utils.Log(_logger.LogTrace);
            
            var (err, isSuccess) = _keywordService.RemoveKeyword(body.Id);

            return Ok(Utils.ErrorHandle(err, isSuccess));
        }

        [HttpGet("boss")]
        public IActionResult GetBossNews([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            Utils.Log(_logger.LogTrace);

            var (err, data) = _newsService.GetBossNewsByDate(start, end);

            return Ok(Utils.ErrorHandle(err, data));
        }
    }
}
