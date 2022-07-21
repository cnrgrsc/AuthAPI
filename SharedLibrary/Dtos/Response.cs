using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T:class
    {
        public T Data { get;private set; } //T dipinde bir data tutup kendi içinde set edecek bu yüzden set private.
        public int StatusCode { get;private set; } //

        [JsonIgnore] //
        public bool lsSuccessful { get;private set; } //kendi iç projemde başarlı olup olmadığını kontrol etmek için

        public ErrorDto  Error { get;private set; } //birden fazla hata tutabilir.

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode,lsSuccessful=true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode,lsSuccessful=true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                lsSuccessful=false

            };
        }

        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T> { Error = errorDto, StatusCode = statusCode,lsSuccessful=false };

        }
    }
}
