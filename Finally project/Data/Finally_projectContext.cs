using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Finally_project.Models;

namespace Finally_project.Data
{
    public class Finally_projectContext : DbContext
    {
        public Finally_projectContext (DbContextOptions<Finally_projectContext> options)
            : base(options)
        {
        }

        public DbSet<Finally_project.Models.ProfessorRow> ProfessorRow { get; set; } = default!;

        public DbSet<Finally_project.Models.StudentRow> StudentRow { get; set; } = default!;

        public DbSet<Finally_project.Models.User> User { get; set; } = default!;

        public DbSet<Finally_project.Models.student_course> student_course { get; set; } = default!;

        public DbSet<Finally_project.Models.Professors_hours> Professors_hours { get; set; } = default!;

      
    }
}
