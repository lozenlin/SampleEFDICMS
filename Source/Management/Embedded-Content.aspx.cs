using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Embedded_Content : BasePage
{
    [Dependency]
    public EmbeddedContentCommonOfBackend EmbeddedContentCommonOfBackendIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    protected EmbeddedContentCommonOfBackend c;
    protected EmployeeAuthorityLogic empAuth;
    private IHeadUpDisplay hud = null;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (EmbeddedContentCommonOfBackendIn == null)
            throw new ArgumentException("EmbeddedContentCommonOfBackendIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        this.c = EmbeddedContentCommonOfBackendIn;
        c.InitialLoggerOfUI(this.GetType());
        c.SelectMenuItemToThisPage();

        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);
        empAuth.InitialAuthorizationResultOfTopPage();

        hud = Master.GetHeadUpDisplay();
        isBackendPage = true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        hud.RebuildBreadcrumbAndUpdateHead(c.GetOpIdOfPage(), empAuth);
        Title = hud.GetHeadText() + " - " + Title;

        if (!IsPostBack)
        {
            if (!empAuth.CanOpenThisPage())
            {
                Response.Redirect(c.BACK_END_HOME);
            }

            LoadUIData();
        }
    }

    private void LoadUIData()
    {
        btnInNewWindow.Title = Resources.Lang.Col_OpenInNewWindow_Hint;
        string decodedUrl = c.DecodeUrlOfMenu(c.qsUrl);

        if (decodedUrl == ""
            || string.Compare(decodedUrl, "http://", true) == 0
            || string.Compare(decodedUrl, "https://", true) == 0
            || decodedUrl.StartsWith("http://#", StringComparison.CurrentCultureIgnoreCase)
            || decodedUrl.StartsWith("https://#", StringComparison.CurrentCultureIgnoreCase))
        {
            EmbeddedContent.Visible = false;
            SubControlsArea.Visible = false;
        }
        else
        {
            EmbeddedContent.Src = decodedUrl;
            btnInNewWindow.HRef = decodedUrl;
        }
    }
}