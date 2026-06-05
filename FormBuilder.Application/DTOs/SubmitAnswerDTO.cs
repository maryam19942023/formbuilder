using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.DTOs
{
    public class SubmitAnswerDTO
    {
        public int FormId { get; set; }

        public List<AnswerDTO> Answers { get; set; } = new();
    }
}
