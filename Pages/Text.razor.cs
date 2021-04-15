using CurrieTechnologies.Razor.Clipboard;
using DevGear.Shared;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace DevGear.Pages
{
    public interface ITextVM : IViewModel
    {
        public Task<bool> TextCalculate();
        public Task<bool> ToUpperCase();
        public Task<bool> ToLowerCase();
        public Task<bool> ToCapitalize();
    }

    public class TextVM : ViewModel, ITextVM
    {
        public TextVM(IJSRuntime jsRunTime, ProtectedLocalStorage protectedLocalStorage, ClipboardService clipboard, IWebHostEnvironment hostingEnvironment)
            : base(jsRunTime, protectedLocalStorage, clipboard, hostingEnvironment)
        {
            PageName = "Text Functions";
            Url = "text";
            TextType = "application/text";
            Functions.Add("Metrics", TextCalculate);
            Functions.Add("Upper Case", ToUpperCase);
            Functions.Add("Lower Case", ToLowerCase);
            Functions.Add("Capitalize", ToCapitalize);
        }

        public async Task<bool> TextCalculate()
        {
            try
            {
                SourceString = (await GetSourceString());
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("String Length: " + SourceString.Length);
                sb.AppendLine("Words: " + SourceString.Trim()
                    .Replace("  ", " ").Replace("  ", " ")
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries).Length);
                sb.AppendLine("Lines: " + SourceString.Split("\n", StringSplitOptions.None).Length);

                string x = SourceString.Replace(".", "|").Replace("!", "|")
                    .Replace("?", "|").Replace("\n", "|")
                    .Replace("||", "|").Replace("||", "|").Replace("||", "|");

                sb.Append("Sentences: " + x.Split("|", StringSplitOptions.None).Length);

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

        public async Task<bool> ToUpperCase()
        {
            try
            {
                ResultString = (await GetSourceString()).ToUpper();
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        public async Task<bool> ToLowerCase()
        {
            try
            {
                ResultString = (await GetSourceString()).ToLower();
                return await SetResult();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                StatusClass = "alert-danger";
                return false;
            }
        }

        

        public async Task<bool> ToCapitalize()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < SourceString.Length; i++)
                {
                    if (i != 0 && (SourceString[i - 1] == ' ' || Char.IsPunctuation(SourceString, i - 1)))
                        sb.Append(SourceString[i].ToString().ToUpper());
                    else
                    {
                        sb.Append(SourceString[i].ToString());
                    }
                }

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

    }
}
