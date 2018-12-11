using System.Linq;
using AspNetWebApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace AspNetWebApi.Controllers
{
    public class TestController : Controller
    {
        private readonly AnimalContext _context;

        public TestController(AnimalContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var animals = _context.Animals.Select(a => new Animal
                {
                    Name = a.Name,
                    LivesIn = new Country
                    {
                        Name = a.LivesIn.Name
                    }
                })
                .ToList();

            return View(animals);
        }

        public IActionResult Countries()
        {
            var countries = _context.Countries.Select(c => new Country
                {
                    Name = c.Name,
                    Animals = c.Animals.Select(a => new Animal
                    {
                        Name = a.Name
                    }).ToList()
                });

            return View(countries);
        }
    }
}