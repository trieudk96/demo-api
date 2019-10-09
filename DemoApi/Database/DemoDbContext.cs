using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApi.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Database
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options)
        {
            DataGenerator.Initialize(this);
        }
        public DbSet<User> Users { get; set; }
    }

    public class DataGenerator
    {
        public static void Initialize(DemoDbContext context)
        {
            if (context.Users.Any()) return;
            var users = new List<User>();
            for (var i = 0; i < 50; i++)
            {
                users.Add(new User() { Id = i + 1, Age = 10 + i, Name = "name abc" + i, UserName = "abc_" + i, Gender = i % 2 == 0 ? "Name" : "Nữ" });
            }
            context.Users.AddRange(users);
            context.SaveChanges();
        }

    }
}
