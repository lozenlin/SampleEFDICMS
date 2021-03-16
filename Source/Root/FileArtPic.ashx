<%@ WebHandler Language="C#" Class="FileArtPic" %>

using System;
using System.Web;
using Common.LogicObject;
using Unity.Attributes;

/// <summary>
/// 網頁照片下載
/// </summary>
public class FileArtPic : IHttpHandler
{
    [Dependency]
    public ArtPicDownloadCommon ArtPicDownloadCommonIn { get; set; }

    protected ArtPicDownloadCommon c;

    public void ProcessRequest(HttpContext context)
    {
        if (ArtPicDownloadCommonIn == null)
            throw new ArgumentException("ArtPicDownloadCommonIn");

        this.c = ArtPicDownloadCommonIn;

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