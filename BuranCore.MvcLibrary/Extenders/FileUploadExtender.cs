using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class FileUploaderExtender
    {
        [Obsolete]
        public static HtmlString FileUploader(this IHtmlHelper html, string name, string title, string imgPath, string uploadUrl, string cssClass = "btnFileUploader")
        {
            var div = $@"<div class='form-group' id='div{name}'>
    <label for='{name}' class='col-sm-3 control-label'>{title}</label>
    <div class='col-sm-8'>
        <img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>
        <button type='button' class='{cssClass} btn btn-xs btn-default' id='btn{name}'>
            <i class='fa fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='fileinput hide' data-url='{uploadUrl}'>
    </div>
</div>";
            return new HtmlString(div);
        }

        //        [Obsolete]
        //        public static HtmlString FileUploader(this IHtmlHelper html, string name, string title, string uploadUrl,
        //            string cssClass = "btnFileUploader", bool showImg = false, string imgPath = null, int labelCol = 3, int editorCol = 9)
        //        {
        //            var img = showImg ? $"<img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>" : "";
        //            var div = $@"<div class='form-group row' id='div{name}'>
        //    <label for='{name}' class='col-form-label col-form-label col-sm-{labelCol}'>{title}</label>
        //    <div class='col-sm-{editorCol}'>
        //        {img}
        //        <button type='button' class='{cssClass} btn btn-sm btn-primary' id='btn{name}'>
        //            <i class='fas fa-upload'></i> {UI.Upload}
        //        </button>
        //        <input id='file{name}' type='file' name='file{name}' class='fileinput d-none' data-url='{uploadUrl}'>
        //    </div>
        //</div>";
        //            return new HtmlString(div);
        //        }

        public static HtmlString FileUploaderNoImage(this IHtmlHelper html, string name, string title, string uploadUrl,
            string cssClass = "btnFileUploader", string fileCssClass = "fileinput",
            string labelCss = "col-sm-3", string editorCss = "col-sm-8", string buttonCss = "btn-xs")
        {
            var div = $@"<div class='form-group' id='div{name}'>
    <label for='{name}' class='{labelCss} control-label'>{title}</label>
    <div class='{editorCss}'>
        <button type='button' class='{cssClass} btn {buttonCss} btn-default' id='btn{name}'>
            <i class='fa fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='{fileCssClass} hide' data-url='{uploadUrl}'>
    </div>
</div>";
            return new HtmlString(div);
        }

        public static long GetImageTempId()
        {
            return -1 * DateTime.Now.Ticks;
        }

        [Obsolete]
        public static HtmlString FileUploader2(this IHtmlHelper html, string name, string imgPath, string uploadUrl, string cssClass = "btnFileUploader")
        {
            var div = $@"<img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>
        <button type='button' class='{cssClass} btn btn-xs btn-default' id='btn{name}'>
            <i class='fa fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='fileinput hide' data-url='{uploadUrl}'>";
            return new HtmlString(div);
        }


        public static HtmlString FileUploader4(this IHtmlHelper html, string name, string title, string uploadUrl,
            string cssClass = "btnFileUploader", bool showImg = false, string imgPath = null, int labelCol = 3, int editorCol = 9)
        {
            var img = showImg ? $"<img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>" : "";
            var div = $@"<div class='form-group row' id='div{name}'>
    <label for='{name}' class='col-form-label col-form-label col-sm-{labelCol}'>{title}</label>
    <div class='col-sm-{editorCol}'>
        {img}
        <button type='button' class='{cssClass} btn btn-sm btn-label-primary' id='btn{name}'>
            <i class='fas fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='fileinput d-none' data-url='{uploadUrl}'>
    </div>
</div>";
            return new HtmlString(div);
        }

        public static HtmlString FileUploader5(this IHtmlHelper html, string name, string title, string imgPath, string uploadUrl,
            string cssClass = "btnFileUploader", int labelCol = 3, int editorCol = 9, bool template = true)
        {
            if (template)
            {
                var div = $@"<div class='form-group row' id='div{name}'>
    <label for='{name}' class='col-sm-{labelCol} col-form-label col-form-label-sm'>{title}</label>
    <div class='col-sm-{editorCol}'>
        <img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>
        <button type='button' class='{cssClass} btn btn-sm btn-label-default' id='btn{name}'>
            <i class='fa fa-upload'></i> {UI.Upload}
        </button>
        <input id='file{name}' type='file' name='file{name}' class='fileinput d-none' data-url='{uploadUrl}'>
    </div>
</div>";
                return new HtmlString(div);
            }
            else
            {
                var div = $@"
<img id='img{name}' src='{imgPath}' class='img-thumbnail img-fileupload'><br>
<button type='button' class='{cssClass} btn btn-sm btn-label-default' id='btn{name}'>
    <i class='fa fa-upload'></i> {UI.Upload}
</button>
<input id='file{name}' type='file' name='file{name}' class='fileinput d-none' data-url='{uploadUrl}'>";
                return new HtmlString(div);
            }
        }
    }
}