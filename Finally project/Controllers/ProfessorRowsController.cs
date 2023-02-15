using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finally_project.Data;
using Finally_project.Models;
using System.Data;
using System.Web;
using System.Configuration;

namespace Finally_project.Controllers
{
    public class ProfessorRowsController : Controller
    {
        public List<ProfessorRow> dataRows { get; set; }
        private readonly Finally_projectContext _context;

        public ProfessorRowsController(Finally_projectContext context)
        {
            _context = context;
        }

        // GET: ProfessorRows
        public async Task<IActionResult> Index()
        {

            //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                dataRows = new List<ProfessorRow>();

                var dataAccessLayer = new SqlDataAccess();
                var datatable = dataAccessLayer.Execute("SELECT  * FROM[Professor]");


                foreach (DataRow item in datatable.Rows)
                {
                    dataRows.Add(prepareData(item));
                }

                ViewData["Row"] = dataRows;


                return View();



            }
            else {

                //if the users are not logged in redirect to log in page

                return RedirectToAction("index", "Users");

            }

         


        }
        public ProfessorRow prepareData(DataRow row)
        {


            var professor = new ProfessorRow()
            {
                lname = row["lname"].ToString(),
                fname = row["fname"].ToString(),
           
                email = row["email"].ToString(),
                phone = row["phone"].ToString()
            };

            return professor;

        }

        // GET: ProfessorRows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProfessorRow == null)
            {
                return NotFound();
            }

            var professorRow = await _context.ProfessorRow
                .FirstOrDefaultAsync(m => m.Id == id);
            if (professorRow == null)
            {
                return NotFound();
            }

            return View(professorRow);
        }

        // GET: ProfessorRows/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProfessorRows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,phone,email,fname,lname")] ProfessorRow professorRow)
        {
            //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                if (ModelState.IsValid)
                {


                    var datalayer = new SqlDataAccess();

                    var sql = prepareDataForInsert(professorRow);

                    var response = datalayer.ExecuteNonQuery(sql);



                    return RedirectToAction(nameof(Index));

                }
                return View(professorRow);
            } else
            {

                //if the users are not logged in redirect to log in page

                return RedirectToAction("index", "Users");

            }
        }
        

        public string prepareDataForInsert(ProfessorRow professorRow) 
        {
            string sqlQuery = $@"INSERT INTO [dbo].[Professor] ( [fname],[lname] ,[email] ,[phone])  VALUES  " +
                $@"  ( '{professorRow.fname}' ,'{ professorRow.lname}' , ' {professorRow.email}' ,'{professorRow.phone}' ) ";

            return sqlQuery;
        
        }

        // GET: ProfessorRows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProfessorRow == null)
            {
                return NotFound();
            }

            var professorRow = await _context.ProfessorRow.FindAsync(id);
            if (professorRow == null)
            {
                return NotFound();
            }
            return View(professorRow);
        }

        // POST: ProfessorRows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,phone,email,fname,lname")] ProfessorRow professorRow)
        {
            if (id != professorRow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professorRow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorRowExists(professorRow.Id))
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
            return View(professorRow);
        }

        // GET: ProfessorRows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProfessorRow == null)
            {
                return NotFound();
            }

            var professorRow = await _context.ProfessorRow
                .FirstOrDefaultAsync(m => m.Id == id);
            if (professorRow == null)
            {
                return NotFound();
            }

            return View(professorRow);
        }

        // POST: ProfessorRows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProfessorRow == null)
            {
                return Problem("Entity set 'Finally_projectContext.ProfessorRow'  is null.");
            }
            var professorRow = await _context.ProfessorRow.FindAsync(id);
            if (professorRow != null)
            {
                _context.ProfessorRow.Remove(professorRow);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessorRowExists(int id)
        {
          return (_context.ProfessorRow?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
