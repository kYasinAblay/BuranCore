namespace Buran.Core.MvcLibrary.Utils
{
    public static class HtmlHelperUtil
    {
        public static string RedirectForm(string url)
        {
            var formPost = string.Format(@"<html>
<head>
<meta http-equiv=""refresh"" content=""0; url={0}"" />
</head>
<body>
    <script language=""javascript"">
        window.location = ""{0}"";
    </script>
</body>
</html>", url);
            return formPost;
        }

        public static string GetEmailDomain(this string email)
        {
            var words = email.Split('@');
            return words.Length == 2 ? words[1] : null;
        }

        //public static IHtmlHelper GetHtmlHelper(this Controller controller)
        //{
        //    var viewContext = new ViewContext(controller.ControllerContext,
        //        new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
        //    return new IHtmlHelper(viewContext, new ViewPage());
        //}

        //public static AjaxHelper GetAjaxHelper(this Controller controller)
        //{
        //    var viewContext = new ViewContext(controller.ControllerContext,
        //        new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
        //    return new AjaxHelper(viewContext, new ViewPage());
        //}
        //public class FakeView : IView
        //{
        //    public void Render(ViewContext viewContext, TextWriter writer)
        //    {
        //        throw new InvalidOperationException();
        //    }
        //}
    }
}
