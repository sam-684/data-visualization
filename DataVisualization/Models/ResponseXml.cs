using Newtonsoft.Json;

namespace DataVisualization.Models;

public class ResponseXml
{
    [JsonProperty("?xml")]
    public XmlEncoding xmlEncoding { get; set; } = new XmlEncoding();
    public ProductsResponse ProductsResponse { get; set; } = new ProductsResponse();
}

public class XmlEncoding
{
    [JsonProperty("@version")]
    public string version { get; set; } = string.Empty;

    [JsonProperty("@encoding")]
    public string encoding { get; set; } = string.Empty;
}

public class ProductsResponse
{
    [JsonProperty("@xmlns:xsi")]
    public string xmlnsxsi { get; set; } = string.Empty;

    [JsonProperty("@xmlns:xsd")]
    public string xmlnsxsd { get; set; } = string.Empty;
    public List<Product> product { get; set; } = new List<Product>();
}
