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
    public class Professors_hoursController : Controller
    {
        private readonly Finally_projectContext _context;
        private List<Professors_hours> dataRows;

        public Professors_hoursController(Finally_projectContext context)
        {
            _context = context;
        }

        // GET: Professors_hours
        // GET: student_course
        public async Task<IActionResult> Index()
        { //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                //_logger = logger;
                dataRows = new List<Professors_hours>();

                var dataAccessLayer = new SqlDataAccess();
                var datatable = dataAccessLayer.Execute("[dbo].[Professors]");


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



        public Professors_hours prepareData(DataRow row)
        {


            var Professors_hours = new Professors_hours()
            {
                Id = int.Parse(row["id"].ToString()),
                lname = row["lname"].ToString(),
                fname = row["fname"].ToString(),
                courseTitle = row["Course_title"].ToString(),
                ModuleTitle = row["Module_title"].ToString(),
                hours = row["hours"].ToString()
            };

            return Professors_hours;

        }


        // GET: Professors_hours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Professors_hours == null)
            {
                return NotFound();
            }

            var professors_hours = await _context.Professors_hours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (professors_hours == null)
            {
                return NotFound();
            }

            return View(professors_hours);
        }

        // GET: Professors_hours/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Professors_hours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,hours,ModuleTitle,courseTitle,fname,lname")] Professors_hours professors_hours)
        {
            if (ModelState.IsValid)
            {
                _context.Add(professors_hours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(professors_hours);
        }

        // GET: Professors_hours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Professors_hours == null)
            {
                return NotFound();
            }

            var professors_hours = await _context.Professors_hours.FindAsync(id);
            if (professors_hours == null)
            {
                return NotFound();
            }
            return View(professors_hours);
        }

        // POST: Professors_hours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,hours,ModuleTitle,courseTitle,fname,lname")] Professors_hours professors_hours)
        {
            if (id != professors_hours.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professors_hours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Professors_hoursExists(professors_hours.Id))
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
            return View(professors_hours);
        }

        // GET: Professors_hours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Professors_hours == null)
            {
                return NotFound();
            }

            var professors_hours = await _context.Professors_hours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (professors_hours == null)
            {
                return NotFound();
            }

            return View(professors_hours);
        }

        // POST: Professors_hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Professors_hours == null)
            {
                return Problem("Entity set 'Finally_projectContext.Professors_hours'  is null.");
            }
            var professors_hours = await _context.Professors_hours.FindAsync(id);
            if (professors_hours != null)
            {
                _context.Professors_hours.Remove(professors_hours);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Professors_hoursExists(int id)
        {
          return (_context.Professors_hours?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
