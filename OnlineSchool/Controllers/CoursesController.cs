using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Models;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineSchool.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // ДОБАВЛЕНИЕ КУРСОВ
        public async Task<IActionResult> AddCourses()
        {
            return _context.Courses != null ?
                        View(await _context.Courses.ToListAsync()) :
                        Problem("Entity set 'SchoolContext.Courses'  is null.");
        }

        // ПРОСМОТР КУРСОВ
        public async Task<IActionResult> GetCourses()
        {
            return _context.Courses != null ?
                        View(await _context.Courses.ToListAsync()) :
                        Problem("Entity set 'SchoolContext.Courses'  is null.");
        }




        // GET: Courses
        public async Task<IActionResult> Index()
        {
              return _context.Courses != null ? 
                          View(await _context.Courses.ToListAsync()) :
                          Problem("Entity set 'SchoolContext.Courses'  is null.");
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // СОЗДАНИЕ КУРСА
        // GET: Courses/Create
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        // СОЗДАНИЕ КУРСА
        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("Id,Title,Description,PhotoPath")] Course course)
        {
            if (ModelState.IsValid)
            {
                var request = HttpContext.Request;
                IFormFileCollection files = request.Form.Files;
                foreach (var file in files)
                {
                    Directory.CreateDirectory("Photos");
                    string photoName = course.Title! + Path.GetExtension(file.FileName);
                    string uploadPath = Path.Combine("Photos", photoName);
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    course.PhotoPath = uploadPath;

                }
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddCourses));
            }
            return View(course);
        }

        // РЕДАКТИРОВАНИЕ КУРСА
        // GET: Courses/Edit/5
        public async Task<IActionResult> EditCourse(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // РЕДАКТИРОВАНИЕ КУРСА
        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, [Bind("Id,Title,Description,PhotoPath")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AddCourses));
            }
            return View(course);
        }

        // УДАЛЕНИЕ КУРСА
        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // УДАЛЕНИЕ КУРСА
        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }           
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course != null)
            {
                string photoPath = course.PhotoPath!;
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                FileInfo file = new FileInfo(photoPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            return RedirectToAction(nameof(AddCourses));
        }

        private bool CourseExists(int id)
        {
          return (_context.Courses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // КУРС
        public async Task<IActionResult> WatchCourse(int id)
        {
            ViewData["CourseId"] = id;
            ViewData["Title"] = (_context.Courses.FirstOrDefault(c => c.Id == id))?.Title;
            var lessons = _context.Lessons.Where(les => les.Course.Id == id).Select(les => les);
            
            return lessons != null ?
                        View(await lessons.ToListAsync()) :
                        Problem("Entity set 'SchoolContext.Lessons'  is null.");
        }

    }
}
