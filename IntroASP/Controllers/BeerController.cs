using IntroASP.Models;
using IntroASP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IntroASP.Controllers
{
    public class BeerController : Controller

    {
        private readonly NetcoreContext _context;

        public BeerController(NetcoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var beers = _context.Beers.Include(b=>b.Brand);
            return View(await beers.ToListAsync());
        }

        public IActionResult Create()
        {
            //necesitamos almacenar al ifnroamcion de lam base de datos sin hacerlo de manera asincrona 
            ViewData["Brands"] = new SelectList(_context.Brands, "BrandId","Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BeerViewModel model)
        {
            if(ModelState.IsValid) {
                var beer = new Beer()
                {
                    Name = model.Name,
                    BrandId = model.BrandId
                };

                _context.Beers.Add(beer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //necesitamos almacenar al ifnroamcion de lam base de datos sin hacerlo de manera asincrona 

            ViewData["Brands"] = new SelectList(_context.Brands, "BrandId", "Name",model.BrandId);
            return View(model);
        }


    }
}
