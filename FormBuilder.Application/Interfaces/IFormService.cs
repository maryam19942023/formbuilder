using FormBuilder.Application.DTOs;
using FormBuilder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.Interfaces
{
    public interface IFormService
    {
        Task<int> CreateAsync(CreateFormDTO dto);
        Task<List<FormDTO>> GetAllAsync(bool? isactive, int page = 1, int pagesize = 10);
        Task<FormDTO?> GetByIdAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
        Task SubmitAnswerAsync(SubmitAnswerDTO dto);
        Task<List<FormResponseDTO>> GetResponsesAsync( int formId, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10);
        Task<int> AddFieldAsync(int formId, CreateFormFieldDTO dto);

        Task<bool> UpdateFieldAsync(int formId,int fieldId, UpdateFormFieldDTO dto);
        Task<bool> DeleteFieldAsync(int formId, int fieldId);

        
    }
}
