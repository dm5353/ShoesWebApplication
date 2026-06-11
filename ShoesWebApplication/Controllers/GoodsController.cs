
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesWebApplication.Models;

[Authorize]
public class GoodsController : Controller
{
    private readonly MyDbContext _context;

    public GoodsController(MyDbContext context)
    {
        _context = context;
    }

    // GET: GOODS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Goods
    .Include(g => g.GoodMeasurement)
    .Include(g => g.GoodCategory)
    .Include(g => g.Supplier)
    .Include(g => g.Manufacturer)
    .ToListAsync());
    }

    // GET: GOODS/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var good = await _context.Goods
            .FirstOrDefaultAsync(m => m.Id == id);
        if (good == null)
        {
            return NotFound();
        }

        return View(good);
    }

    // GET: GOODS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: GOODS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price,Discount,Count,Description,Image,GoodMeasurementId,GoodCategoryId,SupplierId,ManufacturerId,GoodCategory,GoodMeasurement,Manufacturer,OrderItems,Supplier")] Good good)
    {
        if (ModelState.IsValid)
        {
            _context.Add(good);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(good);
    }

    // GET: GOODS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var good = await _context.Goods.FindAsync(id);
        if (good == null)
        {
            return NotFound();
        }
        return View(good);
    }

    // POST: GOODS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, [Bind("Id,Name,Price,Discount,Count,Description,Image,GoodMeasurementId,GoodCategoryId,SupplierId,ManufacturerId,GoodCategory,GoodMeasurement,Manufacturer,OrderItems,Supplier")] Good good)
    {
        if (id != good.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(good);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodExists(good.Id))
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
        return View(good);
    }

    // GET: GOODS/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var good = await _context.Goods
            .FirstOrDefaultAsync(m => m.Id == id);
        if (good == null)
        {
            return NotFound();
        }

        return View(good);
    }

    // POST: GOODS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        var good = await _context.Goods.FindAsync(id);
        if (good != null)
        {
            _context.Goods.Remove(good);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GoodExists(string? id)
    {
        return _context.Goods.Any(e => e.Id == id);
    }
}
