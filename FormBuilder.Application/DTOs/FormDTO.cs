using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class FormDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<FormFieldDTO> Fields { get; set; } = new();
    }
}
