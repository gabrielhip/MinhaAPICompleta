using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevIO.Business.Interfaces
{
    public interface IUser
    {
        string Name { get; }
        
        Guid GetUserId();

        string GetUserEmail();

        bool IsAuthenticated();

        bool IsInRole(string role); //verifica se a role apresentada faz parte das credenciais do usuário

        IEnumerable<Claim> GetClaimsIdentity();
    }
}
