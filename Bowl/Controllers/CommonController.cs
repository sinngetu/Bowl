using Microsoft.AspNetCore.Mvc;
using Bowl.Common;
using Bowl.Services.Business;

namespace Bowl.Controllers
{
    [ApiController]
    [Route("common")]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;
        private readonly IMediaService _mediaService;
        private readonly IPlatformService _platformService;
        private readonly IKeywordService _keywordService;

        public CommonController(
            ILogger<CommonController> logger,
            IMediaService mediaService,
            IPlatformService platformService,
            IKeywordService keywordService
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
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, data) = _mediaService.GetAllMedia();
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, data) = _platformService.GetAllPlatform();
            var result = Utils.ErrorHandle(err, data);
            
            return Ok(Utils.ErrorHandle(err, data));
        }

        [HttpGet("keyword")]
        public IActionResult GetKeyword()
        {
            _logger.LogTrace(Utils.GetClassNameAndMethodName());

            var (err, data) = _keywordService.GetAllKeyword();
            return Ok(Utils.ErrorHandle(err, data));
        }
    }
}
