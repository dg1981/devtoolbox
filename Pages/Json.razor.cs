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
    public interface IJsonVM : IViewModel
    {
       
    }

    public class JsonVM : ViewModel, IJsonVM
    {
        public JsonVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
        : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "JSON Functions";
            Url = "json";
            TextType = "application/ld+json";
            Functions.Add("Format", Format);
            Functions.Add("Validate", Validate);
            Functions.Add("Minify", Minify);
            Functions.Add("To XML", ToXml);
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

        public async Task<bool> Minify()
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(await GetSourceString());
                ResultString = JsonConvert.SerializeObject(parsedJson, Formatting.None);

                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        public async Task<bool> Validate()
        {
            StatusMessage = null;
            if (await Format())
            {
                StatusMessage = "JSON code is valid!";
                StatusClass = "alert-success";
                return true;
            }
            return false;
        }

        public async Task<bool> ToXml()
        {
            try
            {             
                XNode node = JsonConvert.DeserializeXNode(await GetSourceString(), "Root");
                ResultString = node.ToString();
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
