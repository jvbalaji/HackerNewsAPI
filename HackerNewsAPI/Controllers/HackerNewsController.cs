// HackerNewsController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly HackerNewsService _hackerNewsService;

        // Constructor for HackerNewsController
        public HackerNewsController(HackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        /// <summary>
        /// Gets the best n stories from the Hacker News API based on their score.
        /// </summary>
        /// <param name="n">The number of top stories to retrieve.</param>
        /// <returns>An ActionResult containing a list of top n stories.</returns>
        [HttpGet("best-stories")]
        public async Task<ActionResult<List<HackerNewsStory>>> GetBestStories(int n)
        {
            try
            {
                if (n <= 0)
                {
                    // Return BadRequest if n is not a valid value
                    return BadRequest("Invalid value for 'n'.");
                }

                var bestStoryIds = await _hackerNewsService.GetBestStoryIdsAsync();

                if (bestStoryIds.Length == 0)
                {
                    // Handle case where story IDs couldn't be retrieved
                    return StatusCode(503, "Failed to fetch best story IDs.");
                }

                var topNStoryIds = bestStoryIds.Take(n).ToList();
                var stories = new List<HackerNewsStory>();

                foreach (var storyId in topNStoryIds)
                {
                    var storyDetails = await _hackerNewsService.GetStoryDetailsAsync(storyId);
                    if (storyDetails != null)
                    {
                        stories.Add(storyDetails);
                    }
                }

                if (stories.Count == 0)
                {
                    // Handle case where no valid story details could be retrieved
                    return StatusCode(503, "Failed to fetch story details.");
                }

                // Order the stories by score in descending order
                stories = stories.OrderByDescending(s => s.Score).ToList();

                return Ok(stories);
            }
            catch (ApiException ex)
            {
                // Handle custom ApiException
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                // Handle other unexpected exceptions
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}