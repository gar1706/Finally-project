﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finally_project.Data;
using Finally_project.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

using System;
using System.Security.Cryptography;
using System.Text;

namespace Finally_project.Controllers
{
    public class UsersController : Controller
    {
        private readonly Finally_projectContext _context;

        public UsersController(Finally_projectContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
                    return View();
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {


            return View();
        }



        //log out method to destroy session variable

        public IActionResult logout()
        {

            //delete session variable
            HttpContext.Session.Remove("UserIsLoggedIn");

            //rediret to login page

            return RedirectToAction("index", "Users");
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] User user)
        {
            
                    var datalayer = new SqlDataAccess();

            //hash method and encrypt password

                    user.Password = HashPasword(user.Password); 


                       
                    var sql = prepareDataForInsert(user);

                    var response = datalayer.ExecuteNonQuery(sql);



                    return RedirectToAction(nameof(Index));
           
          
        }


        public string prepareDataForInsert(User user)
        {
            string sqlQuery = $@"INSERT INTO [dbo].[Users] ( [email] ,[password],[name])  VALUES  " +
                $@"  ( '{user.Email}' , ' {user.Password}', ' {user.Name}'   ) ";

            return sqlQuery;


        }




       public   string HashPasword(string password)
        {

            const int keySize = 20;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

           

            var salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password.Trim()),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            //return Convert.ToHexString(hash).ToString().Trim();

            return password;
        }

      

        bool VerifyPassword(string password, string hash)
        {


            try
            {
                const int keySize = 20;
                const int iterations = 350000;
                var salt = RandomNumberGenerator.GetBytes(keySize);
                HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
                byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

                var hashText = Convert.ToHexString(hashToCompare).ToString();

                byte[] hashset = Convert.FromHexString(hash.Trim());

                bool result =  hashToCompare.Equals(hashset);

               
                return true;
          
             
            }
            catch (Exception ex)
            {

                return false;
            }
         
        }


        public async Task<IActionResult> login(string password,string email)
        {
            if (password != null)
            {
                var datalayer = new SqlDataAccess();

                var sql = $"select * from Users where Email='{email}' ";

                var response =  datalayer.Execute(sql);

                var userpassword = "";

                foreach (DataRow item in response.Rows)
                {
                    userpassword = item["password"].ToString();



                }


                bool passwordsmatch = VerifyPassword(password, userpassword);       
                if (userpassword.Trim() == password.Trim())
                {
                    // /Create session variable
                    HttpContext.Session.SetString("UserIsLoggedIn", email.ToString() ); 
                    return RedirectToAction("index", "Home");
                }
                else
                {

                    this.Index();
                }

                return RedirectToAction("index", "Users");
            }



            /// var user = await _context.User.FindAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}

            return RedirectToAction("index", "Users");

        }




        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Finally_projectContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
