using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Domain.Entities
{
    public class FormAnswers
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FormAnswerDetail> AnswerDetails { get; set; } = new List<FormAnswerDetail>();
    }
}
