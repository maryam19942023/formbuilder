using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Domain.Entities
{
    public class Form
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        


        public ICollection<FormFields> Fields { get; set; } = new List<FormFields>();

        public ICollection<FormAnswers> Answers { get; set; } = new List<FormAnswers>();
    }
}
