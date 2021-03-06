using CurrieTechnologies.Razor.Clipboard;
using DevGear.Shared;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;

namespace DevGear.Pages
{
    public interface IDiffVM : IViewModel
    {
       
    }

    public class DiffVM : ViewModel, IDiffVM
    {
        public DiffVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
        : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "Diff Text";
            Url = "diff3";
            TextType = "application/ld+json";
            LeftTextBoxTitle = "Source 1";
            RightTextBoxTitle = "Source 2";
            RightTextboxReadOnly = false;
            Functions.Add("Format", Format);
        }

        public async Task<bool> Format()
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(await GetSourceString());
                ResultString = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
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
