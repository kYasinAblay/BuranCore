using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class FileBasicUploaderExtender
    {
        public static HtmlString FileBasicUploader(this IHtmlHelper html, string name, string title, string uploadUrl,
            string cssClass = "btnFileUploader", bool showImg = false, string imgPath = null, int labelCol = 3, int editorCol = 9)
        {
            var img = showImg ? $"<img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>" : "";
            var div = $@"<div class='form-group row' id='div{name}'>
    <label for='{name}' class='col-form-label col-form-label col-sm-{labelCol}'>{title}</label>
    <div class='col-sm-{editorCol}'>
        {img}
        <button type='button' class='{cssClass} btn btn-sm btn-primary' id='btn{name}'>
            <i class='fas fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='fileinput d-none' data-url='{uploadUrl}'>
    </div>
</div>";
            return new HtmlString(div);
        }
    }
}