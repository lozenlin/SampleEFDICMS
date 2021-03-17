<%@ WebHandler Language="C#" Class="jsonService" %>

using System;
using System.Web;
using Common.LogicObject;
using System.Web.SessionState;
using Newtonsoft.Json;
using JsonService;
using Unity.Attributes;

public class jsonService : IHttpHandler, IRequiresSessionState
{
    [Dependency]
    public RoleCommonOfBackend RoleCommonOfBackendIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }
    [Dependency]
    public ArticlePublisherLogic ArticlePublisherLogicIn { get; set; }

    protected RoleCommonOfBackend c;
    protected EmployeeAuthorityLogic empAuth;
    protected ArticlePublisherLogic artPub;
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

    protected HttpSessionState Session
    {
        get { return context.Session; }
    }

    #endregion

    private bool allowedParamFromQueryString = false;   // true for testing

    public void ProcessRequest(HttpContext context)
    {
        if (RoleCommonOfBackendIn == null)
            throw new ArgumentException("RoleCommonOfBackendIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        if (ArticlePublisherLogicIn == null)
            throw new ArgumentException("ArticlePublisherLogicIn");

        this.c = RoleCommonOfBackendIn;
        c.InitialLoggerOfUI(this.GetType());
        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);
        this.artPub = ArticlePublisherLogicIn;

        this.context = context;

        string serviceName = GetParamValue("serviceName");
        ClientResult clientResult = null;

        try
        {
            IJsonServiceHandler handler = JsonServiceHandlerFactory.GetHandler(context, serviceName, c, empAuth, artPub);

            if (handler == null)
                throw new Exception("service name is invalid");

            clientResult = handler.ProcessRequest();
        }
        catch (Exception ex)
        {
            c.LoggerOfUI.Error("", ex);

            clientResult = new ClientResult() { b = false, err = ex.Message };
        }

        string result = JsonConvert.SerializeObject(clientResult);

        Response.ContentType = "text/plain";
        Response.Write(result);
    }

    private string GetParamValue(string name)
    {
        if (allowedParamFromQueryString)
            return GetSafeStringExtensions.ToSafeStr(Request[name]);

        return c.FormToSafeStr(name);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
