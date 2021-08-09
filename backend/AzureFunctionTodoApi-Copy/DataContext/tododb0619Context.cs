using Microsoft.EntityFrameworkCore;
using System;

namespace AzureFunctionTodoApi
{
    public class tododb0619Context : DbContext
    {
        public tododb0619Context()
        { }

        public virtual DbSet<TodoModel> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
