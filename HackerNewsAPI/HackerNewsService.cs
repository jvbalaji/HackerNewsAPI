// HackerNewsService.cs
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HackerNewsAPI
{
    public class HackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<HackerNewsService> _logger;

        // Constructor for HackerNewsService
        public HackerNewsService(HttpClient httpClient, IConfiguration configuration, ILogger<HackerNewsService> logger)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["HackerNewsApiBaseUrl"];
            _logger = logger;
        }

        /// <summary>
        /// Gets the best story IDs from the Hacker News API.
        /// </summary>
        /// <returns>An array of best story IDs.</returns>
        public async Task<int[]> GetBestStoryIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<int[]>($"{_apiBaseUrl}/beststories.json");

                return response ?? Array.Empty<int>();
            }
            catch (HttpRequestException ex)
            {
                // Log and throw ApiException for network or API errors
                _logger.LogError(ex, "Error fetching best story IDs.");
                throw new ApiException("An error occurred while fetching best story IDs.", 500);
            }
        }

        /// <summary>
        /// Gets the details of a specific story from the Hacker News API.
        /// </summary>
        /// <param name="storyId">The ID of the story to retrieve details for.</param>
        /// <returns>The details of the specified story.</returns>
        public async Task<HackerNewsStory> GetStoryDetailsAsync(int storyId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<HackerNewsStory>($"{_apiBaseUrl}/item/{storyId}.json");

                return response;
            }
            catch (HttpRequestException ex)
            {
                // Log and throw ApiException for network or API errors
                _logger.LogError(ex, $"Error fetching details for story ID: {storyId}.");
                throw new ApiException($"An error occurred while fetching details for story ID: {storyId}.", 500);
            }
            catch (JsonException ex)
            {
                // Log and throw ApiException for JSON deserialization errors
                _logger.LogError(ex, $"Error deserializing response for story ID: {storyId}.");
                throw new ApiException($"An error occurred while processing the response for story ID: {storyId}.", 500);
            }
        }
    }

}
