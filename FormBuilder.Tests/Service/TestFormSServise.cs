using FluentAssertions;
using FormBuilder.Application.DTOs;
using FormBuilder.Application.Services;
using FormBuilder.Domain;
using FormBuilder.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using FormBuilder.Domain.Entities;

namespace FormBuilder.Tests.Service
{
    public class TestFormSServise
    {

        private FormBuilderDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<FormBuilderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FormBuilderDbContext(options);
        }
        private FormService CreateService(FormBuilderDbContext context)
        {
            var logger = new Mock<ILogger<FormService>>();

            return new FormService(
                context,
                logger.Object);
        }
        [Fact]
        public async Task DeleteFieldAsync_Should_ReturnFalse_When_Field_Not_Exist()
        {
            var context = GetDbContext();
            var service =  CreateService(context);

            var result = await service.DeleteFieldAsync(1, 999);

            result.Should().BeFalse();
        }
        [Fact]
        public async Task CreateAsync_Should_ReturnFOrmID()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var dto = new CreateFormDTO
            {
                Name = "یونیت تست",
                Description = " تست متد ",
                Fields = new List<CreateFormFieldDTO>
                {
                    new CreateFormFieldDTO
                    {
                        Label = "تست",
                        Required = false,
                        Order = 2,
                        Placeholder = "تست",
                        FieldType = FormBuilder.Domain.Enum.FieldType.Number,
                        DefaultValue = "تست دیفالت  "
                    },
                     new CreateFormFieldDTO
                    {
                        Label = "تست2",
                        Required = true,
                        Order = 3,
                        Placeholder = "تست2",
                        FieldType = Domain.Enum.FieldType.Checkbox,
                        DefaultValue = "تست دیفالت  "
                    }

                }
            };
            var formId = await service.CreateAsync(dto);

            formId.Should().BeGreaterThan(0);

            var form = await context.Forms.Include(f => f.Fields).FirstOrDefaultAsync(f => f.Id == formId);

            form.Should().NotBeNull();
            form.Fields.Should().HaveCount(2);

