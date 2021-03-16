<%@ WebHandler Language="C#" Class="Logout" %>

using System;
using System.Web;
using Common.LogicObject;
using System.Web.Security;
using Unity.Attributes;

public class Logout : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    [Dependency]
    public LoginCommonOfBackend LoginCommonOfBackendIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    public void ProcessRequest(HttpContext context)
    {
        if (LoginCommonOfBackendIn == null)
            throw new ArgumentException("LoginCommonOfBackendIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        LoginCommonOfBackend c = LoginCommonOfBackendIn;
        EmployeeAuthorityLogic empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);

        //新增後端操作記錄
        empAuth.InsertBackEndLogData(new BackEndLogData()
        {
            EmpAccount = c.GetEmpAccount(),
            Description = "．登出系統！　．Logged out!",
            IP = c.GetClientIP()
        });

        //登出
        string urlSuffix = "?l=" + c.seLangNoOfBackend.ToString();
        context.Session.Clear();
        FormsAuthentication.SignOut();
        //回到登入頁
        context.Response.Redirect(FormsAuthentication.LoginUrl + urlSuffix);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}