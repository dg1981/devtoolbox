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

namespace DevGear.Pages
{
    public interface IBase64VM : IViewModel
    {
        public Task<bool> Encode();
        public Task<bool> Decode();
    }

    public class Base64VM : ViewModel, IBase64VM
    {
        public Base64VM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
            : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "Base64 Functions";
            Url = "base64";
            TextType = "application/text";
            Functions.Add("Encode", Encode);
            Functions.Add("Decode", Decode);
        }
        public async Task<bool> Encode()
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(await GetSourceString());
                ResultString = Convert.ToBase64String(plainTextBytes);

                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        public async Task<bool> Decode()
        {
            try
            {
                var input = await GetSourceString();

                ResultString = Decode(input);
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }


        private string Decode(string input)
        {

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(input));

            }
            catch
            {
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/')));
                }
                catch (Exception) { }
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "="));
                }
                catch (Exception) { }
                return Encoding.UTF8.GetString(Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/') + "=="));

            }
        }
    }
}
