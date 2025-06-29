using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class TagsController : Controller
{
    private readonly AppDbContext _context;
    public TagsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Tags
    public IActionResult Index()
    {
        var tags = _context.Tags.ToList();
        return View(tags);
    }

    // POST: /Tags/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Tag name is required.";
            return RedirectToAction("Index");
        }
        var tag = new Tag { Name = name, Color = string.IsNullOrWhiteSpace(color) ? "#6a11cb" : color };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tag created successfully.";
        return RedirectToAction("Index");
    }

    // POST: /Tags/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            TempData["Error"] = "Tag not found.";
            return RedirectToAction("Index");
        }
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tag deleted successfully.";
        return RedirectToAction("Index");
    }
} 