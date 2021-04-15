using CurrieTechnologies.Razor.Clipboard;
using DevGear.Shared;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace DevGear.Pages
{
    public interface IJwtVM : IViewModel
    {
        public Task<bool> Decode();
    }

    public class JwtVM : ViewModel, IJwtVM
    {
        public JwtVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
            : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "Jwt Functions";
            Url = "Jwt";
            TextType = "application/ld+json";

            Functions.Add("Decode", Decode);
            Functions.Add("Decode Detailed", DecodeDetailed);
        }


        public async Task<bool> Decode()
        {
            try
            {
                var input = await GetSourceString();

                var claims = await Decode(input.Trim());
                StringBuilder sb = new StringBuilder("HEADER: \r\n-----------------------\r\n");
                sb.AppendLine(JsonConvert.SerializeObject(claims.Header, Formatting.Indented));

                sb.AppendLine("\r\n\r\nPAYLOAD: \r\n-----------------------\r\n");
                sb.AppendLine(JsonConvert.SerializeObject(claims.Payload, Formatting.Indented));

                ResultString = sb.ToString();
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        public async Task<bool> DecodeDetailed()
        {
            try
            {
                var input = await GetSourceString();
                var claims = await Decode(input.Trim());

                StringBuilder sb = new StringBuilder("DETAILS: \r\n-----------------------\r\n");
                sb.AppendLine(JsonConvert.SerializeObject(claims, Formatting.Indented));

                ResultString = sb.ToString();
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }


        private async Task<JwtSecurityToken> Decode(string input, string secret = null)
        {
            ResultString = "";
            await SetResult();

            //string secret = "this is a string used for encrypt and decrypt token";
            //  var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                //  IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var claims = handler.ReadJwtToken(input);
            //var claims = handler.ValidateToken(input, validations, out var tokenSecure);

            return claims;

          
        }
    }
}
