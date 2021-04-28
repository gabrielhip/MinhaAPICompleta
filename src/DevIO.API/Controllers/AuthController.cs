﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using DevIO.API.Extensions;
using DevIO.API.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevIO.API.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager; //faz o trabalho de realizar o signin (autenticação do usuário)
        private readonly UserManager<IdentityUser> _userManager; //responsável por criar o usuário e fazer suas manipulações
        private readonly AppSettings _appSettings;

        public AuthController(INotificador notificador, 
                              UserManager<IdentityUser> userManager, 
                              SignInManager<IdentityUser> signInManager, 
                              //IOptions serve para pegar dados que servem como parâmetros
                              IOptions<AppSettings> appSettings) : base(notificador)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
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
                return CustomResponse(GerarJwt());
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
                return CustomResponse(GerarJwt());
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

        //gera o token
        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras), //UtcNow => universal time clock (pois nunca sei de qual região o usuário é)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token); //serializa um JwtSecurityToken em um token Compact Serialization Token (para ficar compatível com padrão web)
            return encodedToken;
        }
    }
}