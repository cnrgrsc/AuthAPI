using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using SharedLibrary.Exceptions;
using System.Text.Json;

namespace SharedLibrary.Extensions
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config => //UseExceptionHandler bütün hataları yakalayan bir middleviere
            {
                config.Run(async context => //run methodu sonlandırıcı bir midelviere sonraki middleware geçmez
                {
                    context.Response.StatusCode = 500; //status kodu 500 verdi çünkü sorun benden kaynaklı oldu
                    context.Response.ContentType = "application/json"; //burada bir json döneceğim

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>(); //IExceptionHandlerFeature bu interface üzerinden hataları yakalyacağız

                    if (errorFeature != null) //errorFeature null değilse 
                    {
                        var ex = errorFeature.Error;

                        ErrorDto errorDto = null; //bunu ilk null set ettik

                        if (ex is CustomException) //CustomException ise farklı olacak
                        {
                            errorDto = new ErrorDto(ex.Message, true); //mesajı veriyprum 
                        }
                        else
                        {
                            errorDto = new ErrorDto(ex.Message, false); //değilsede uygulama kendi içinde hata fırlatmıştır
                        }

                        var response = Response<NoDataDto>.Fail(errorDto, 500); //burada artık response veriyoruz

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response)); //JsonSerializer edeceğiz burada artık nwetonsoft kullanamaya gerek yok çünü microsoft bu sorunu kendi gördü ve bu konu ile güncelleme yaptı system.text.json içinden geleni kullanacağız. response serilazze ettik
                    }
                });
            });
        }
    }
}
