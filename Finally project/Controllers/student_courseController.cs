using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finally_project.Data;
using Finally_project.Models;
using System.Data;

namespace Finally_project.Controllers
{
    public class student_courseController : Controller
    {
        private readonly Finally_projectContext _context;
        private List<student_course> dataRows;

        public student_courseController(Finally_projectContext context)
        {
            _context = context;
        }

        // GET: student_course
        public async Task<IActionResult> Index()
        { //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                //_logger = logger;
                dataRows = new List<student_course>();

                var dataAccessLayer = new SqlDataAccess();
                var datatable = dataAccessLayer.Execute("[dbo].[Students_Course]");


                foreach (DataRow item in datatable.Rows)
                {
                    dataRows.Add(prepareData(item));
                }

                ViewData["Row"] = dataRows;

                return View();
            }
            else
            {

                //if the users are not logged in redirect to log in page

                return RedirectToAction("index", "Users");

            }
        }



        public student_course prepareData(DataRow row)
        {


            var studentcourse = new student_course()
            {
                Id = int.Parse(row["id"].ToString()),
                lname = row["lname"].ToString(),
                fname = row["fname"].ToString(),
                courseTitle = row["Course_title"].ToString(),
                ModuleTitle = row["Module_title"].ToString(),
                grade = row["grade"].ToString() 
            };

            return studentcourse;

        }

        // GET: student_course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.student_course == null)
            {
                return NotFound();
            }

            var student_course = await _context.student_course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student_course == null)
            {
                return NotFound();
            }

            return View(student_course);
        }

        // GET: student_course/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: student_course/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,grade,ModuleTitle,courseTitle,fname,lname")] student_course student_course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student_course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student_course);
        }

        // GET: student_course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.student_course == null)
            {
                return NotFound();
            }

            var student_course = await _context.student_course.FindAsync(id);
            if (student_course == null)
            {
                return NotFound();
            }
            return View(student_course);
        }

        // POST: student_course/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,grade,ModuleTitle,courseTitle,fname,lname")] student_course student_course)
        {
            if (id != student_course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student_course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!student_courseExists(student_course.Id))
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
            return View(student_course);
        }

        // GET: student_course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.student_course == null)
            {
                return NotFound();
            }

            var student_course = await _context.student_course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student_course == null)
            {
                return NotFound();
            }

            return View(student_course);
        }

        // POST: student_course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.student_course == null)
            {
                return Problem("Entity set 'Finally_projectContext.student_course'  is null.");
            }
            var student_course = await _context.student_course.FindAsync(id);
            if (student_course != null)
            {
                _context.student_course.Remove(student_course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool student_courseExists(int id)
        {
          return (_context.student_course?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
