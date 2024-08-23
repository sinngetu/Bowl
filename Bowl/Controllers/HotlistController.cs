using Microsoft.AspNetCore.Mvc;
using Bowl.Common;
using Bowl.Models.Entities;
using Bowl.Models.Request.News;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("hotlist")]
    public class HotlistController : ControllerBase
    {
        private readonly ILogger<HotlistController> _logger;
        private readonly HotlistService _hotlistService;
        private readonly KeywordService _keywordService;

        public HotlistController(ILogger<HotlistController> logger, HotlistService hotlistService, KeywordService keywordService)
        {
            _logger = logger;
            _hotlistService = hotlistService;
            _keywordService = keywordService;
        }

        [HttpGet]
        public IActionResult GetHotlist([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            Utils.Log(_logger.LogTrace);

            var (err, data) = _hotlistService.GetHotlistByDate(start, end);
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpPost("keyword")]
        public IActionResult AddKeyword([FromBody] AddKeywordRequestBody body)
        {
            Utils.Log(_logger.LogTrace);

            var keyword = new Keyword { Word = body.Content, Type = (int)KeywordType.Hotlist };
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
    }
}
