using System;
using System.Collections.Generic;
using System.Text;
using FormBuilder.Domain.Enum;

namespace FormBuilder.Application.DTOs
{
    public class FormFieldDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public FieldType FieldType { get; set; }

        public bool Required { get; set; }

        public string? Placeholder { get; set; }

        public string? DefaultValue { get; set; }

        public int Order { get; set; }
    }
}
