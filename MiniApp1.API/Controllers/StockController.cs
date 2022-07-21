using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name; //veri tabanından istediğimiz veriyi alabiliriz.

            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier); // veri tabanı üzerinden userıd ve ya username alanlarını gerekli olanları çekiyoruz
            //stockId stockQuantity Category User/UserName

            return Ok($"Stock işlemleri => UserName:{userName}-UserId:{userIdClaim.Value}");
        }
    }
}
