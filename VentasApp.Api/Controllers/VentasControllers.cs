using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentasApp.Api.Data;
using VentasApp.Api.Models;

namespace VentasApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly VentasDbContext _context;

        public UsersController(VentasDbContext context)
        {
            _context = context;
        }

        [HttpGet("login")]
        public async Task<ActionResult<User>> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null) return Unauthorized();
            return user;
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly VentasDbContext _context;
        public ProductsController(VentasDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => await _context.Products.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (product.Id == 0) _context.Products.Add(product);
            else _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            bool hasOrders = await _context.OrderItems.AnyAsync(i => i.ProductId == id);
            if (hasOrders) return BadRequest("El producto tiene ventas asociadas");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly VentasDbContext _context;
        public CustomersController(VentasDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers() => await _context.Customers.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (customer.Id == 0) _context.Customers.Add(customer);
            else _context.Entry(customer).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            // Opcional: Verificar si tiene pedidos antes de eliminar
            bool hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == id);
            if (hasOrders) return BadRequest("El cliente tiene pedidos asociados y no puede eliminarse");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly VentasDbContext _context;
        public OrdersController(VentasDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Vendedor)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);

            // Reducir stock
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null) product.Stock -= item.Quantity;
            }

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            order.Status = status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/delivery")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] string deliveryStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            order.DeliveryStatus = deliveryStatus;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class VendedoresController : ControllerBase
    {
        private readonly VentasDbContext _context;
        public VendedoresController(VentasDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendedor>>> GetVendedores() => await _context.Vendedores.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Vendedor>> PostVendedor(Vendedor vendedor)
        {
            if (vendedor.Id == 0) _context.Vendedores.Add(vendedor);
            else _context.Entry(vendedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(vendedor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendedor(int id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor == null) return NotFound();
            _context.Vendedores.Remove(vendedor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly VentasDbContext _context;
        public EquiposController(VentasDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipo>>> GetEquipos() => 
            await _context.Equipos.Include(e => e.Vendedor).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Equipo>> PostEquipo(Equipo equipo)
        {
            if (equipo.Id == 0) _context.Equipos.Add(equipo);
            else _context.Entry(equipo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(equipo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();
            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
