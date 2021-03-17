<%@ WebHandler Language="C#" Class="FileAttView" %>

using System;
using System.Web;
using Common.LogicObject;
using Unity.Attributes;

/// <summary>
/// 附件檔案(以直接檢視的方式)下載
/// </summary>
public class FileAttView : IHttpHandler
{
    [Dependency]
    public AttViewDownloadCommon AttViewDownloadCommonIn { get; set; }

    protected AttViewDownloadCommon c;

    public void ProcessRequest(HttpContext context)
    {
        if (AttViewDownloadCommonIn == null)
            throw new ArgumentException("AttViewDownloadCommonIn");

        this.c = AttViewDownloadCommonIn;

        c.IsInBackend = true;

        if (!c.ProcessRequest())
        {
            context.Response.Redirect("ErrorPage.aspx", true);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}