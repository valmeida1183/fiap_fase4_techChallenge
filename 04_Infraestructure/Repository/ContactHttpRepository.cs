using Core.Entity;
using Core.Repository.Interface;
using Infraestructure.Repository.Base;
using System.Net.Http.Json;

namespace Infraestructure.Repository;
public class ContactHttpRepository : BaseHttpRepository<Contact>, IContactHttpRepository
{
    public ContactHttpRepository(IHttpClientFactory httpClientFactory) 
        : base(httpClientFactory, "persistence-api/v1/contacts")
    {        
    }

    public async Task<IList<Contact>> GetAllByDddAsync(int dddId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<Contact>>($"{_url}/ddd-code/{dddId}");

        return result is null ? new List<Contact>() : result;
    }

    public async Task<string> ResilienceTest(bool fail)
    {
        var response = await _httpClient.GetAsync($"{_url}/persistence-error-test/{fail}");

        // Let Polly handle the fallback/circuit breaker based on the response
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}
