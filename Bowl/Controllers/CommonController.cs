using Microsoft.AspNetCore.Mvc;
using Bowl.Common;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;
        private readonly MediaService _mediaService;
        private readonly PlatformService _platformService;
        private readonly KeywordService _keywordService;

        public CommonController(
            ILogger<CommonController> logger,
            MediaService mediaService,
            PlatformService platformService,
            KeywordService keywordService
        )
        {
            _logger = logger;
            _mediaService = mediaService;
            _platformService = platformService;
            _keywordService = keywordService;
        }

        [HttpGet("media")]
        public IActionResult GetMedia()
        {
            Utils.Log(_logger.LogTrace);

            var (err, data) = _mediaService.GetAllMedia();
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            Utils.Log(_logger.LogTrace);

            var (err, data) = _platformService.GetAllPlatform();
            var result = Utils.ErrorHandle(err, data);
            
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpGet("keyword")]
        public IActionResult GetKeyword()
        {
            Utils.Log(_logger.LogTrace);

            var (err, data) = _keywordService.GetAllKeyword();
            return Ok(Utils.ErrorHandle(err, data));
        }
    }
}
