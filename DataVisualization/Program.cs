using ConsoleTables;

namespace DataVisualization;

    public class Program
    {
        public static async Task Main()
        {
            var productData = new ProductData();
            var productList = await productData.GetCommonProductListAsync();

            var productTable = new ConsoleTable("ItemCode", "UPC");

            foreach (var product in productList)
            {
                productTable.AddRow(product.itemCode, product.upc);
            }

            Console.WriteLine(productTable);
        }
    }