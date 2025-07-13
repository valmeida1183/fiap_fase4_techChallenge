using Core.Entity.Base;
using Core.Repository.Interface;
using System.Net;
using System.Net.Http.Json;

namespace Infraestructure.Repository.Base;
public abstract class BaseHttpRepository<T> : IHttpRepository<T> where T : BaseEntity
{
    protected readonly HttpClient _httpClient;    
    protected readonly string _url; 

    public BaseHttpRepository(IHttpClientFactory httpClientFactory, string endpoint)
    {
        _httpClient = httpClientFactory.CreateClient("PersistanceClientApi");        
        _url = $"/{endpoint}";
    }
    public async Task<IList<T>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<T>>(_url);

        return result is null ? new List<T>() : result;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var url = $"{_url}/{id}";

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task CreateAsync(T entity)
    {
        var response = await _httpClient.PostAsJsonAsync(_url, entity);
        response.EnsureSuccessStatusCode();
    }

    public async Task EditAsync(T entity)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_url}/{entity.Id}", entity);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(T entity)
    {
        var response = await _httpClient.DeleteAsync($"{_url}/{entity.Id}");
        response.EnsureSuccessStatusCode();
    }
}
