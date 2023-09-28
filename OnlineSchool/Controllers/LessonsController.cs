using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Models;

namespace OnlineSchool.Controllers
{
    public class LessonsController : Controller
    {
        private readonly SchoolContext _context;

        public LessonsController(SchoolContext context)
        {
            _context = context;
        }

        // ДОБАВЛЕНИЕ УРОКОВ
        public async Task<IActionResult> AddLessons(int id)
        {
            ViewData["CourseId"] = id;
            var lessons = _context.Lessons.Where(les => les.Course.Id == id).Select(les => les);
            return lessons != null ?
                        View(await lessons.ToListAsync()) :
                        Problem("Entity set 'SchoolContext.Lessons'  is null.");
        }

        // ПРОСМОТР УРОКОВ
        public async Task<IActionResult> GetLessons(int courseId)
        {
            var lessons = _context.Lessons.Where(les => les.Course.Id == courseId).Select(les => les);
            return lessons != null ?
                        View(await lessons.ToListAsync()) :
                        Problem("Entity set 'SchoolContext.Lessons'  is null.");
        }


        // GET: Lessons
        public async Task<IActionResult> Index()
        {
              return _context.Lessons != null ? 
                          View(await _context.Lessons.ToListAsync()) :
                          Problem("Entity set 'SchoolContext.Lessons'  is null.");
        }

        // GET: Lessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // СОЗДАНИЕ УРОКА
        // GET: Lessons/Create
        [HttpGet]
        public IActionResult CreateLesson(int id)
        {
            ViewData["CourseId"] = id;
            return View();
        }

        // СОЗДАНИЕ УРОКА
        // POST: Lessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(int id, [Bind("Id,Title,Description,VideoLink")] Lesson lesson)
        {
            Course? course = _context.Courses.ToList().FirstOrDefault(course => course.Id == id);
            var lessons = _context.Lessons.ToList().Where(les => les.Course == course).Select(les => les);
            if (ModelState.IsValid && course != null)
            {
                lesson.Id = default;
                lesson.Course = course;
                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                var dict = new RouteValueDictionary();
                dict["id"] = id;
                return RedirectToAction(nameof(AddLessons), dict);
            }
            
            return View(lesson);
        }

        // РЕДАКТИРОВАНИЕ УРОКА
        // GET: Lessons/Edit/5
        public async Task<IActionResult> EditLesson(int? id, int? courseid)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons.FindAsync(id);
            //var course = await _context.Courses.Where(c => c.Id).Select(c => c);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = courseid;
            return View(lesson);
        }

        // РЕДАКТИРОВАНИЕ УРОКА
        // POST: Lessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int? id, int? courseid, [Bind("Id,Title,Description,VideoLink")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var dict = new RouteValueDictionary();
                dict["id"] = _context.Lessons.Where(l => l.Id == lesson.Id).Select(l => l.Course.Id).FirstOrDefault();
                return RedirectToAction(nameof(AddLessons), dict);
            }
            return View(lesson);
        }

        // УДАЛЕНИЕ УРОКА
        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id, int? courseid)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = courseid;
            return View(lesson);
        }

        // УДАЛЕНИЕ УРОКА
        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int? courseid)
        {
            if (_context.Lessons == null)
            {
                return Problem("Entity set 'SchoolContext.Lessons'  is null.");
            }
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
            }
            
            await _context.SaveChangesAsync();
            var dict = new RouteValueDictionary();
            dict["id"] = courseid;
            return RedirectToAction(nameof(AddLessons), dict);
        }

        private bool LessonExists(int id)
        {
          return (_context.Lessons?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // УРОК
        public async Task<IActionResult> WatchLesson(int? id, int? courseid)
        {
            if (id == null || _context.Lessons == null)
            {
                return NotFound();
            }
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = courseid;
            return View(lesson);
        }

    }
}
