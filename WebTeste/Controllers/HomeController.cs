using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebTeste.Models;
using WebTeste.Service;
using System.Security.Claims;

namespace WebTeste.Controllers
{
    public class HomeController : Controller
    {

        [AllowAnonymous] //Indica que esse metodo pode ser acessado sem esta logado
        public IActionResult Index(bool erroLogin)
        {
            if (erroLogin)
            {
                ViewBag.Erro = "Nome ou senha errado!";
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
               return RedirectToAction("Proficional");
            }

            return View(); 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Usuario user)
        {

            var usuarioDB = new Usuario()
            {
                Nome = "Gustavo",
                Senha = "123456",
                Cargo = ""
            };

            if (!usuarioDB.Nome.Equals(user.Nome)||
                !usuarioDB.Senha.Equals(user.Senha)
                )
            {
                return RedirectToAction("Index", new {erroLogin = true});
            }

            await new Services().Login(HttpContext, user);
            return RedirectToAction("Proficional");
        }

        [Authorize]
        public async Task<IActionResult> Sair()
        {
            await new Services().Logoff(HttpContext);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "adm, teste")]
        public IActionResult Proficional()
        {
            ViewBag.Permissoes = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);

            return View();
        }

    }
}