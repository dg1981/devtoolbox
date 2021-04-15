using CurrieTechnologies.Razor.Clipboard;
using DevGear.Shared;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;

namespace DevGear.Pages
{
    public interface IXmlVM : IViewModel
    {

    }

    public class XmlVM : ViewModel, IXmlVM
    {
        public XmlVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
            : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "XML Functions";
            Url = "xml";
            TextType = "application/xml";
            Functions.Add("Format", Format);
            Functions.Add("Validate", Validate);
            Functions.Add("Minify", Minify);
            Functions.Add("To Json", ToJson);
        }
        public async Task<bool> Format()
        {
            try
            {
                var stringBuilder = new StringBuilder();
                var element = XElement.Parse(await GetSourceString());

                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = false;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.ConformanceLevel = ConformanceLevel.Auto;

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }

                ResultString = stringBuilder.ToString();
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
                var stringBuilder = new StringBuilder();
                var element = XElement.Parse(await GetSourceString());

                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = false;
                settings.NewLineOnAttributes = false;
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.NewLineHandling = NewLineHandling.None;

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }

                ResultString = stringBuilder.ToString();
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
            if (await Format())
            {
                StatusMessage = "XML code is valid!";
                StatusClass = "alert-success";
                return true;
            }
            return false;
        }


        public async Task<bool> ToJson()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(await GetSourceString());
                ResultString = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
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
