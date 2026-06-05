using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace FormBuilder.Application.DTOs
{
    public class CreateFormDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CreateFormFieldDTO> Fields { get; set; } = new();
    }
}
