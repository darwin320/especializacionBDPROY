using IntroASP.Models;
using IntroASP.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntroASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIBeerController : ControllerBase
    {

        private readonly NetcoreContext _context;

        public APIBeerController(NetcoreContext netcoreContext)
        {
            _context = netcoreContext;
        }

        public async Task<List<BeerBrandViewModel>> Get()
            => await _context.Beers.Include(b=>b)
            .Select(b=>new BeerBrandViewModel
            {
                Id = b.BeerId,
                Name = b.Name,
                Brand = b.Brand.Name
            })
            .ToListAsync();
    }
}
