using CurrieTechnologies.Razor.Clipboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using ElectronNET.API;

namespace DevGear.Shared
{
    public interface IViewModel
    {
        public ElementReference ref_SourceTextArea { get; set; }
        public ElementReference ref_ResultTextArea { get; set; }

        [Required]
        public string SourceString { get; set; }
        public string ResultString { get; set; }

        public string StatusMessage { get; set; }
        public string StatusClass { get; set; }
        public string PageName { get; set; }
        public string Url { get; set; }
        public Task<bool> Initialize();
        public Task<bool> InitializeDiff();
        public Task<string> GetSourceString();
        public Task<bool> SetResult();

        public Task<bool> CopyToClipboard(ElementReference elem);
        public Task<bool> PasteFromClipboard(ElementReference elem);
        public Task<bool> ResetText();

        public string LeftTextBoxTitle { get; set; }
        public string RightTextBoxTitle { get; set; }

        public bool RightTextboxReadOnly { get; set; }

        public Dictionary<string, Func<Task<bool>>> Functions { get; set; }

    }


    public class ViewModel : IViewModel
    {
        public ElementReference ref_SourceTextArea { get; set; }
        public ElementReference ref_ResultTextArea { get; set; }

        [Required]
        public string SourceString { get; set; }
        public string ResultString { get; set; }
        public string StatusMessage { get; set; }
        public string StatusClass { get; set; } = "alert-danger";
        public string PageName { get; set; }
        public string TextType { get; set; }

        public string LeftTextBoxTitle { get; set; } = "Source";
        public string RightTextBoxTitle { get; set; } = "Result";
        public bool RightTextboxReadOnly { get; set; } = true;

        IJSRuntime _jsRunTime;
        ProtectedLocalStorage _protectedLocalStorage;
        ClipboardService _clipboard;
        public Dictionary<string, Func<Task<bool>>> Functions { get; set; } = new Dictionary<string, Func<Task<bool>>>();
        public string Url { get; set; }

        private readonly IWebHostEnvironment _hostingEnvironment;

        public ViewModel(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
        {
            try
            {
                _jsRunTime = jsRunTime;
                _protectedLocalStorage = protectedLocalStorage;
                _clipboard = clipboard;
                _hostingEnvironment = hostingEnvironment;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public ViewModel()
        {

        }

        public async Task<bool> Initialize()
        {
            try
            {

                await _jsRunTime.InvokeVoidAsync("setTitle", PageName + " - DevGearApp");
    
                await _protectedLocalStorage.SetAsync("LastPage", Url);
                await _jsRunTime.InvokeAsync<int>("codeMirrorJsFunctions.initialize", ref_SourceTextArea, TextType, false);
                await _jsRunTime.InvokeAsync<int>("codeMirrorJsFunctions.initialize", ref_ResultTextArea, TextType, RightTextboxReadOnly);

                SourceString = await _protectedLocalStorage.GetAsync<string>(PageName + "_source") ?? "";
                ResultString = await _protectedLocalStorage.GetAsync<string>(PageName + "_result") ?? "";

                if (string.IsNullOrWhiteSpace(SourceString))
                    SourceString = Sample();

                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", ref_SourceTextArea, SourceString);
                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", ref_ResultTextArea, ResultString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public async Task<bool> InitializeDiff()
        {
            try
            {
                await _jsRunTime.InvokeVoidAsync("setTitle", PageName + " - DevGearApp");
                await _protectedLocalStorage.SetAsync("LastPage", Url);
                SourceString = await _protectedLocalStorage.GetAsync<string>(PageName + "_source") ?? "";
                ResultString = await _protectedLocalStorage.GetAsync<string>(PageName + "_result") ?? "";

                //if (string.IsNullOrWhiteSpace(SourceString))
                //    SourceString = Sample();

            //    await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setLeftDiffValue", ref_SourceTextArea, SourceString);
            //    await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setRightDiffValue", ref_SourceTextArea, ResultString);

                await _jsRunTime.InvokeAsync<int>("codeMirrorJsFunctions.dodiff", ref_SourceTextArea);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }


        public void AddCssToCodeMirror(string css)
        {
            _jsRunTime.InvokeAsync<int>("codeMirrorJsFunctions.addCSS", css);
        }

        public async Task<string> GetSourceString()
        {
            try
            {
                StatusMessage = null;
                SourceString = await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.getCode", ref_SourceTextArea);

                if (string.IsNullOrWhiteSpace(SourceString))
                    throw new Exception("Please enter the string to convert");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return SourceString;
        }

        public string Sample()
        {
            try
            {
                return File.ReadAllText(_hostingEnvironment.WebRootPath  + ("/Data/" + PageName.Replace(" ", "").ToLower().Replace("functions","") + ".txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public async Task<bool> SetResult()
        {
            try
            {
                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", ref_ResultTextArea, ResultString);
                await _protectedLocalStorage.SetAsync(PageName + "_source", SourceString);
                await _protectedLocalStorage.SetAsync(PageName + "_result", ResultString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public async Task<bool> CopyToClipboard(ElementReference elem)
        {
            try
            {
                string s = await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.getCode", elem);
                await _clipboard.WriteTextAsync(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public async Task<bool> PasteFromClipboard(ElementReference elem)
        {
            try
            {
                string s = await _clipboard.ReadTextAsync();
                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", elem, s);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public async Task<bool> ResetText()
        {
            try
            {
                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", ref_SourceTextArea, "");
                await _jsRunTime.InvokeAsync<string>("codeMirrorJsFunctions.setValue", ref_ResultTextArea, "");

                await _protectedLocalStorage.SetAsync(PageName + "_source", "");
                await _protectedLocalStorage.SetAsync(PageName + "_result", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;

        }
    }
}
