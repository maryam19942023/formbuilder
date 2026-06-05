using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Domain.Entities
{
    public class FormAnswerDetail
    {
        public int Id { get; set; }

        public int FormAnswerId { get; set; }
        public FormAnswers FormAnswer { get; set; }

        public int FormFieldId { get; set; }
        public FormFields FormField { get; set; }

        public string? Value { get; set; }

    }
}
