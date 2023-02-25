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
    public class ModulesController : Controller
    {
        public List<Module> dataRows { get; set; }
        private readonly Finally_projectContext _context;

        public ModulesController(Finally_projectContext context)
        {
            _context = context;
        }


        // GET: Modules
        public async Task<IActionResult> Index()
        {

            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                //_logger = logger;
                dataRows = new List<Module>();

                var dataAccessLayer = new SqlDataAccess();
                var datatable = dataAccessLayer.Execute("SELECT [id] " +
                    " ,[course$id]   ,[title]  ,[hours] FROM  [Module]");


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


        public Module prepareData(DataRow row)
        {


            var module = new Module()
            {
                courseTitle = row["course$id"].ToString(),
                title = row["title"].ToString(),
                Id = int.Parse(row["id"].ToString()),
                hours = row["hours"].ToString()

            };

            return module;

        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,courseTitle,title,hours")] Module @module)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }



        public string getRowData(int id)
        {
            
            string sqlQuery = $@"SELECT [id]  
                   ,[course$id]   ,[title]  ,[hours] 
                    FROM  [Module]  
                    Where  [Module].[id]='{id}'";



            return sqlQuery;

        }


        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {



            var dataAccessLayer = new SqlDataAccess();
            var datatable = dataAccessLayer.Execute(this.getRowData(int.Parse(id.ToString())));

            Module moduleRow = new Module();


            foreach (DataRow item in datatable.Rows)
            {

                moduleRow = prepareData(item);
                ViewData["Row"] = moduleRow;

            }

            if (id == null || _context.Module== null)
            {
                return NotFound();
            }


            if (moduleRow == null)
            {
                return NotFound();
            }


            return View(moduleRow);
        
    }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,courseTitle,title,hours")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
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
            return View(@module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Module == null)
            {
                return Problem("Entity set 'Finally_projectContext.Module'  is null.");
            }
            var @module = await _context.Module.FindAsync(id);
            if (@module != null)
            {
                _context.Module.Remove(@module);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return (_context.Module?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
