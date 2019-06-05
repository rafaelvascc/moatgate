using MoatGate.RequestHandling.Exceptions;
using System;
using System.Collections.Generic;

namespace MoatGate.RequestHandling.Extensions
{
    public static class RestRequestExceptionExtensions
    {
        private static Response BuildBaseResponse(RestResquestException ex, Guid id)
        {
            switch (ex.StatusCode)
            {
                case 401:
                    return Response.Unauthorized(id, ex);
                case 403:
                    return Response.Forbidden(id, ex);
                case 404:
                    return Response.NotFound(id, ex);
                case 429:
                    return Response.TooManyRequests(id, ex);
                default:
                    {
                        if (ex.StatusCode >= 400 && ex.StatusCode < 500)
                            return Response.BadRequest(id, ex);

                        return Response.InternalServerError(id, ex);
                    }
            }
        }

        public static Response AsErrorResponse(this RestResquestException ex, Guid id)
        {
            return BuildBaseResponse(ex, id);
        }

        public static Response AsErrorResponse(this RestRequestUserCreationException ex, Guid id)
        {
            Response response = BuildBaseResponse(ex, id);
            response.Messages = ex.Errors;
            return response;
        }

        public static Response AsErrorResponse(this AuthenticationException ex, Guid id)
        {
            Response response = BuildBaseResponse(ex, id);
            response.Messages = new List<string> { ex.Message };
            return response;
        }
    }
}
