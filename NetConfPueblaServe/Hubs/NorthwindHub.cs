using Microsoft.AspNetCore.SignalR;
using NetConfPuebla.Entities;
using NetConfPueblaServe.Services;
using System.Threading.Tasks;

namespace NetConfPueblaServe.Hubs
{
    public class NorthwindHub : Hub
    {
        private readonly ServiceProduct _service;
        public NorthwindHub(ServiceProduct service)
        {
            _service = service;
        }

        public async Task SendInsertProduct(Product product)
        {
            var resultProduct = await _service.InsertProduct(product);
            if (resultProduct != null)
            {
                await Clients.All.SendAsync("ReceiveInsertProduct", resultProduct);

            }
        }
        public async Task SendUpdateProduct(int id, Product product)
        {
            var result = await _service.UpdateProduct(id, product);
            if (result)
            {
                await Clients.All.SendAsync("ReceiveUpdateProduct", id, product);

            }
        }
    }
}
