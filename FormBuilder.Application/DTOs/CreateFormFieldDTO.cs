using FormBuilder.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class CreateFormFieldDTO
    {
        [Required]
        [StringLength(100)]
        public string Label { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(FieldType))]
        public FieldType FieldType { get; set; }

        public bool Required { get; set; }

        [StringLength(200)]
        public string? Placeholder { get; set; }

        public string? DefaultValue { get; set; }

        [Range(1, 100)]
        public int Order { get; set; } = 1;
    }

}
