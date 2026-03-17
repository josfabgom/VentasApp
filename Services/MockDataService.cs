using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentasApp.Models;

namespace VentasApp.Services
{
    public interface IDataService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<List<Product>> GetProductsAsync();
        Task SaveProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<List<Customer>> GetCustomersAsync();
        Task SaveCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<List<Order>> GetOrdersAsync();
        Task SaveOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task UpdateOrderDeliveryStatusAsync(int orderId, string newStatus);
        Task<List<Vendedor>> GetVendedoresAsync();
        Task SaveVendedorAsync(Vendedor vendedor);
        Task<bool> DeleteVendedorAsync(int id);
        Task<List<Equipo>> GetEquiposAsync();
        Task SaveEquipoAsync(Equipo equipo);
        Task<bool> DeleteEquipoAsync(int id);
    }

    public class MockDataService : IDataService
    {
        private List<User> _users;
        private List<Product> _products;
        private List<Customer> _customers;
        private List<Order> _orders;
        private List<Vendedor> _vendedores;
        private List<Equipo> _equipos;

        public MockDataService()
        {
            _users = new List<User>
            {
                new User { Id = 1, Username = "admin", Password = "123", Role = "Admin" },
                new User { Id = 2, Username = "vendedor", Password = "123", Role = "Vendedor" }
            };

            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop Dell", Description = "Laptop para desarrollo", Price = 1200.50m, Stock = 10, Barcode="1234567890", ImageUrl="dotnet_bot.png" },
                new Product { Id = 2, Name = "Mouse Inalámbrico", Description = "Mouse ergonómico", Price = 25.00m, Stock = 50, Barcode="0987654321", ImageUrl="dotnet_bot.png" },
                new Product { Id = 3, Name = "Teclado Mecánico", Description = "Teclado RGB", Price = 75.00m, Stock = 20, Barcode="1122334455", ImageUrl="dotnet_bot.png" },
                new Product { Id = 4, Name = "Monitor LG 24'", Description = "Monitor IPS 1080p", Price = 150.00m, Stock = 15, Barcode="5544332211", ImageUrl="dotnet_bot.png" },
            };

            _customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Juan Pérez", Email = "juan@example.com", Phone = "555-0101", Address = "Calle Falsa 123", Localidad = "CABA", Provincia = "Buenos Aires", Barrio = "Palermo", Zona = "Norte", Tipo = "Minorista" },
                new Customer { Id = 2, Name = "María Gómez", Email = "maria@example.com", Phone = "555-0202", Address = "Av. Siempre Viva 456", Localidad = "Rosario", Provincia = "Santa Fe", Barrio = "Centro", Zona = "Oeste", Tipo = "Mayorista" }
            };

            _orders = new List<Order>();

            _vendedores = new List<Vendedor>
            {
                new Vendedor { Id = 1, Nombre = "Vendedor Test 1", Email = "vendedor1@test.com", Telefono = "111-222" },
                new Vendedor { Id = 2, Nombre = "Vendedor Test 2", Email = "vendedor2@test.com", Telefono = "333-444" }
            };

            _equipos = new List<Equipo>
            {
                new Equipo { Id = 1, Nombre = "Samsung S24", Tipo = "Celular", NumeroSerie = "SN123456", VendedorId = 1, Vendedor = _vendedores[0] },
                new Equipo { Id = 2, Nombre = "Dell Latitude", Tipo = "PC", NumeroSerie = "SN789012", VendedorId = 2, Vendedor = _vendedores[1] }
            };

            _orders = new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, Customer = _customers[0], VendedorId = 1, Vendedor = _vendedores[0], Date = DateTime.Now, TotalAmount = 1225.50m, Status = "Facturado" },
                new Order { Id = 2, CustomerId = 2, Customer = _customers[1], VendedorId = 1, Vendedor = _vendedores[0], Date = DateTime.Now, TotalAmount = 75.00m, Status = "Pendiente" },
                new Order { Id = 3, CustomerId = 1, Customer = _customers[0], VendedorId = 2, Vendedor = _vendedores[1], Date = DateTime.Now, TotalAmount = 150.00m, Status = "Facturado" },
                new Order { Id = 4, CustomerId = 2, Customer = _customers[1], VendedorId = 1, Vendedor = _vendedores[0], Date = DateTime.Now.AddDays(-1), TotalAmount = 500.00m, Status = "Facturado" }
            };
        }

        public async Task<List<Vendedor>> GetVendedoresAsync()
        {
            await Task.Delay(200);
            return _vendedores.ToList();
        }

        public async Task SaveVendedorAsync(Vendedor vendedor)
        {
            await Task.Delay(200);
            if (vendedor.Id == 0)
            {
                vendedor.Id = _vendedores.Any() ? _vendedores.Max(v => v.Id) + 1 : 1;
                _vendedores.Add(vendedor);
            }
            else
            {
                var existing = _vendedores.FirstOrDefault(v => v.Id == vendedor.Id);
                if (existing != null)
                {
                    existing.Nombre = vendedor.Nombre;
                    existing.Email = vendedor.Email;
                    existing.Telefono = vendedor.Telefono;
                }
            }
        }

        public async Task<bool> DeleteVendedorAsync(int id)
        {
            await Task.Delay(200);
            var existsInEquipos = _equipos.Any(e => e.VendedorId == id);
            if (existsInEquipos) return false;

            var item = _vendedores.FirstOrDefault(v => v.Id == id);
            if (item != null)
            {
                _vendedores.Remove(item);
                return true;
            }
            return false;
        }

        public async Task<List<Equipo>> GetEquiposAsync()
        {
            await Task.Delay(200);
            return _equipos.ToList();
        }

        public async Task SaveEquipoAsync(Equipo equipo)
        {
            await Task.Delay(200);
            // Refresh relation
            if (equipo.VendedorId.HasValue)
            {
                equipo.Vendedor = _vendedores.FirstOrDefault(v => v.Id == equipo.VendedorId);
            }

            if (equipo.Id == 0)
            {
                equipo.Id = _equipos.Any() ? _equipos.Max(e => e.Id) + 1 : 1;
                _equipos.Add(equipo);
            }
            else
            {
                var existing = _equipos.FirstOrDefault(e => e.Id == equipo.Id);
                if (existing != null)
                {
                    existing.Nombre = equipo.Nombre;
                    existing.Tipo = equipo.Tipo;
                    existing.NumeroSerie = equipo.NumeroSerie;
                    existing.VendedorId = equipo.VendedorId;
                    existing.Vendedor = equipo.Vendedor;
                }
            }
        }

        public async Task<bool> DeleteEquipoAsync(int id)
        {
            await Task.Delay(200);
            var item = _equipos.FirstOrDefault(e => e.Id == id);
            if (item != null)
            {
                _equipos.Remove(item);
                return true;
            }
            return false;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            await Task.Delay(500); // Simulate network
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await Task.Delay(300);
            return _products.ToList();
        }

        public async Task SaveProductAsync(Product product)
        {
            await Task.Delay(300);
            if (product.Id == 0)
            {
                product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
                _products.Add(product);
            }
            else
            {
                var existing = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existing != null)
                {
                    existing.Name = product.Name;
                    existing.Description = product.Description;
                    existing.Price = product.Price;
                    existing.Stock = product.Stock;
                    existing.Barcode = product.Barcode;
                    existing.QrCode = product.QrCode;
                    existing.ImageUrl = product.ImageUrl;
                }
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            await Task.Delay(200);

            // Verificamos si existe en pedidos
            bool hasOrders = _orders.Any(o => o.Items.Any(i => i.ProductId == productId));
            if (hasOrders)
            {
                return false; // No se puede eliminar si tiene ventas
            }

            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _products.Remove(product);
                return true;
            }
            return false;
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            await Task.Delay(300);
            return _customers.ToList();
        }

        public async Task SaveCustomerAsync(Customer customer)
        {
            await Task.Delay(300);
            if (customer.Id == 0)
            {
                customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
                _customers.Add(customer);
            }
            else
            {
                var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
                if (existing != null)
                {
                    existing.Name = customer.Name;
                    existing.Email = customer.Email;
                    existing.Phone = customer.Phone;
                    existing.Address = customer.Address;
                    existing.Localidad = customer.Localidad;
                    existing.Provincia = customer.Provincia;
                    existing.Barrio = customer.Barrio;
                    existing.Zona = customer.Zona;
                    existing.Tipo = customer.Tipo;
                }
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            await Task.Delay(200);
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return true;
            }
            return false;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            await Task.Delay(300);
            return _orders.ToList();
        }

        public async Task SaveOrderAsync(Order order)
        {
            await Task.Delay(500);
            if (order.Id == 0)
            {
                order.Id = _orders.Any() ? _orders.Max(o => o.Id) + 1 : 1;
                _orders.Add(order);
                // Reduce stock
                foreach (var item in order.Items)
                {
                    var product = _products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            await Task.Delay(300);
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = newStatus;
            }
        }

        public async Task UpdateOrderDeliveryStatusAsync(int orderId, string newStatus)
        {
            await Task.Delay(300);
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.DeliveryStatus = newStatus;
            }
        }
    }
}
