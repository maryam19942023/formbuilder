using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FormBuilder.Infrastructure
{
    public class FormBuilderDbContextFactory
        : IDesignTimeDbContextFactory<FormBuilderDbContext>
    {
        public FormBuilderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FormBuilderDbContext>();

            optionsBuilder.UseSqlite("Data Source=Data/FormBuilder.db");

            return new FormBuilderDbContext(optionsBuilder.Options);
        }
    }
}
