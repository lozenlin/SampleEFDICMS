<%@ WebHandler Language="C#" Class="ToSearchResult" %>

using System;
using System.Web;
using Common.LogicObject;
using Unity.Attributes;

public class ToSearchResult : IHttpHandler
{
    [Dependency]
    public SearchPageCommon SearchPageCommonIn { get; set; }

    protected SearchPageCommon c;
    protected HttpContext context;

    #region 工具屬性

    protected HttpServerUtility Server
    {
        get { return context.Server; }
    }

    protected HttpRequest Request
    {
        get { return context.Request; }
    }

    protected HttpResponse Response
    {
        get { return context.Response; }
    }

    #endregion

    public void ProcessRequest(HttpContext context)
    {
        if (SearchPageCommonIn == null)
            throw new ArgumentException("SearchPageCommonIn");

        this.c = SearchPageCommonIn;
        c.InitialLoggerOfUI(this.GetType());

        this.context = context;

        string keyWord = c.qsQueryKeyword.Trim();
        c.GoToSearchResult(keyWord);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}