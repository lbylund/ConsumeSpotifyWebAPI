using ConsumeSpotifyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumeSpotifyWebAPI.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;

        public SpotifyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Release>> GetNewReleases(string countryCode, int limit, string accesToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesToken);
            var response = await _httpClient.GetAsync($"browse/new-releases?country={countryCode}&limit={limit}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseObject = await JsonSerializer.DeserializeAsync<GetNewReleaseResult>(responseStream);

            return responseObject?.albums?.items.Select(i => new Release
            {
                Name = i.name,
                Date = i.release_date,
                ImageUrl = i.images.FirstOrDefault().url,
                Link = i.external_urls.spotify,
                Artist = string.Join(",", i.artists.Select(i => i.name))
            });
        }
    }
}
