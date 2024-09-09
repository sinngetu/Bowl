using Microsoft.AspNetCore.Mvc;
using Bowl.Common;
using Bowl.Models.Entities;
using Bowl.Models.Request;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("hotlist")]
    public class HotlistController : ControllerBase
    {
        private readonly ILogger<HotlistController> _logger;
        private readonly IHotlistService _hotlistService;
        private readonly IKeywordService _keywordService;

        public HotlistController(
            ILogger<HotlistController> logger,
            IHotlistService hotlistService,
            IKeywordService keywordService
        )
        {
            _logger = logger;
            _hotlistService = hotlistService;
            _keywordService = keywordService;
        }

        [HttpGet]
        public IActionResult GetHotlist([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            DateTime _start = (start ?? DateTime.Now.AddMinutes(-30)).ToLocalTime();
            DateTime _end = (end ?? DateTime.Now).ToLocalTime();

            var (err, data) = _hotlistService.GetHotlistByDate(_start, _end);
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPost("keyword")]
        public IActionResult AddKeyword([FromBody] AddKeywordRequestBody body)
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var keyword = new Keyword { Word = body.Content, Type = (int)KeywordType.Hotlist };
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

        [HttpGet("weibo")]
        public IActionResult GetWeibo()
        {
            var data = _hotlistService.GetWeiboList();

            return Ok(Utils.ErrorHandle(ErrorType.NoError, data));
        }
    }
}
