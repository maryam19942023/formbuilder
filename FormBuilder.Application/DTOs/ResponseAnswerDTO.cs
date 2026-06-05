using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class ResponseAnswerDTO
    {
        public int FieldId { get; set; }

        public string FieldLabel { get; set; } = string.Empty;

        public string? Value { get; set; }
    }
}
