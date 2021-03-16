using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class UserControls_wucSearchCondition : System.Web.UI.UserControl
{
    [Dependency]
    public SearchPageCommon SearchPageCommonIn { get; set; }

    protected SearchPageCommon c;

    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (SearchPageCommonIn == null)
            throw new ArgumentException("SearchPageCommonIn");

        this.c = SearchPageCommonIn;
        c.InitialLoggerOfUI(this.GetType());
    }
}