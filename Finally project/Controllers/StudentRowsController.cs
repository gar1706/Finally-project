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
    public class StudentRowsController : Controller

    {
        public List<StudentRow> dataRows { get; set; }
        private readonly Finally_projectContext _context;

        public StudentRowsController(Finally_projectContext context)
        {
            _context = context;
        }

        // GET: StudentRows
        public async Task<IActionResult> Index()
        {
            //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");


            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                //_logger = logger;
                dataRows = new List<StudentRow>();

                var dataAccessLayer = new SqlDataAccess();
                var datatable = dataAccessLayer.Execute("SELECT [fname],  [Student].[id] as id  " +
                    " ,[lname]   ,[phone]  ,[email] " +
                    ",Course.title as title ,[address]  FROM  [Course] ," +
                    " [Student] Where [course].id = [Student].course$id");


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

        ////get Users session variable from sessions storage
        //string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");

        ////if users are logged in , then execute code
        //    if (!String.IsNullOrEmpty(usersSession))
        //    {
        //        dataRows = new List<StudentRow>();

        //    var dataAccessLayer = new SqlDataAccess();
        //    var datatable = dataAccessLayer.Execute("SELECT  * FROM[Student]");


        //foreach (DataRow item in datatable.Rows)
        //        {
        //            dataRows.Add(prepareData(item));
        //        }
        //          ViewData["Row"] = dataRows;
        //            return View();
        //        }
        //    else {

        //     //if the users are not logged in redirect to log in page

        //      return RedirectToAction("index", "Users");
        //     }

        //    }



        public StudentRow prepareData(DataRow row)
        {


            var studentRow = new StudentRow()
            {
                Id = int.Parse(row["id"].ToString()),
                lname = row["lname"].ToString(),
                fname = row["fname"].ToString(),
                courseTitle = row["title"].ToString(),
                address = row["address"].ToString(),
                email = row["email"].ToString(),
                phone = row["phone"].ToString()
            };

            return studentRow;

        }
        //         {
        //    var Student = new StudentRow()
        //    {
        //        lname = row["lname"].ToString(),
        //        fname = row["fname"].ToString(),
        //        Id = int.Parse(row["id"].ToString()),
        //        email = row["email"].ToString(),
        //        phone = row["phone"].ToString(),
        //        courseTitle= row["courseTitle"].ToString(),
        //       /// course.id = row["course.id"].ToString(),
        //        address = row["address"].ToString()
        //    };

        //    return Student;

        //}




        //method to redirect to home
        public IActionResult Home()
        {
            return RedirectToAction("index", "StudentRows");
        }


        // GET: StudentRows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentRow == null)
            {
                return NotFound();
            }

            var studentRow = await _context.StudentRow
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentRow == null)
            {
                return NotFound();
            }

            return View(studentRow);
        }


        //if (id == null || _context.StudentrRow == null)
        //{
        //   return NotFound();
        //}

        //var StudentRow = await _context.StudentRow
        //   .FirstOrDefaultAsync(m => m.Id == id);
        //if (StudentRow == null)
        //{
        //           return NotFound();
        //}

        //return View(StudentRow);
        //}


        // GET: StudentRows/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudentRows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,phone,address,email,courseTitle,fname,lname")] StudentRow studentRow)
        {
            if (ModelState.IsValid)
            {

                var datalayer = new SqlDataAccess();

                var sql = prepareDataForInsert(studentRow);

                var response = datalayer.ExecuteNonQuery(sql);



                return RedirectToAction(nameof(Index));
            }
            return View(studentRow);
        }

        public string prepareDataForInsert(StudentRow studentRow)
        {
            string sqlQuery = $@"INSERT INTO [dbo].[Student] ( [fname],[lname] ,[email] ,[phone] ,[address] ,[course$id])  VALUES  " +
                $@"  ( '{studentRow.fname}' ,'{studentRow.lname}' , ' {studentRow.email}' ,'{studentRow.phone}','{studentRow.address}' ,'{studentRow.courseTitle}') ";

            return sqlQuery;

        }

        public string prepareDataForUPdate(StudentRow studentRow)
        {
            string sqlQuery = $@"UPDATE [dbo].[student]
                    SET  [fname]= '{studentRow.fname}'
                        ,[lname] ='{studentRow.lname}'
                        ,[email] = '{studentRow.email}'
                        ,[phone] = '{studentRow.phone}'
                        ,[course$id] ='{studentRow.courseTitle}'
                    Where id = {studentRow.Id}";

            return sqlQuery;

        }

        public string getRowData(int id)
        {
            string sqlQuery = $@"SELECT [fname], [Student].[id] as id ,[lname]  ,[phone]  ,[email]   ,Student.course$id as title ,[address] 
                    FROM  [Course] , [Student]
                    Where [course].id = [Student].course$id AND  [Student].[id]='{id}'";

             

            return sqlQuery;

        }
        // GET: StudentRows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var dataAccessLayer = new SqlDataAccess();
            var datatable = dataAccessLayer.Execute(this.getRowData(int.Parse(id.ToString())));
            StudentRow studentRow = new StudentRow();

            foreach (DataRow item in datatable.Rows)
            {

                studentRow = prepareData(item);
                ViewData["Row"] = studentRow;

            }



            if (id == null || _context.ProfessorRow == null)
            {
                return NotFound();
            }


            if (studentRow == null)
            {
                return NotFound();
            }


            return View(studentRow);
        }

        // POST: StudentRows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,phone,address,email,courseTitle,fname,lname")] StudentRow studentRow)
        {
            //get Users session variable from sessions storage
            string? usersSession = HttpContext.Session.GetString("UserIsLoggedIn");



            //if users are logged in , then execute code
            if (!String.IsNullOrEmpty(usersSession))
            {
                if (ModelState.IsValid)
                {


                    var datalayer = new SqlDataAccess();

                    var sql = prepareDataForUPdate(studentRow);

                    var response = datalayer.ExecuteNonQuery(sql);



                    return RedirectToAction(nameof(Index));

                }
                return View(studentRow);
            }
            else
            {

                //if the users are not logged in redirect to log in page

                return RedirectToAction("index", "Users");

            }
            return View(studentRow);
        }

        // GET: StudentRows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentRow == null)
            {
                return NotFound();
            }

            var studentRow = await _context.StudentRow
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentRow == null)
            {
                return NotFound();
            }

            return View(studentRow);
        }

        // POST: StudentRows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentRow == null)
            {
                return Problem("Entity set 'Finally_projectContext.StudentRow'  is null.");
            }
            var studentRow = await _context.StudentRow.FindAsync(id);
            if (studentRow != null)
            {
                _context.StudentRow.Remove(studentRow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentRowExists(int id)
        {
            return (_context.StudentRow?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
