using System.Collections.Generic;

namespace ozgurtek.framework.common.Util
{
    public class GdJwtUtil
    {
        public string GenerateJwtToken(string secretKey, IReadOnlyDictionary<string, string> payloadContents)
        {
            //Base64UrlEncoder.

            //JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            //SigningCredentials signingCredentials = new SigningCredentials(securityKey, "HS256");

            //IEnumerable<Claim> payloadClaims = payloadContents.Select(c => new Claim(c.Key, c.Value));

            //JwtPayload payload = new JwtPayload(payloadClaims);
            //JwtHeader header = new JwtHeader(signingCredentials);
            //JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);

            //return jwtSecurityTokenHandler.WriteToken(securityToken);
            return null;
        }
    }
}