﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<String> Errors { get;private set; } //hataları listo larak tutuyorum birden fazla hata döndüğünde bu methodu çalıştıracağım.
        public bool lsShow { get;private set; } //developerin anlayacağı hataları false olacak. 

        public ErrorDto() 
        {
            Errors = new List<string>(); 
        }

        public ErrorDto(string error, bool isShow) //birden fazla gelen hata durumunda yukardaki hata dizinin buraya ekleyip veriyoruz. 
        {
            Errors.Add(error); 
            isShow = true;
        }
        public ErrorDto(List<string> errors, bool isShow) //hata dizininde gleen hatayı vereceğiz
        {
            Errors = Errors;
            lsShow = isShow;
        }
    }
}
