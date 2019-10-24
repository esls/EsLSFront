using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace EsLSFront.Helpers
{
    public static class TokenUsernameReader
    {
        public static string GetUsername(string authCookie)
        {
            if (authCookie != null)
            {
                JwtSecurityToken token;
                try { token = new JwtSecurityTokenHandler().ReadJwtToken(authCookie); }
                catch { return null; }
                var nameClaim = token.Claims.SingleOrDefault(x => x.Type == "friendly_name");
                if (nameClaim == null)
                    return null;
                return nameClaim.Value;
            }
            else
                return null;
        }
    }
}
