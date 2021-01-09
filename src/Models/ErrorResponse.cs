using System.Collections.Generic;

namespace Sparx.Sdk.Models
{
    public class Response<T>
    {
        public Response(string message)
        {
            Success = false;
            ErrorMessage = message;
        }

        public Response()
        {
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}