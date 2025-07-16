using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNaati.Ui.Security
{
    public class EncodeScriptOnlyAttribute : Attribute
    {
        //no code. Is a token for decorating a mthod to be picked up by HtmlEncodeFilter set up in Global.asax.cs
    }

    public class EncodeIgnoreAttribute : Attribute
    {
        //no code. Is a token for decorating a mthod to be picked up by HtmlEncodeFilter set up in Global.asax.cs
    }

    public class EncodeHtmlOnlyAttribute : Attribute
    {
        //no code. Is a token for decorating a mthod to be picked up by HtmlEncodeFilter set up in Global.asax.cs
    }

    public class OnEncodeIssueFoundThrowErrorAttribute : Attribute
    {
        //no code. Is a token for decorating a mthod to be picked up by HtmlEncodeFilter set up in Global.asax.cs
    }
}