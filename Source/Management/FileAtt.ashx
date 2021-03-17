<%@ WebHandler Language="C#" Class="FileAtt" %>

using System;
using System.Web;
using Common.LogicObject;
using Unity.Attributes;

/// <summary>
/// 附件檔案下載
/// </summary>
public class FileAtt : IHttpHandler
{
    [Dependency]
    public AttDownloadCommon AttDownloadCommonIn { get; set; }

    protected AttDownloadCommon c;

    public void ProcessRequest(HttpContext context)
    {
        if (AttDownloadCommonIn == null)
            throw new ArgumentException("AttDownloadCommonIn");

        this.c = AttDownloadCommonIn;

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