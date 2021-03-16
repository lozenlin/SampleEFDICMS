﻿using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_wucSearchCondition : System.Web.UI.UserControl
{
    protected SearchPageCommon c;

    protected void Page_Init(object sender, EventArgs e)
    {
        c = new SearchPageCommon(this.Context, new ArticlePublisherLogic(null, new Common.DataAccess.EF.ArticlePublisherDataAccess(), new Common.DataAccess.EF.EmployeeAuthorityDataAccess()));
        c.InitialLoggerOfUI(this.GetType());
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}