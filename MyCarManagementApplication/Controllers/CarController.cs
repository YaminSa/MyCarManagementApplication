using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using MyCarManagementApplication.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCarManagementApplication.Data;

namespace MyCarManagementApplication.Controllers
{
    public class CarController : Controller
    {
        private readonly CarDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(CarDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Car
        public IActionResult Index(string searchString, string sort)
        {
            var cars = from c in _context.Cars select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Make.Contains(searchString) || c.Model.Contains(searchString));
            }

            cars = sort switch
            {
                "price" => cars.OrderBy(c => c.Price),
                "year" => cars.OrderBy(c => c.Year),
                "make" => cars.OrderBy(c => c.Make),
                _ => cars
            };

            return View(cars.ToList());
        }

        // GET: Car/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Car/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.ImageFile != null)
                {
                    car.ImagePath = await SaveImageAsync(car.ImageFile);
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Car/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCar = await _context.Cars.FindAsync(id);
                    if (existingCar == null)
                    {
                        return NotFound();
                    }

                    existingCar.Make = car.Make;
                    existingCar.Model = car.Model;
                    existingCar.Year = car.Year;
                    existingCar.Price = car.Price;

                    if (car.ImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingCar.ImagePath))
                        {
                            DeleteImage(existingCar.ImagePath);
                        }
                        existingCar.ImagePath = await SaveImageAsync(car.ImageFile);
                    }

                    _context.Update(existingCar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cars.Any(e => e.Id == car.Id))
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
            return View(car);
        }

        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                if (!string.IsNullOrEmpty(car.ImagePath))
                {
                    DeleteImage(car.ImagePath);
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Cars");
            Directory.CreateDirectory(uploadsFolder); 

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/Images/Cars/{uniqueFileName}";
        }

        private void DeleteImage(string imagePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
