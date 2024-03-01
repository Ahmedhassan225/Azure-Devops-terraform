using Infrastructure.Middleware.Exceptions.Common;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Application.Helpers.PdfHelper
{
    public class PdfHelper : IPdfHelper
    {
        private readonly IConverter _converter;

        public PdfHelper(IConverter converter)
        {
            _converter = converter;
        }
        public byte[] ConvertHtmlToPdf(string html)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true }
                    }
                }
            };

            var pdf = _converter.Convert(doc);

            return pdf;
        }
        public string GenerateHtmlTable<T>(List<T> dataSource, string title)
        {
            if (dataSource is null || dataSource.Count == 0)
                throw new CustomException("Validation Error, Empty Array", statusCode: HttpStatusCode.BadRequest);

            StringBuilder htmlBuilder = new();

            // Start HTML content
            htmlBuilder.Append("<!DOCTYPE html><html><head><style>");
            //css styles here
            htmlBuilder.Append(" .styled-table { border-collapse: collapse; margin: 25px0; width: 100%; font-size: 0.9em; font-family: sans-serif; min-width: 400px; box-shadow: 0020pxrgba(0,0,0,0.15); } .styled-table thead tr { background-color: #3f51b5; color: #ffffff; height :2rem; text-align: left; } .styled-table thead tr td { padding-right:2rem; } .styled-table tbody tr { border-bottom: 1pxsolid#dddddd; } .styled-table tbody tr:nth-of-type(even) { background-color: #f3f3f3; } .styled-table tbody tr:last-of-type{ border-bottom: 2pxsolid#3f51b5; }");
            htmlBuilder.Append($"</style></head><body><h2>{title}</h2><table class='styled-table'>");


            Type type = dataSource.FirstOrDefault().GetType();

            // Get all properties of the object
            PropertyInfo[] properties = type.GetProperties();

            // Add table header
            htmlBuilder.Append("<thead><tr>");
            foreach (var property in properties)
            {
                // Get the name of the property
                string propertyName = property.Name;
                htmlBuilder.Append($"<td>{propertyName.ToString()}</td>");
            }
            htmlBuilder.Append("</tr></thead>");

            htmlBuilder.Append("<tbody>");
            // Add table rows dynamically based on data
            foreach (var item in dataSource)
            {
                // Loop through each property to add columns
                htmlBuilder.Append("<tr>");
                foreach (var property in properties)
                {

                    // Get the value of the property
                    object propertyValue = property.GetValue(item) ?? string.Empty;

                    htmlBuilder.Append($"<td>{propertyValue.ToString()}</td>");
                }
                htmlBuilder.Append("</tr>");
            }
            htmlBuilder.Append("</tbody>");


            // End HTML content
            htmlBuilder.Append("</table></body></html>");

            var res = htmlBuilder.ToString();
            return res;
        }
    }
}