            form.Fields.First().Label.Should().Be("تست");



        }
        [Fact]
        public async Task CreateAsync_Should_Save_Order()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var dto = new CreateFormDTO
            {
                Name = "Test Form",
                Fields = new List<CreateFormFieldDTO>
                    {
                        new CreateFormFieldDTO { Label="لیبل دو", Order=2, FieldType=Domain.Enum.FieldType.Text },
                        new CreateFormFieldDTO { Label="لیبل یک", Order=1, FieldType=Domain.Enum.FieldType.Text }
                    }
            };

            var formId = await service.CreateAsync(dto);

            var fields = await context.FormFields.Where(f => f.FormId == formId).OrderBy(f => f.Order).ToListAsync();

            fields[0].Label.Should().Be("لیبل یک");
            fields[1].Label.Should().Be("لیبل دو");
        }
        [Fact]
        public async Task SubmitAnswerAsync_Should_Save_Response()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var form = new Form
            {
                Name = "Test Form",
                IsActive = true
            };

            var field = new FormFields
            {
                Label = "Name",
                Form = form,
                Required = true,
                FieldType = FormBuilder.Domain.Enum.FieldType.Text,
                Order = 1
            };

            context.Forms.Add(form);
            context.FormFields.Add(field);

            await context.SaveChangesAsync();

            var dto = new SubmitAnswerDTO
            {
                FormId = form.Id,
                Answers =
                [
                    new AnswerDTO
                            {
                                FieldId = field.Id,
                                Value = "Maryam"
                            }
                ]
            };

            await service.SubmitAnswerAsync(dto);

            var savedResponse = await context.FormAnswers.Include(x => x.AnswerDetails).FirstOrDefaultAsync();

            savedResponse.Should().NotBeNull();

            savedResponse!.AnswerDetails.Should().HaveCount(1);

            savedResponse.AnswerDetails.First().Value.Should().Be("Maryam");
        }
        [Fact]
        public async Task SubmitAnswerAsync_Should_Throw_When_Required_Field_Empty()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var form = new Form
            {
                Name = "Test Form",
                IsActive = true
            };

            var field = new FormFields
            {
                Form = form,
                Label = "نام",
                Required = true,
                FieldType = FormBuilder.Domain.Enum.FieldType.Text,
                Order = 1
            };

            context.Forms.Add(form);
            context.FormFields.Add(field);

            await context.SaveChangesAsync();

            var dto = new SubmitAnswerDTO
            {
                FormId = form.Id,
                Answers = new List<AnswerDTO>
        {
            new AnswerDTO
            {
                FieldId = field.Id,
                Value = ""
            }
        }
            };

            Func<Task> act = async () =>await service.SubmitAnswerAsync(dto);

            await act.Should().ThrowAsync<Exception>().WithMessage("*الزامی است*");
        }

        [Fact]
        public async Task ChangeStatusAsync_Should_Change_Status()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var form = new Form
            {
                Name = "Test Form",
                IsActive = true
            };

            context.Forms.Add(form);
            await context.SaveChangesAsync();

            var result = await service.ChangeStatusAsync(form.Id);

            result.Should().BeTrue();

            var updatedForm = await context.Forms.FirstOrDefaultAsync(x => x.Id == form.Id);

            updatedForm.Should().NotBeNull();
            updatedForm!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateFieldAsync_Should_Update_Field()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var form = new Form
            {
                Name = "Test Form",
                IsActive = true
            };

            context.Forms.Add(form);
            await context.SaveChangesAsync();

            var field = new FormFields
            {
                FormId = form.Id,
                Label = "نام",
                FieldType = FormBuilder.Domain.Enum.FieldType.Text,
                Required = true,
                Order = 1
            };

            context.FormFields.Add(field);
            await context.SaveChangesAsync();

            var dto = new UpdateFormFieldDTO
            {
                Label = "نام ",
                FieldType = FormBuilder.Domain.Enum.FieldType.Text,
                Required = false,
                Placeholder = "نام خود را وارد کنید",
                DefaultValue = "علی",
                Order = 2
            };

            var result = await service.UpdateFieldAsync(form.Id, field.Id, dto);

            result.Should().BeTrue();

            var updatedField = await context.FormFields.FirstOrDefaultAsync(x => x.Id == field.Id);

            updatedField.Should().NotBeNull();
            updatedField!.Label.Should().Be("نام ");
            updatedField.Required.Should().BeFalse();
            updatedField.Placeholder.Should().Be("نام خود را وارد کنید");
            updatedField.DefaultValue.Should().Be("علی");
            updatedField.Order.Should().Be(2);
        }
        [Fact]
        public async Task UpdateFieldAsync_Should_ReturnFalse_When_Field_Not_Found()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var dto = new UpdateFormFieldDTO
            {
                Label = "Test",
                FieldType = FormBuilder.Domain.Enum.FieldType.Text
            };
            var result = await service.UpdateFieldAsync(1,999, dto);
            result.Should().BeFalse();
        }


        [Fact]
        public async Task GetByIdAsync_Should_Return_Form()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var form = new Form
            {
                Name = "فرم تست",
                Description = "توضیحات تست",
                IsActive = true,
                Fields = new List<FormFields>
        {
            new FormFields
            {
                Label = "نام",
                FieldType = FormBuilder.Domain.Enum.FieldType.Text,
                Required = true,
                Order = 1
            }
        }
            };

            context.Forms.Add(form);
            await context.SaveChangesAsync();
            var result = await service.GetByIdAsync(form.Id);
            result.Should().NotBeNull();

            result!.Id.Should().Be(form.Id);
            result.Name.Should().Be("فرم تست");
            result.Description.Should().Be("توضیحات تست");
            result.IsActive.Should().BeTrue();

            result.Fields.Should().HaveCount(1);

            result.Fields.First().Label.Should().Be("نام");
            result.Fields.First().Required.Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Form_Not_Found()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var result = await service.GetByIdAsync(999);

            result.Should().BeNull();
        }


        [Fact]
        public async Task ChangeStatusAsync_Should_ReturnFalse_When_Form_Not_Found()
        {
            var context = GetDbContext();
            var service = CreateService(context);

            var result = await service.ChangeStatusAsync(999);

            result.Should().BeFalse();
        }

    }
}