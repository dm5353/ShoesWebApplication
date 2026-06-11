
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesWebApplication.Models;

[Authorize]
public class OrdersController : Controller
{
    private readonly MyDbContext _context;

    public OrdersController(MyDbContext context)
    {
        _context = context;
    }

    // GET: ORDERS
    [Authorize(Roles = "Администратор, Менеджер")]
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Orders.Include(o => o.User)
            .Include(o => o.Address)
            .Include(o => o.OrderStatus).ToListAsync());
    }

    // GET: ORDERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: ORDERS/Create
    [Authorize(Roles = "Администратор")]
    public IActionResult Create()
    {
        ViewData["Users"] = _context.Users.ToList();
        ViewData["Addresses"] = _context.Addresses.ToList();
        ViewData["Statuses"] = _context.OrderStatuses.ToList();

        return View();
    }

    // POST: ORDERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Create([Bind("Id,OrderDate,SupplyDate,AddressId,Code,OrderStatusId,UserId,Address,OrderItems,OrderStatus,User")] Order order)
    {
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Users"] = _context.Users.ToList();
        ViewData["Addresses"] = _context.Addresses.ToList();
        ViewData["Statuses"] = _context.OrderStatuses.ToList();

        return View(order);
    }

    // GET: ORDERS/Edit/5
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    // POST: ORDERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,OrderDate,SupplyDate,AddressId,Code,OrderStatusId,UserId,Address,OrderItems,OrderStatus,User")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: ORDERS/Delete/5
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: ORDERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int? id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
