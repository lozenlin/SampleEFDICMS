using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Dashboard : System.Web.UI.Page
{
    [Dependency]
    public BackendPageCommon BackendPageCommonIn { get; set; }

    protected BackendPageCommon c;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (BackendPageCommonIn == null)
            throw new ArgumentException("BackendPageCommonIn");

        this.c = BackendPageCommonIn;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadUIData();
        }
    }

    private void LoadUIData()
    {
        Master.SetHeadUpDisplayVisible(false);
        IHeadUpDisplay hud = Master.GetHeadUpDisplay();
        hud.SetHeadText("Dashboard");

        if (c.IsAuthenticated())
        {
            LoginEmployeeData d = c.seLoginEmpData;
            ltrEmpAccount.Text = d.EmpAccount;

            if (d.ThisLoginTime != DateTime.MinValue)
            {
                ltrThisLoginTime.Text = string.Format("{0:yyyy-MM-dd HH:mm}", d.ThisLoginTime);
                ltrThisLoginIP.Text = d.ThisLoginIP;
            }

            if (d.LastLoginTime != DateTime.MinValue)
            {
                ltrLastLoginTime.Text = string.Format("{0:yyyy-MM-dd HH:mm}", d.LastLoginTime);
                ltrLastLoginIP.Text = d.LastLoginIP;
            }
        }
        else
        {
        }

        LoadSystemVersion();
    }

    private void LoadSystemVersion()
    {
        try
        {
            System.Reflection.Assembly asmAppCode = System.Reflection.Assembly.Load("App_Code");
            ltrSystemVersion.Text = asmAppCode.GetName().Version.ToString();

            System.Reflection.Assembly asmLogicObject = System.Reflection.Assembly.Load("Common.LogicObject");
            ltrLogicObjectVersion.Text = asmLogicObject.GetName().Version.ToString();

            System.Reflection.Assembly asmDataAccess = System.Reflection.Assembly.Load("Common.DataAccess.EF");
            ltrDataAccessVersion.Text = asmDataAccess.GetName().Version.ToString();

            System.Reflection.Assembly asmUtility = System.Reflection.Assembly.Load("Common.Utility");
            ltrUtilityVersion.Text = asmUtility.GetName().Version.ToString();

            System.Reflection.Assembly asmDataDomain = System.Reflection.Assembly.Load("Common.Data.Domain");
            ltrDataDomainVersion.Text = asmDataDomain.GetName().Version.ToString();
        }
        catch { }
    }
}