﻿using Domain.Exceptions;
using Shared.ErrorModels;
using System.Net;

namespace E_Commerce.API.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

            }
            catch (Exception exception)
            {
                _logger.LogError($"something went wrong{exception}");

                await HandelExceptionAsync(httpContext, exception);
            }
        }

        private async Task HandelExceptionAsync(HttpContext httpContext, Exception exception)
        {
            // set Default Status Code to 500
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // set content type => Application\json
            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };
           //return standard Response
           var response = new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                ErrorMessage = exception.Message
            }.ToString();

            await httpContext.Response.WriteAsync(response);
        }
    }
}