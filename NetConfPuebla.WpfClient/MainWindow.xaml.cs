using Microsoft.AspNetCore.SignalR.Client;
using NetConfPuebla.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace NetConfPuebla.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Product> products;
        private readonly HubConnection hubConnection;
        public MainWindow()
        {
            InitializeComponent();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/northwindhub")
                .WithAutomaticReconnect()
                .Build();
            hubConnection.On<Product>("ReceiveInsertProduct", product => InsertProduct(product));
            hubConnection.On<int, Product>("ReceiveUpdateProduct", (id, product) => UpdateProduct(id, product));
            hubConnection.StartAsync().GetAwaiter();

            buttonGetProduct.Click += async (sender, e) =>
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(
                    "https://netconfapi20201114124013.azurewebsites.net/api/products");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    products = JsonSerializer.Deserialize<List<Product>>(
                        json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    dataGridProducts.ItemsSource=products;
                }
            };

            buttonInsertProduct.Click += async (sender, e) =>
            {
                var product = new Product { Name = "Producto Nuevo desde WPF", UnitPrice = 23.34m, UnitsInStock = 100 };
                await hubConnection.InvokeAsync("SendInsertProduct", product);
            };

            buttonUpdateProduct.Click += async (sender, e) =>
            {
                var product = new Product { Name = "Producto Modificado desde WPF", UnitPrice = 23.34m, UnitsInStock = 100 };
                await hubConnection.InvokeAsync("SendUpdateProduct", 1, product);
            };
        }
        private void InsertProduct(Product product)
        {
            products.Add(product);
            dataGridProducts.Items.Refresh();
        }
        private void UpdateProduct(int id, Product product)
        {
            products.RemoveAt(id - 1);
            products.Insert(id - 1, product);
            dataGridProducts.Items.Refresh();
        }
    }
}

