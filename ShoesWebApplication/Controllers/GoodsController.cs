
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoesWebApplication.Models;
using SixLabors.ImageSharp;

[Authorize]
public class GoodsController : Controller
{
    private readonly MyDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public GoodsController(MyDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: GOODS
    public async Task<IActionResult> Index(string search, int? supplierId, string sortOrder)    
    {
        var goods = _context.Goods
        .Include(g => g.GoodMeasurement)
        .Include(g => g.GoodCategory)
        .Include(g => g.Supplier)
        .Include(g => g.Manufacturer)
        .AsQueryable();

        if (User.IsInRole("Администратор") || User.IsInRole("Менеджер")) {
            // Поиск
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                goods = goods.Where(g =>
                    g.Name.ToLower().Contains(search) ||
                    g.Description.ToLower().Contains(search) ||
                    g.Supplier.Name.ToLower().Contains(search) ||
                    g.Manufacturer.Name.ToLower().Contains(search) ||
                    g.GoodCategory.Category.ToLower().Contains(search) ||
                    g.GoodMeasurement.Name.ToLower().Contains(search));
            }

            // Фильтр по поставщику
            if (supplierId.HasValue)
            {
                goods = goods.Where(g => g.SupplierId == supplierId);
            }

            // Сортировка по количеству
            goods = sortOrder switch
            {
                "asc" => goods.OrderBy(g => g.Count),
                "desc" => goods.OrderByDescending(g => g.Count),
                _ => goods
            };

            ViewBag.Search = search;
            ViewBag.SupplierId = supplierId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Suppliers = await _context.Suppliers.ToListAsync();
        }

        return View(await goods.ToListAsync());
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
    [Authorize(Roles ="Администратор")]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.GoodCategories, "Id", "Category");
        ViewBag.Manufacturers = new SelectList(_context.Manufacturers, "Id", "Name");
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");
        ViewBag.Measurements = new SelectList(_context.Measurements, "Id", "Name");

        return View();
    }

    // POST: GOODS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Create(Good good, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            LoadSelectLists();
            return View(good);
        }

        var lastId = await _context.Goods
            .Select(x => x.Id)
            .ToListAsync();

        int max = lastId
            .Where(x => int.TryParse(x, out _))
            .Select(int.Parse)
            .DefaultIfEmpty(0)
            .Max();

        good.Id = (max + 1).ToString();

        if (imageFile != null && imageFile.Length > 0)
        {
            /*using var image = Image.Load(imageFile.OpenReadStream());

            if (image.Width > 300 || image.Height > 200)*/
            if (imageFile.Length > 1024 * 1024)
            {
                ModelState.AddModelError("", "Размер изображения не должен превышать 300x200");
                LoadSelectLists();
                return View(good);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

            var path = Path.Combine(_environment.WebRootPath, "images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            good.Image = fileName;
        }

        _context.Goods.Add(good);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: GOODS/Edit/5
    [Authorize(Roles = "Администратор")]
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

        LoadSelectLists();

        return View(good);
    }

    // POST: GOODS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Edit(string? id, [Bind("Id,Name,Price,Discount,Count,Description,Image,GoodMeasurementId,GoodCategoryId,SupplierId,ManufacturerId,GoodCategory,GoodMeasurement,Manufacturer,OrderItems,Supplier")] Good good, IFormFile imageFile)
    {
        if (id != good.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile != null)
                {
                    if (!string.IsNullOrEmpty(good.Image))
                    {
                        var oldPath = Path.Combine(
                            _environment.WebRootPath,
                            "images",
                            good.Image);

                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    /*using var image = Image.Load(imageFile.OpenReadStream());

                    if (image.Width > 300 || image.Height > 200)*/
                    if (imageFile.Length > 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Размер изображения не должен превышать 300x200");

                        return View(good);
                    }

                    var fileName = Guid.NewGuid() +
                                   Path.GetExtension(imageFile.FileName);

                    var path = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    good.Image = fileName;
                }

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

        LoadSelectLists();

        return View(good);
    }

    // GET: GOODS/Delete/5
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var good = await _context.Goods
    .Include(g => g.OrderItems)
    .Include(g => g.Supplier)
    .Include(g => g.Manufacturer)
    .Include(g => g.GoodCategory)
    .Include(g => g.GoodMeasurement)
    .FirstOrDefaultAsync(g => g.Id == id);
        if (good == null)
        {
            return NotFound();
        }

        return View(good);
    }

    // POST: GOODS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Администратор")]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        var good = await _context.Goods
    .Include(g => g.OrderItems)
    .Include(g => g.Supplier)
    .Include(g => g.Manufacturer)
    .Include(g => g.GoodCategory)
    .Include(g => g.GoodMeasurement)
    .FirstOrDefaultAsync(g => g.Id == id);

        if (good == null)
            return NotFound();

        // запрет удаления если есть заказы
        if (good.OrderItems != null && good.OrderItems.Any())
        {
            ModelState.AddModelError("", "Товар нельзя удалить, так как он есть в заказах");

            return View(good);
        }

        // удалить изображение (если есть)
        if (!string.IsNullOrEmpty(good.Image))
        {
            var path = Path.Combine(_environment.WebRootPath, "images", good.Image);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        _context.Goods.Remove(good);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool GoodExists(string? id)
    {
        return _context.Goods.Any(e => e.Id == id);
    }

    private void LoadSelectLists()
    {
        ViewBag.Categories = new SelectList(_context.GoodCategories, "Id", "Category");
        ViewBag.Manufacturers = new SelectList(_context.Manufacturers, "Id", "Name");
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");
        ViewBag.Measurements = new SelectList(_context.Measurements, "Id", "Name");
    }
}
