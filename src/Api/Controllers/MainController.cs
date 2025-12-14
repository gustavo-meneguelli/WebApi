using Application.Common.Models;
using Application.Enums;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace Api.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    // Traduz Result Pattern para códigos HTTP apropriados
    protected IActionResult ParseResult<T>(Result<T>? result)
    {
        if (result is null) return BadRequest(new { message = ErrorMessages.InvalidOperation });

        return result.TypeResult switch
        {
            TypeResult.Success => Ok(result.Data),

            TypeResult.Created => Created(string.Empty, result.Data),

            TypeResult.NotFound => NotFound(new { message = result.Message }),
            TypeResult.Duplicated => Conflict(new { message = result.Message }),
            TypeResult.Unauthorized => Unauthorized(new { message = result.Message }),
            TypeResult.Failure => BadRequest(new { message = result.Message }),
            TypeResult.NoContent => NoContent(),

            _ => BadRequest(new { message = result.Message })
        };
    }

    protected IActionResult? CustomResponse(ValidationResult validationResult)
    {
        // Se estiver válido, retorna null (para o controller saber que pode seguir)
        if (validationResult.IsValid) return null;

        var errors = validationResult.Errors.Select(e => new
        {
            Field = e.PropertyName,
            Message = e.ErrorMessage
        });

        return BadRequest(new
        {
            Message = "Validation errors were found.",
            Errors = errors
        });
    }
}