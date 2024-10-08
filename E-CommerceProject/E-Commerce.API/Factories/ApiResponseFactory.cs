﻿using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;
using System.Net;

namespace E_Commerce.API.Factories
{
    public class ApiResponseFactory
    {
        public static IAsyncResult CustomValidationErrorResponse(ActionContext context)
        {
            // get all Errors in Model state
            var errors = context.ModelState.Where(error => error.Value.Errors.Any())
                .Select(error => new ValidationError
                {
                    Field = error.Key,
                    Errors = error.Value.Errors.Select(e => e.ErrorMessage)
                });
            // Create custom Response
            var response = new ValidationErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "Validation Failed",
                Errors = errors
            };
            // return
            return new BadRequestObjectResult(response);
        }
    }
}
