using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ErrorPage : System.Web.UI.Page
{
    protected PageCommon c;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        c = new PageCommon(this.Context);
        c.InitialLoggerOfUI(this.GetType());
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }
}