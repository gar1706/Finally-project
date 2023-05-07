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
using System.Runtime.InteropServices.ObjectiveC;

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
                var datatable = dataAccessLayer.Execute(" SELECT M.[id] as id   ,[course$id], M.[title] As ModuleTitle, C.[title] AS CourseTitle,[hours] FROM[Module] AS M, [Course] AS C where M.course$id = TRIM(C.id)");


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
                courseId = row["course$id"].ToString(),
                ModuleTitle = row["ModuleTitle"].ToString(),
                courseTitle = row["CourseTitle"].ToString(),
                Id = int.Parse(row["id"].ToString()),
                hours = row["hours"].ToString()

            };

            return module;

        }
        //method to redirect to home
        public IActionResult Home()
        {
            return RedirectToAction("index", "Modules");
        }




        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
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
        public async Task<IActionResult> Create([Bind("Id,courseTitle,ModuleTitle,hours,courseId")] Module module)
        {
            if (ModelState.IsValid)
            {

                var datalayer = new SqlDataAccess();


                if (module.courseId == null)
                {
                    var sql = prepareDataForInsert(module);

                    var response = datalayer.ExecuteNonQuery(sql);
                }
                else
                {
                    var sql = prepareDataForInsert(module);

                    var courseSql = insertIntoCourseTableSqlQuery(module);

                    var response = datalayer.ExecuteNonQuery(courseSql);
                   var  response2 = datalayer.ExecuteNonQuery(sql);


                }

                  
                return RedirectToAction(nameof(Index));
            }
            return View(module);
        }

        public string prepareDataForInsert(Module module)
        {


            string sqlQuery = $"";

            //insert in the table Course

            sqlQuery = $@"INSERT INTO [dbo].[Module] (   [title] ,[hours] ,[course$id])  
                                VALUES    (  '{module.ModuleTitle}' ,'{module.hours}' ,'{module.courseId}') ";



            return sqlQuery;

        }



        public string insertIntoCourseTableSqlQuery(Module module)
        {
            //insert into course table

            string sql = $@"  INSERT INTO [dbo].[Course](   [id],   [title] ) 
                            VALUES(' {module.courseId}', '{module.courseTitle}');";

            return sql;

        }

        public string prepareDataForUPdate(Module module)
        {
            string sqlQuery = $@"UPDATE [dbo].[Module]
                    SET  [title] ='{module.ModuleTitle}'
                        ,[hours] = '{module.hours}'
                        ,[course$id] ='{module.courseTitle}'
                    Where id = {module.Id}";

            return sqlQuery;

        }

        public string getRowData(int id)
        {

            string sqlQuery = $@"SELECT [Module].[id] as id,[course$id], [title]  ,[hours] 
                    FROM  [Module]  
                    Where [Module].[id]='{id}'";



            return sqlQuery;

        }


        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var dataAccessLayer = new SqlDataAccess();
            var datatable = dataAccessLayer.Execute(this.getRowData(int.Parse(id.ToString())));
            Module module = new Module();

            foreach (DataRow item in datatable.Rows)
            {

                module = prepareData(item);
                ViewData["Row"] = module;

            }

            if (id == null || _context.Module == null)
            {
                return NotFound();
            }


            if (module == null)
            {
                return NotFound();
            }


            return View(module);

        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,courseTitle,ModuleTitle,hours")] Module module)
        {
            //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                 

                    var datalayer = new SqlDataAccess();

                    var sql = prepareDataForUPdate(module);

                    var response = datalayer.ExecuteNonQuery(sql);



                    return RedirectToAction(nameof(Index));

              
            }
            else
            {

                //if the users are not logged in redirect to log in page

                return RedirectToAction("index", "Users");

            }
            return View(module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
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
            var module = await _context.Module.FindAsync(id);
            if (module != null)
            {
                _context.Module.Remove(module);
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
