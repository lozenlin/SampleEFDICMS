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