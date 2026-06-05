using FormBuilder.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Domain.Entities
{
    public class FormFields
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        public string Label { get; set; }

        public FieldType FieldType { get; set; }

        public bool Required { get; set; }

        public string? Placeholder { get; set; }

        public string? DefaultValue { get; set; }

        public int Order { get; set; }
        public bool IsDeleted { get; set; } 


        public ICollection<FormAnswerDetail> AnswerDetails { get; set; } = new List<FormAnswerDetail>();
    
}
}
