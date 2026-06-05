using FormBuilder.Application.DTOs;
using FormBuilder.Application.Interfaces;
using FormBuilder.Domain.Entities;
using FormBuilder.Domain.Enum;
using FormBuilder.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormBuilder.Application.Services
{
    public class FormService : IFormService
    {
        private readonly FormBuilderDbContext _context;
        private readonly ILogger<FormService> _logger;

        public FormService(FormBuilderDbContext context, ILogger<FormService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> CreateAsync(CreateFormDTO dto)
        {
           
                if (dto.Fields == null || !dto.Fields.Any())
                    throw new ArgumentException("فرم باید حداقل یک فیلد داشته باشد ");
                var form = new Form
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    Fields = dto.Fields.Select(x => new FormFields
                    {
                        Label = x.Label,
                        FieldType = x.FieldType,
                        Required = x.Required,
                        Placeholder = x.Placeholder,
                        DefaultValue = x.DefaultValue,
                        Order = x.Order,
                    }).ToList()

                };
            try
            {
                _context.Forms.Add(form);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Form created Id={FormId} Name={Name}", form.Id, form.Name);
                return form.Id;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,"FAiled to create form {FormId} Name={Name}", form.Id, form.Name);
                throw;
            }
        }
        public async Task<List<FormDTO>> GetAllAsync(bool? isactive,int page=1, int pagesize=10 )
        {
            if (page < 1)
                page = 1;

            if (pagesize < 1)
                pagesize = 10;
            var content = _context.Forms.AsNoTracking().AsQueryable();


            if (isactive.HasValue)
            {
                 content= content.Where(x => x.IsActive == isactive);
            }
            return await content.Select(f => new FormDTO
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                IsActive = f.IsActive,
                Fields = f.Fields.Where(d=>!d.IsDeleted).OrderBy(x => x.Order).Select(x => new FormFieldDTO
                {
                    Id = x.Id,
                    Label = x.Label,
                    FieldType = x.FieldType,
                    Required = x.Required,
                    Placeholder = x.Placeholder,
                    DefaultValue = x.DefaultValue,
                    Order = x.Order
                }).ToList()}).Skip((page - 1) * pagesize).Take(pagesize).ToListAsync();

        }
        
        public async Task<FormDTO?> GetByIdAsync(int id)
        {
            return await _context.Forms.AsNoTracking().Where(f => f.Id == id).Select(f => new FormDTO
            { 
                 Id = f.Id,
                 Name = f.Name,
                 Description = f.Description,
                 IsActive = f.IsActive,
                 Fields = f.Fields.Where(d=>!d.IsDeleted).OrderBy(x => x.Order).Select(x => new FormFieldDTO
                 {
                     Id = x.Id,
                     Label = x.Label,
                     FieldType = x.FieldType,
                     Required = x.Required,
                     Placeholder= x.Placeholder,
                     DefaultValue= x.DefaultValue,
                     Order = x.Order
                 }).ToList()}).FirstOrDefaultAsync();

            
        }
        public async Task<bool> ChangeStatusAsync(int id)
        {
            var form = await _context.Forms.FirstOrDefaultAsync(x => x.Id == id);
            if (form == null)
                return false;

            form.IsActive = !form.IsActive;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Form status changed Id={FormId} IsActive={IsActive}",  form.Id, form.IsActive);
            return true;
        }
        public async Task SubmitAnswerAsync(SubmitAnswerDTO dto)
        {
            try
            {
                var form = await _context.Forms.FirstOrDefaultAsync(x => x.Id == dto.FormId);

                if (form == null)
                    throw new Exception("فرم پیدا نشد");
                if (!form.IsActive)
                    throw new Exception("فرم غیر فعال است ");

                if (dto.Answers == null || !dto.Answers.Any())
                    throw new Exception("حداقل یک پاسخ باید ارسال شود");

                var formFields = await _context.FormFields.Where(x => x.FormId == dto.FormId && !x.IsDeleted).ToListAsync();

                foreach (var answer in dto.Answers)
                {
                    if (!formFields.Any(f => f.Id == answer.FieldId))
                        throw new Exception("یکی از فیلدها معتبر نیست");
                }
                foreach (var answer in dto.Answers)
                {
                    var field = formFields.First(x => x.Id == answer.FieldId);

                    if (string.IsNullOrWhiteSpace(answer.Value))
                        continue;

                    switch (field.FieldType)
                    {
                        case FieldType.Number:

                            if (!decimal.TryParse(answer.Value, out _))
                                throw new Exception($"مقدار فیلد {field.Label} باید عدد باشد");
                            break;

                        case FieldType.Date:
                            if (!DateTime.TryParse(answer.Value, out _))
                                throw new Exception($"مقدار فیلد {field.Label} باید تاریخ معتبر باشد");
                            break;

                        case FieldType.Checkbox:

                            if (!bool.TryParse(answer.Value, out _))
                                throw new Exception($"مقدار فیلد {field.Label} باید true یا false باشد");
                            break;

                        case FieldType.Text:
                            break;
                        case FieldType.TextArea:
                            break;

                        case FieldType.Select:
                            break;

                        default:
                            throw new Exception($"نوع فیلد {field.FieldType} پشتیبانی نمی‌شود");
                    }
                }
                var requiredFields = formFields.Where(x => x.Required);

                foreach (var field in requiredFields)
                {
                    var answer = dto.Answers.FirstOrDefault(x => x.FieldId == field.Id);

                    if (answer == null || string.IsNullOrWhiteSpace(answer.Value))
                        throw new Exception($"{field.Label} الزامی است");
                }

                var formAnswer = new FormAnswers
                {
                    FormId = dto.FormId
                };

                foreach (var answer in dto.Answers)
                {
                    formAnswer.AnswerDetails.Add(new FormAnswerDetail
                    {
                        FormFieldId = answer.FieldId,
                        Value = answer.Value
                    });
                }

                _context.FormAnswers.Add(formAnswer);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Response submitted FormId={FormId} ResponseId={ResponseId}",dto.FormId,formAnswer.Id);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,  "Error while submitting response for FormId={FormId}", dto.FormId);
                throw;

            }
        }


        public async Task<List<FormResponseDTO>> GetResponsesAsync(int formId, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;
            var formExists = await _context.Forms.AnyAsync(x => x.Id == formId);

            if (!formExists)
                throw new Exception("فرم پیدا نشد");

            var query = _context.FormAnswers.AsNoTracking().Where(x => x.FormId == formId);

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= toDate.Value);
            }

            return await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new FormResponseDTO
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,

                    Answers = x.AnswerDetails
                        .Select(a => new ResponseAnswerDTO
                        {
                            FieldId = a.FormFieldId,
                            FieldLabel = a.FormField.Label,
                            Value = a.Value
                        }).ToList()
                }).ToListAsync();
        }


        public async Task<int> AddFieldAsync(int formId, CreateFormFieldDTO dto)
        {
            var form = await _context.Forms.AnyAsync(x => x.Id == formId);

            if (!form)
                throw new Exception("فرم پیدا نشد");

            var field = new FormFields
            {
                FormId = formId,
                Label = dto.Label,
                FieldType = dto.FieldType,
                Required = dto.Required,
                Placeholder = dto.Placeholder,
                DefaultValue = dto.DefaultValue,
                Order = dto.Order
            };

            _context.FormFields.Add(field);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Field added FormId={FormId} FieldId={FieldId}",formId,field.Id);
            return field.Id;
        }


        public async Task<bool> UpdateFieldAsync(int formId, int fieldId, UpdateFormFieldDTO dto)
        {
            var field = await _context.FormFields.FirstOrDefaultAsync(x => x.Id == fieldId && x.FormId == formId && !x.IsDeleted);

            if (field == null)
                return false;

            field.Label = dto.Label;
            field.FieldType = dto.FieldType;
            field.Required = dto.Required;
            field.Placeholder = dto.Placeholder;
            field.DefaultValue = dto.DefaultValue;
            field.Order = dto.Order;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Field updated FormId={FormId} FieldId={FieldId}", formId, fieldId);
            return true;
        }


        public async Task<bool> DeleteFieldAsync(int formId, int fieldId)
        {
            var field = await _context.FormFields.FirstOrDefaultAsync(x => x.Id == fieldId && x.FormId == formId && !x.IsDeleted);

            if (field == null)
                return false;

            field.IsDeleted = true;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Field deleted FormId={FormId} FieldId={FieldId}", formId,fieldId);

            return true;
        }

        
       
    }
}
