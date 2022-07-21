using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<String> Errors { get;private set; } = new List<string>(); //hataları listo larak tutuyorum birden fazla hata döndüğünde bu methodu çalıştıracağım. = direk ınıtizial işlemini ypatık
        public bool IsShow { get;private set; } //developerin anlayacağı hataları false olacak. 


        public ErrorDto(string error, bool isShow) //birden fazla gelen hata durumunda yukardaki hata dizinin buraya ekleyip veriyoruz. 
        {
            Errors.Add(error);
            IsShow = isShow;
        }
        public ErrorDto(List<string> errors, bool isShow) //hata dizininde gleen hatayı vereceğiz
        {
            Errors = errors;
            IsShow = isShow;
        }
    }
}
