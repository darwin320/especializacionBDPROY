using IntroASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace IntroASP.Controllers
{
    public class BrandController : Controller
    {

        private readonly NetcoreContext _context;

        // lo injectamos en program.cs y por eso podemos obtenerlo en este constructor
        public BrandController(NetcoreContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // podemos imaginar que __context es la base de datos, brands es la tabla y tolist trae una lista de objetos de 
            // tipo brans
            return View(await _context.Brands.ToListAsync());
        }
    }
}
