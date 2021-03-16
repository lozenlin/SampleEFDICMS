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

        //使用 Client Cache
        context.Response.Cache.SetCacheability(HttpCacheability.Private);    // Private:Client, Public:Server+Proxy+Client, Server:Client No-Cache
        context.Response.Cache.VaryByParams["attid"] = true;
        context.Response.Cache.VaryByParams["w"] = true;
        context.Response.Cache.VaryByParams["h"] = true;
        context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));    // for Client

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