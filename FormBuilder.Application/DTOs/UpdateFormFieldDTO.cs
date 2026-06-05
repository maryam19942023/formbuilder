using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FormBuilder.Domain.Enum;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class UpdateFormFieldDTO
    {
        [Required]
        [StringLength(100)]
        public string Label { get; set; }

        [Required]
        public FieldType FieldType { get; set; }

        public bool Required { get; set; }

        [StringLength(200)]
        public string? Placeholder { get; set; }

        public string? DefaultValue { get; set; }

        [Range(1, int.MaxValue)]
        public int Order { get; set; }
    }
}
