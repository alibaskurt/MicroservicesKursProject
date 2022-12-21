using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.ControllerBases
{
    //Tüm microservislerde ortak bir response dönebilmek adına bu sınıf yazıldı. Her defasında Ok(),NotFound(),BadRequest()
    //vs gibi methodlar yerıne tek bir methodda belirttiğimiz durum kodlarını göndericez.
    public class CustomControllerBase : ControllerBase
    {
        //Bu metot response<T> paremetresini alıp ortak bir response nesnesi oluşturup verdiğimiz status kodlarını dönmeyi sağlayacak.
        protected IActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            var responseObject = new ObjectResult(response)
            {
                StatusCode = response.StatusCode,
            };
            return responseObject;
        }
    }
}
