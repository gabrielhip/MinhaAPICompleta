using System.Threading.Tasks;
using DevIO.API.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager; //faz o trabalho de realizar o signin (autenticação do usuário)
        private readonly UserManager<IdentityUser> _userManager; //responsável por criar o usuário e fazer suas manipulações

        public AuthController(INotificador notificador, 
                              UserManager<IdentityUser> userManager, 
                              SignInManager<IdentityUser> signInManager) : base(notificador)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email, //não obriga a passar o email como UserName, mas é melhor para o usuário não ter que lembrar de mais uma informação
                Email = registerUser.Email,
                EmailConfirmed = true //já garante que o e-mail está confirmado, pois não se trata de uma aplicação MVC
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password); //criando usuário
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false); //realiza o login do usuário, isPersistent = se desejo lembrar do usuário/gravar login dele
                return CustomResponse(registerUser);
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description); //caso exista algum erro, notifica o mesmo para o usuário
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            //só dá o nome do usuário e a senha para realizar login
            //isPersistent = se desejo lembrar do usuário/gravar login dele
            //lockoutOnFailure = se o usuário tentar mais de 5 vezes com credenciais erradas, o sistema irá travar o login por um tempo definido, impossibilitando o login
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(loginUser);
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUser);
            }

            //SEMPRE dar o mínimo de informação possível, como dizer se somente a senha ou o usuário estão incorretos, por questões de segurança
            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(loginUser);
        }
    }
}