using FormBuilder.Application.DTOs;
using FormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormService _formService;
       
        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(CreateFormDTO dto)
        {
            var formId = await _formService.CreateAsync(dto);

            return Ok(formId);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllForms(bool? isactive, int page = 1, int pageSize = 10)
        {
            var Forms = await _formService.GetAllAsync(isactive, page,pageSize);
            return Ok(Forms);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormById(int id)
        {
            var Form= await _formService.GetByIdAsync(id);
            if (Form==null) 
                return NotFound();
            return Ok(Form);
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeFormStatus(int id)
        {
            var result = await _formService.ChangeStatusAsync(id);

            if (!result)
                return NotFound();

            return Ok("وضعیت فرم تغییر داده شد ");

        }


        [HttpPost("{formId}/responses")]
        public async Task<IActionResult> SubmitAnswer(int formId, SubmitAnswerDTO dto)
        {
            try
            {
                dto.FormId = formId;
                
                await _formService.SubmitAnswerAsync(dto);

                return Ok("پاسخ شما با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}/responses")]
        public async Task<IActionResult> GetResponses(int id, DateTime? fromDate, DateTime? toDate, int page = 1,int pageSize = 10)
        {
            var responses = await _formService.GetResponsesAsync(id, fromDate,toDate, page, pageSize);

            return Ok(responses);
        }
        [HttpPost("{formId}/fields")]
        public async Task<IActionResult> AddField(int formId,CreateFormFieldDTO dto)
        {
            var fieldId =await _formService.AddFieldAsync(formId,dto);

            return Ok(fieldId);
        }


        [HttpPut("{formId}/fields/{fieldId}")]
        public async Task<IActionResult> UpdateField(int formId, int fieldId, UpdateFormFieldDTO dto)
        {
            var result = await _formService.UpdateFieldAsync(formId, fieldId, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }




        [HttpDelete("{formId}/fields/{fieldId}")]
        public async Task<IActionResult> DeleteField(int formId, int fieldId)
        {
            var result = await _formService.DeleteFieldAsync(formId, fieldId);

            if (!result)
                return NotFound();

            return Ok("فیلد مورد نظر شما پاک شد");
        }

     
    }
}
