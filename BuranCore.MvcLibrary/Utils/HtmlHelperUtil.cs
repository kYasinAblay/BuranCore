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
    }
}
