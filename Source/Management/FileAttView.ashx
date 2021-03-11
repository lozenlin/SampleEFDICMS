﻿<%@ WebHandler Language="C#" Class="FileAttView" %>

using System;
using System.Web;
using Common.LogicObject;

/// <summary>
/// 附件檔案(以直接檢視的方式)下載
/// </summary>
public class FileAttView : IHttpHandler
{
    protected AttViewDownloadCommon c;

    public void ProcessRequest(HttpContext context)
    {
        c = new AttViewDownloadCommon(context, null, new ArticlePublisherLogic(null, new Common.DataAccess.EF.ArticlePublisherDataAccess(), new Common.DataAccess.EF.EmployeeAuthorityDataAccess()));

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