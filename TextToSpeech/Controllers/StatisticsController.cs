using Microsoft.AspNetCore.Mvc;
using TextToSpeech.Models;
using TextToSpeech.Services;

namespace TextToSpeech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly RealtimeDatabaseService _dbService;

        public StatisticsController(RealtimeDatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("global-statistic")]
        public async Task<IActionResult> GetGlobalStatistic()
        {
            try
            {
                var path = "statistics/total";
                var globalStatistics = await _dbService.GetDataAsync<TotalStatistics>(path);

                if (globalStatistics == null)
                {
                    return NotFound(new
                    {
                        Message = "Global statistics not found in the database."
                    });
                }

                return Ok(new
                {
                    Message = "Successfully retrieved global statistics.",
                    Data = globalStatistics
                });
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    Message = "Error communicating with Firebase.",
                    Details = httpEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("language-statistic")]
        public async Task<IActionResult> GetLanguageStatistic()
        {
            try
            {
                var path = "statistics";
                var globalStatistics = await _dbService.GetDataAsync<LanguageStatisticsContainer>(path);

                if (globalStatistics == null)
                {
                    return NotFound(new
                    {
                        Message = "Language statistics not found in the database."
                    });
                }

                return Ok(new
                {
                    Message = "Successfully retrieved language statistics.",
                    Data = globalStatistics
                });
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    Message = "Error communicating with Firebase.",
                    Details = httpEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }
    }
}
