using System.Text;
using MedicalBackend.Services.Abstraction;
using MedicalBackend.Utils;

namespace MedicalBackend.Services;

public class ShortLinkService: IShortLinkService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly IConfiguration _configuration;
    
    public ShortLinkService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string?> ShortLink(string link)
    {
        var linkDeleteShorten = "";
            var shortUrl = _configuration.GetSection("Short:url").Value ?? "";
            var shortKey = _configuration.GetSection("Short:key").Value ?? "";
            var request = new
            {
                domain = shortUrl,
                originalURL = link
            };

            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("authorization", shortKey);
            var responseShort = await _httpClient.PostAsync("https://api.short.io/links/public", content);
            var jsonString = await responseShort.Content.ReadAsStringAsync();
            ShortResponse shortObject =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ShortResponse>(jsonString ?? "") ?? new ShortResponse();
            linkDeleteShorten = shortObject?.shortURL;
            return linkDeleteShorten;
    }
}