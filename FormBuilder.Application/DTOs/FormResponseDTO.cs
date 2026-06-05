using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class FormResponseDTO
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ResponseAnswerDTO> Answers { get; set; } = new();
    }

}
