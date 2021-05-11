using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodosApplication.Model;

namespace TodosApplication.DAL
{
    public class TodoContext : DbContext , IDbContext
    {

        public DbSet<Todo> Todo { get; set; }
        public DbSet<TodoType> TodoTypes { get; set; }
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Todo>().HasKey(t => t.Id);
            modelBuilder.Entity<Todo>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Todo>()
                .HasOne<TodoType>(t => t.Type)
                .WithMany()
                .HasForeignKey(tt => tt.TypeId)
                .HasPrincipalKey(a => a.Id);




            modelBuilder.Entity<TodoType>().HasKey(t => t.Id);
            modelBuilder.Entity<TodoType>().Property(t => t.Id).ValueGeneratedOnAdd();


            modelBuilder.Entity<TodoType>(entity =>
            {
                entity.HasData(
                        new TodoType { Id = 1, Name = "Függőben" , Order = 1 },
                        new TodoType { Id = 2, Name = "Folyamatban", Order = 2 },
                        new TodoType { Id = 3, Name = "Kész", Order = 3 },
                        new TodoType { Id = 4, Name = "Elhalasztva", Order = 4 }

                    );

            });


            modelBuilder.Entity<Todo>(entity =>
            {
                entity.HasData(
                        new Todo { Id = 1, Name = "Teszt", Deadline = DateTime.Now, Details = "Csak egy teszt", Order = 1, TypeId = 1 }

                    );

            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                throw new System.Exception("DbContext not configured");

            base.OnConfiguring(optionsBuilder);
        }

    }
}
