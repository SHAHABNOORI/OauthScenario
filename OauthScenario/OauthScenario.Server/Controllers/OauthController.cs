using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace OauthScenario.Server.Controllers
{
    public class OauthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
          string response_type, //authorization flow type
          string client_id, // client id
          string redirect_uri,
          string scope, // what info i want = email,grandma,tell
          string state) // random string generated to confirm thatwe are going back to the same client
        {
            // ?a=foo?b=bar
            var query = new QueryBuilder { { "redirectUri", redirect_uri }, { "state", state } };
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(string username, string redirectUri, string state)
        {
            const string code = "BABABABABABA";
            var query = new QueryBuilder { { "code", code }, { "state", state } };
            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type, // flow of access_token request
            string code,//confirmation of the authentication process
            string redirect_uri,
            string client_id
            )
        {
            // Some mechanism for validating the code

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim("granny", "cookie")
            };
            var secretBytes = Encoding.UTF8.GetBytes(OauthConstant.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(OauthConstant.Issuer, OauthConstant.Audience, claims, notBefore: DateTime.Now, expires: DateTime.Now.AddHours(1), signCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var responseObject = new
            {
                access_token = accessToken,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var jsonResponce = JsonConvert.SerializeObject(responseObject);
            var responceByte = Encoding.UTF8.GetBytes(jsonResponce);
            await Response.Body.WriteAsync(responceByte, 0, responceByte.Length);
            return Redirect(redirect_uri);
        }
    }
}