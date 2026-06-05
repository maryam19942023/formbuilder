using Microsoft.EntityFrameworkCore;
using FormBuilder.Domain.Entities;

namespace FormBuilder.Infrastructure
{
    public class FormBuilderDbContext: DbContext
    {
        public FormBuilderDbContext(DbContextOptions<FormBuilderDbContext> options) : base(options)
        {
        }

        public DbSet<Form> Forms { get; set; }

        public DbSet<FormFields> FormFields { get; set; }

        public DbSet<FormAnswers> FormAnswers { get; set; }
        public DbSet<FormAnswerDetail> FormAnswerDetails { get; set; }

    }
}
