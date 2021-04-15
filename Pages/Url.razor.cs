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

namespace DevGear.Pages
{
    public interface IUrlVM : IViewModel
    {
        public Task<bool> UrlEncode();
        public Task<bool> UrlDecode();
    }

    public class UrlVM : ViewModel, IUrlVM
    {
        public UrlVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
            : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "URL Functions";
            Url = "url";
            TextType = "application/text";
            Functions.Add("Encode", UrlEncode);
            Functions.Add("Decode", UrlDecode);
            AddCssToCodeMirror("HorizontalScroll");
         }

        public async Task<bool> UrlEncode()
        {
            try
            {
                ResultString = Uri.EscapeDataString(await GetSourceString());
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        public async Task<bool> UrlDecode()
        {
            try
            {
                ResultString = System.Net.WebUtility.UrlDecode(await GetSourceString());
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }
    }
}
