using Application.Common.Models;
using Application.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace Api.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected IActionResult ParseResult<T>(Result<T>? result)
    {
        if (result is null) return BadRequest(new { message = "Operação inválida." });

        return result.TypeResult switch
        {
            TypeResult.Success => Ok(result.Data),
            
            TypeResult.Created => Created(string.Empty, result.Data),
            
            TypeResult.NotFound => NotFound(new { message = result.Message }),
            TypeResult.Duplicated => Conflict(new { message = result.Message }),
            TypeResult.Unauthorized => Unauthorized(new { message = result.Message }),
            
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
            Message = "Erros de validação foram encontrados.",
            Errors = errors
        });
    }
}