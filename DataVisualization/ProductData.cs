using System.Configuration;
using System.Net.Mime;
using System.Xml.Linq;
using DataVisualization.Exceptions;
using DataVisualization.Models;
using Newtonsoft.Json;

namespace DataVisualization;

public class ProductData
{
    private readonly HttpClient _client = new HttpClient();
    private readonly string getTokenUrl = "https://auth.dkhardware.com/realms/ctesting/protocol/openid-connect/token";
    private readonly string getJsonProductsUrl = "https://dkh-c-testing-api.staging.dkhdev.com/products/json";
    private readonly string getXmlProductsUrl = "https://dkh-c-testing-api.staging.dkhdev.com/products/xml";

    public async Task<List<Product>> GetCommonProductListAsync()
    {
        try
        {
            // Ensure that the configuration is valid (client ID and client secret are present)
            EnsureConfigurationIsValid();

            // Retrieve the access token
            var token = await GetAccessTokenAsync();

            // Clear headers and set new headers for JSON request with authorization
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("ContentType", MediaTypeNames.Application.Json);
            _client.DefaultRequestHeaders.Add("Authorization", token);

            // Retrieve JSON and XML product lists
            var jsonProductList = await GetJsonProductListAsync();
            var xmlProductList = await GetXmlProductListAsync();

            // Find common products between JSON and XML lists
            return GetCommonProducts(jsonProductList, xmlProductList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<Product>();
        }
    }

    // Ensure that the configuration is valid (client ID and client secret are present)
    private void EnsureConfigurationIsValid()
    {
        var clientId = ConfigurationManager.AppSettings["client_id"];
        var clientSecret = ConfigurationManager.AppSettings["client_secret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new MissingOrEmptyConfigurationException("clientId or clientSecret is missing or empty.");
        }
    }

    // Get the access token from the authentication server
    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = ConfigurationManager.AppSettings["client_id"];
        var clientSecret = ConfigurationManager.AppSettings["client_secret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new MissingOrEmptyConfigurationException("clientId or clientSecret is missing or empty.");
        }

        var parameters = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" },
            { "scope", "products" }
        };

        var response = await _client.PostAsync(getTokenUrl, new FormUrlEncodedContent(parameters));
        response.EnsureSuccessStatusCode();
        var tokenResponseJson = await response.Content.ReadAsStringAsync();
        var token = ParseAccessToken(tokenResponseJson);

        if (string.IsNullOrEmpty(token))
        {
            throw new NullOrEmptyTokenException("Access token is null or empty.");
        }

        return token;
    }

    // Parse the access token from the JSON response
    private string ParseAccessToken(string tokenResponseJson)
    {
        var tokenResponse = JsonConvert.DeserializeObject<Token>(tokenResponseJson);
        return $"{tokenResponse?.token_type} {tokenResponse?.access_token}";
    }

    // Retrieve the JSON product list from the API
    private async Task<List<Product>> GetJsonProductListAsync()
    {
        var response = await _client.GetAsync(getJsonProductsUrl);
        response.EnsureSuccessStatusCode();
        var jsonProductList = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ProductList>(jsonProductList)?.products ?? new List<Product>();
    }

    // Retrieve the XML product list from the API and convert it to JSON
    private async Task<List<Product>> GetXmlProductListAsync()
    {
        var response = await _client.GetAsync(getXmlProductsUrl);
        response.EnsureSuccessStatusCode();
        var xmlProductList = await response.Content.ReadAsStringAsync();
        var jsonOfXml = JsonConvert.SerializeXNode(XDocument.Parse(xmlProductList));
        var productListXml = JsonConvert.DeserializeObject<ResponseXml>(jsonOfXml);
        return productListXml?.ProductsResponse.product ?? new List<Product>();
    }

    // Find common products between two lists, filtering out null or empty UPCs
    private List<Product> GetCommonProducts(List<Product> jsonProducts, List<Product> xmlProducts)
    {
        return xmlProducts?
            .IntersectBy(jsonProducts?.Select(j => j.upc) ?? Enumerable.Empty<string>(), x => x.upc)?
            .Where(product => !string.IsNullOrEmpty(product?.upc))
            .ToList() ?? new List<Product>();
    }
}