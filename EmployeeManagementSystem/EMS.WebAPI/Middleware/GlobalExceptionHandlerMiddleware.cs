using EMS.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMS.WebAPI.Middleware
{
    /// <summary>
    /// Global exception handling middleware to catch and handle all unhandled exceptions
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Success = false,
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Title = "Not Found";
                    errorResponse.Message = GetUserFriendlyNotFoundMessage(notFoundEx);
                    errorResponse.Suggestion = "Please check the ID and try again, or contact support if the problem persists.";
                    _logger.LogWarning(exception, "Resource not found: {Message}", notFoundEx.Message);
                    break;

                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "Invalid Information";
                    errorResponse.Message = "Some of the information you provided is not valid. Please review and correct the highlighted fields.";
                    errorResponse.FieldErrors = GetUserFriendlyValidationErrors(validationEx.Errors);
                    errorResponse.Suggestion = "Please check your input and make sure all required fields are filled correctly.";
                    _logger.LogWarning(exception, "Validation error: {Message}", validationEx.Message);
                    break;

                case DuplicateException duplicateEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Title = "Already Exists";
                    errorResponse.Message = GetUserFriendlyDuplicateMessage(duplicateEx);
                    errorResponse.Suggestion = "Please use a different value or update the existing record instead.";
                    _logger.LogWarning(exception, "Duplicate resource: {Message}", duplicateEx.Message);
                    break;

                case UnauthorizedException unauthorizedEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Title = "Access Denied";
                    errorResponse.Message = GetUserFriendlyUnauthorizedMessage(unauthorizedEx);
                    errorResponse.Suggestion = "Please log in with valid credentials or contact your administrator for access.";
                    _logger.LogWarning(exception, "Unauthorized access: {Message}", unauthorizedEx.Message);
                    break;

                case DatabaseException dbEx:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Title = "Service Temporarily Unavailable";
                    errorResponse.Message = "We're having trouble processing your request right now. Our team has been notified.";
                    errorResponse.Suggestion = "Please wait a few moments and try again. If the problem continues, contact support.";
                    _logger.LogError(exception, "Database error: {Message}", dbEx.Message);
                    break;

                case InvalidOperationException invalidOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "Action Not Allowed";
                    errorResponse.Message = GetUserFriendlyInvalidOperationMessage(invalidOpEx);
                    errorResponse.Suggestion = "Please verify your request and try again.";
                    _logger.LogWarning(exception, "Invalid operation: {Message}", invalidOpEx.Message);
                    break;

                case ArgumentNullException argNullEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "Missing Information";
                    errorResponse.Message = $"The {FormatFieldName(argNullEx.ParamName ?? "required field")} is missing. Please provide this information to continue.";
                    errorResponse.Suggestion = "Please fill in all required fields and try again.";
                    _logger.LogWarning(exception, "Missing parameter: {ParamName}", argNullEx.ParamName);
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = "Invalid Input";
                    errorResponse.Message = "The information you provided is not in the correct format.";
                    errorResponse.Suggestion = "Please check your input and try again.";
                    _logger.LogWarning(exception, "Invalid argument: {Message}", argEx.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Title = "Something Went Wrong";
                    errorResponse.Message = "We encountered an unexpected issue while processing your request. Our team has been notified and is working on it.";
                    errorResponse.Suggestion = "Please try again later. If the problem persists, contact our support team.";
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await response.WriteAsync(result);
        }

        /// <summary>
        /// Generates a user-friendly message for NotFoundException
        /// </summary>
        private static string GetUserFriendlyNotFoundMessage(NotFoundException ex)
        {
            if (!string.IsNullOrEmpty(ex.ResourceName))
            {
                var friendlyName = FormatResourceName(ex.ResourceName);
                return $"We couldn't find the {friendlyName} you're looking for. It may have been removed or the ID might be incorrect.";
            }
            return "The item you're looking for doesn't exist or may have been removed.";
        }

        /// <summary>
        /// Generates a user-friendly message for DuplicateException
        /// </summary>
        private static string GetUserFriendlyDuplicateMessage(DuplicateException ex)
        {
            if (!string.IsNullOrEmpty(ex.ResourceName) && !string.IsNullOrEmpty(ex.PropertyName))
            {
                var friendlyResource = FormatResourceName(ex.ResourceName);
                var friendlyProperty = FormatFieldName(ex.PropertyName);
                return $"A {friendlyResource} with this {friendlyProperty} already exists. Each {friendlyResource} must have a unique {friendlyProperty}.";
            }
            return "This record already exists in our system. Please use different information or update the existing record.";
        }

        /// <summary>
        /// Generates a user-friendly message for UnauthorizedException
        /// </summary>
        private static string GetUserFriendlyUnauthorizedMessage(UnauthorizedException ex)
        {
            if (ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("credential", StringComparison.OrdinalIgnoreCase))
            {
                return "The username or password you entered is incorrect. Please check your credentials and try again.";
            }
            if (ex.Message.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("authorized", StringComparison.OrdinalIgnoreCase))
            {
                return "You don't have permission to perform this action. Please contact your administrator if you need access.";
            }
            return "You need to log in to access this feature. Please sign in and try again.";
        }

        /// <summary>
        /// Generates a user-friendly message for InvalidOperationException
        /// </summary>
        private static string GetUserFriendlyInvalidOperationMessage(InvalidOperationException ex)
        {
            if (ex.Message.Contains("JWT", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("token", StringComparison.OrdinalIgnoreCase))
            {
                return "There's a configuration issue with the authentication system. Please contact the administrator.";
            }
            return "This action cannot be completed at this time. Please verify your request and try again.";
        }

        /// <summary>
        /// Converts validation errors to user-friendly messages
        /// </summary>
        private static IDictionary<string, string[]> GetUserFriendlyValidationErrors(IDictionary<string, string[]> errors)
        {
            var friendlyErrors = new Dictionary<string, string[]>();

            foreach (var error in errors)
            {
                var friendlyFieldName = FormatFieldName(error.Key);
                var friendlyMessages = error.Value.Select(msg => MakeMessageUserFriendly(friendlyFieldName, msg)).ToArray();
                friendlyErrors[friendlyFieldName] = friendlyMessages;
            }

            return friendlyErrors;
        }

        /// <summary>
        /// Makes individual validation messages more user-friendly
        /// </summary>
        private static string MakeMessageUserFriendly(string fieldName, string message)
        {
            // Common validation message patterns and their user-friendly versions
            if (message.Contains("required", StringComparison.OrdinalIgnoreCase))
                return $"Please enter your {fieldName.ToLower()}.";

            if (message.Contains("email", StringComparison.OrdinalIgnoreCase) && message.Contains("valid", StringComparison.OrdinalIgnoreCase))
                return "Please enter a valid email address (e.g., name@example.com).";

            if (message.Contains("minimum length", StringComparison.OrdinalIgnoreCase) || message.Contains("at least", StringComparison.OrdinalIgnoreCase))
                return $"Your {fieldName.ToLower()} is too short. Please enter more characters.";

            if (message.Contains("maximum length", StringComparison.OrdinalIgnoreCase) || message.Contains("exceed", StringComparison.OrdinalIgnoreCase))
                return $"Your {fieldName.ToLower()} is too long. Please shorten it.";

            if (message.Contains("format", StringComparison.OrdinalIgnoreCase))
                return $"The {fieldName.ToLower()} format is not correct. Please check and try again.";

            // If no pattern matches, return the original message
            return message;
        }

        /// <summary>
        /// Formats resource names to be more readable (e.g., "Employee" instead of "Employees")
        /// </summary>
        private static string FormatResourceName(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName)) return "record";

            // Remove trailing 's' for plural names
            var name = resourceName.TrimEnd('s');

            // Convert PascalCase to readable format
            return SplitPascalCase(name).ToLower();
        }

        /// <summary>
        /// Formats field names to be more readable (e.g., "First Name" instead of "FirstName")
        /// </summary>
        private static string FormatFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return "field";

            // Handle common field name patterns
            var name = fieldName
                .Replace("ID", " ID")
                .Replace("_", " ");

            return SplitPascalCase(name).ToLower();
        }

        /// <summary>
        /// Splits PascalCase text into separate words
        /// </summary>
        private static string SplitPascalCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var result = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (char.IsUpper(c) && result.Length > 0)
                    result.Append(' ');
                result.Append(c);
            }
            return result.ToString().Trim();
        }
    }

    /// <summary>
    /// Standard error response model with user-friendly messages
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Suggestion { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public IDictionary<string, string[]>? FieldErrors { get; set; }
    }

    /// <summary>
    /// Extension method to register the middleware
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}

