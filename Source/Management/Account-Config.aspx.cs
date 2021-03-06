using Common.Data.Domain.Model;
using Common.Data.Domain.QueryParam;
using Common.LogicObject;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Account_Config : System.Web.UI.Page
{
    [Dependency]
    public AccountCommonOfBackend AccountCommonOfBackendIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    protected AccountCommonOfBackend c;
    protected EmployeeAuthorityLogic empAuth;

    /// <summary>
    /// 使用嚴格的密碼規則
    /// </summary>
    private bool useStrictPasswordRule = false;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (AccountCommonOfBackendIn == null)
            throw new ArgumentException("AccountCommonOfBackendIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        this.c = AccountCommonOfBackendIn;
        c.InitialLoggerOfUI(this.GetType());

        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);
        empAuth.InitialAuthorizationResultOfSubPages();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Authenticate
            if (c.qsAct == ConfigFormAction.edit && !(empAuth.CanEditThisPage() || c.IsMyAccount())
                || c.qsAct == ConfigFormAction.add && !empAuth.CanAddSubItemInThisPage())
            {
                string jsClose = "closeThisForm();";
                ClientScript.RegisterStartupScript(this.GetType(), "invalid", jsClose, true);
                return;
            }

            LoadUIData();
            DisplayAccountData();
            txtEmpAccount.Focus();
        }
        else
        {
            if (txtPsw.Text.Trim() != "")
            {
                rfvPswConfirm.Enabled = true;
            }
        }

        LoadTitle();
    }

    private void LoadUIData()
    {
        rfvEmpAccount.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        rfvEmpName.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        rfvPsw.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        revPsw.ErrorMessage = "*" + Resources.Lang.ErrMsg_PswNotQualified;
        cuvPsw.ErrorMessage = "*" + Resources.Lang.ErrMsg_PswNotQualified;
        ltrPswComment.Text = "(" + Resources.Lang.Account_PswComment + ")";
        btnGenPsw.ToolTip = Resources.Lang.Account_btnGenPsw_Hint;
        rfvPswConfirm.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        covPswConfirm.ErrorMessage = "*" + Resources.Lang.ErrMsg_PswConfirmNotMatch;
        rfvEmail.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        revEmail.ErrorMessage = "*" + Resources.Lang.ErrMsg_WrongFormat;
        chkIsAccessDenied.Text = Resources.Lang.Account_chkIsAccessDenied;
        rfvStartDate.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        covStartDate.ErrorMessage = "*" + Resources.Lang.ErrMsg_InvalidDate;
        rfvEndDate.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        covEndDate.ErrorMessage = "*" + Resources.Lang.ErrMsg_InvalidDate;

        if (c.seLangNoOfBackend != 1)
        {
            Master.EnableDatepickerTW = false;
        }

        InitPasswordRule();
        LoadDeptUIData();
        LoadRolesUIData();
    }

    private void InitPasswordRule()
    {
        if (useStrictPasswordRule)
        {
            cuvPsw.Enabled = true;
            PswRuleNotice.InnerHtml = Resources.Lang.PswRuleNotice_StrictRule;
        }
        else
        {
            revPsw.Enabled = true;
            revPsw.ValidationExpression = StringUtility.GetPswSimpleRuleValidationExpression();
            PswRuleNotice.InnerHtml = Resources.Lang.PswRuleNotice_LooseRule;
        }
    }

    private void LoadDeptUIData()
    {
        ddlDept.Items.Clear();
        List<Department> depts = empAuth.GetDepartmentListToSelect();

        if (depts != null)
        {
            ddlDept.DataTextField = "DeptName";
            ddlDept.DataValueField = "DeptId";
            ddlDept.DataSource = depts;
            ddlDept.DataBind();
        }
    }

    private void LoadRolesUIData()
    {
        ddlRoles.Items.Clear();
        List<EmployeeRoleToSelect> roles = empAuth.GetEmployeeRoleListToSelect();

        if (roles != null)
        {
            ddlRoles.DataTextField = "DisplayText";
            ddlRoles.DataValueField = "RoleId";
            ddlRoles.DataSource = roles;
            ddlRoles.DataBind();

            // move admin to last
            ListItem liAdmin = ddlRoles.Items.FindByValue("1");
            if (liAdmin != null)
            {
                ddlRoles.Items.Remove(liAdmin);
                ddlRoles.Items.Add(liAdmin);
            }
        }
    }

    private void LoadTitle()
    {
        if (c.qsAct == ConfigFormAction.add)
            Title = Resources.Lang.Account_Title_AddNew;
        else if (c.qsAct == ConfigFormAction.edit)
            Title = string.Format(Resources.Lang.Account_Title_Edit_Format, c.qsEmpId);
    }

    private void DisplayAccountData()
    {
        bool isOwner = false;
        int curRoleId = 0;

        if (c.qsAct == ConfigFormAction.edit)
        {
            EmployeeForBackend account = empAuth.GetEmployeeData(c.qsEmpId);

            if (account != null)
            {
                string empAccount = account.EmpAccount;

                //account
                txtEmpAccount.Text = account.EmpAccount;
                txtEmpAccount.Enabled = false;

                //name
                txtEmpName.Text = account.EmpName;

                //password
                rfvPsw.Enabled = false;
                hidEmpPasswordOri.Text = account.EmpPassword;
                hidPasswordHashed.Text = account.PasswordHashed.ToString();
                hidDefaultRandomPassword.Text = account.DefaultRandomPassword;

                //email
                txtEmail.Text = account.Email;

                //remarks
                txtRemarks.Text = account.Remarks;

                // is access denied
                chkIsAccessDenied.Checked = account.IsAccessDenied;
                ltrIsAccessDenied.Text = chkIsAccessDenied.Checked ? Resources.Lang.Account_IsAccessDenied_Checked : Resources.Lang.Account_IsAccessDenied_Unchecked;

                //valid date
                txtStartDate.Text = string.Format("{0:yyyy-MM-dd}", account.StartDate.Value);
                txtEndDate.Text = string.Format("{0:yyyy-MM-dd}", account.EndDate.Value);
                ltrDateRange.Text = txtStartDate.Text + " ~ " + txtEndDate.Text;

                if (empAccount == "admin")
                {
                    DateRangeArea.Visible = false;
                }

                //department
                ddlDept.SelectedValue = account.DeptId.ToString();
                if (ddlDept.SelectedItem != null)
                    ltrDept.Text = ddlDept.SelectedItem.Text;

                //role
                curRoleId = account.RoleId;
                ddlRoles.SelectedValue = curRoleId.ToString();
                ltrRoles.Text = account.RoleDisplayText;

                //owner
                txtOwnerAccount.Text = account.OwnerAccount;
                ltrOwnerAccount.Text = txtOwnerAccount.Text;

                isOwner = empAuth.CanEditThisPage(false, account.OwnerAccount, account.OwnerDeptId);

                //modification info
                ltrPostAccount.Text = account.PostAccount;
                ltrPostDate.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", account.PostDate);

                if (account.MdfDate.HasValue)
                {
                    ltrMdfAccount.Text = account.MdfAccount;
                    ltrMdfDate.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", account.MdfDate.Value);
                }

                btnSave.Visible = true; 
            }
        }
        else
        {
            //add

            txtStartDate.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
            DateTime endDate = DateTime.Today.AddYears(10);
            txtEndDate.Text = string.Format("{0:yyyy-MM-dd}", endDate);

            txtOwnerAccount.Text = c.GetEmpAccount();
            ltrOwnerAccount.Text = txtOwnerAccount.Text;

            isOwner = true;

            btnSave.Visible = true;
        }

        // owner privilege
        if (isOwner)
        {
            chkIsAccessDenied.Visible = true;
            ltrIsAccessDenied.Visible = false;

            DateRangeEditCtrl.Visible = true;
            ltrDateRange.Visible = false;

            ddlDept.Visible = true;
            ltrDept.Visible = false;

            ddlRoles.Visible = true;
            ltrRoles.Visible = false;
        }

        // role-admin privilege
        if (c.IsInRole("admin"))
        {
            //owner
            txtOwnerAccount.Visible = true;
            ltrOwnerAccount.Visible = false;
        }
        else
        {
            // only role-admin can assigns role-admin to another (但是,保留已經是role-admin的選項)
            if (curRoleId != 1)
            {
                ListItem liAdmin = ddlRoles.Items.FindByValue("1");
                if (liAdmin != null)
                {
                    ddlRoles.Items.Remove(liAdmin);
                }
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        try
        {
            txtEmpAccount.Text = txtEmpAccount.Text.Trim();
            txtPsw.Text = txtPsw.Text.Trim();
            string passwordValue = hidEmpPasswordOri.Text;
            bool passwordHashed = Convert.ToBoolean(hidPasswordHashed.Text);

            if (txtPsw.Text != "")
            {
                passwordValue = HashUtility.GetPasswordHash(txtPsw.Text);
                passwordHashed = true;
            }

            txtEmpName.Text = txtEmpName.Text.Trim();
            txtEmail.Text = txtEmail.Text.Trim();
            txtRemarks.Text = txtRemarks.Text.Trim();
            txtOwnerAccount.Text = txtOwnerAccount.Text.Trim();

            AccountParams param = new AccountParams()
            {
                EmpAccount = txtEmpAccount.Text,
                EmpPassword = passwordValue,
                EmpName = txtEmpName.Text,
                Email = txtEmail.Text,
                Remarks = txtRemarks.Text,
                DeptId = Convert.ToInt32(ddlDept.SelectedValue),
                RoleId = Convert.ToInt32(ddlRoles.SelectedValue),
                IsAccessDenied = chkIsAccessDenied.Checked,
                StartDate = Convert.ToDateTime(txtStartDate.Text),
                EndDate = Convert.ToDateTime(txtEndDate.Text),
                OwnerAccount = txtOwnerAccount.Text,
                PasswordHashed = passwordHashed,
                DefaultRandomPassword = hidDefaultRandomPassword.Text,
                PostAccount = c.GetEmpAccount()
            };

            bool result = false;

            if (c.qsAct == ConfigFormAction.add)
            {
                result = empAuth.InsertEmployeeData(param);

                if(!result)
                {
                    if (param.HasAccountBeenUsed)
                    {
                        Master.ShowErrorMsg(Resources.Lang.ErrMsg_AccountHasBeenUsed);
                    }
                    else
                    {
                        Master.ShowErrorMsg(Resources.Lang.ErrMsg_AddFailed);
                    }
                }
            }
            else if (c.qsAct == ConfigFormAction.edit)
            {
                param.EmpId = c.qsEmpId;
                result = empAuth.UpdateEmployeeData(param);

                if (!result)
                {
                    Master.ShowErrorMsg(Resources.Lang.ErrMsg_UpdateFailed);
                }
            }

            if (result)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", StringUtility.GetNoticeOpenerJs("Config"), true);
            }

            //新增後端操作記錄
            empAuth.InsertBackEndLogData(new BackEndLogData()
            {
                EmpAccount = c.GetEmpAccount(),
                Description = string.Format("．{0}　．儲存帳號/Save account[{1}]　EmpId[{2}]　結果/result[{3}]", Title, txtEmpAccount.Text, param.EmpId, result),
                IP = c.GetClientIP()
            });
        }
        catch (Exception ex)
        {
            c.LoggerOfUI.Error("", ex);
            Master.ShowErrorMsg(ex.Message);
        }
    }

    protected void cuvPsw_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool isValidPsw = true;

        //內文至少包含下列種類

        //特殊符號
        if (!Regex.IsMatch(args.Value, @"[~`!@#$%^&*()\-_=+,\.<>;':""\[\]{}\\|?/]{1,1}"))
        {
            isValidPsw = false;
        }

        //英文字母大寫
        if (!Regex.IsMatch(args.Value, @"[A-Z]+"))
        {
            isValidPsw = false;
        }

        //及小寫
        if (!Regex.IsMatch(args.Value, @"[a-z]+"))
        {
            isValidPsw = false;
        }

        //數字
        if (!Regex.IsMatch(args.Value, @"[0-9]+"))
        {
            isValidPsw = false;
        }

        //最少12字
        if (args.Value.Length < 12)
        {
            isValidPsw = false;
        }

        args.IsValid = isValidPsw;
    }

    protected void btnGenPsw_Click(object sender, EventArgs e)
    {
        txtPsw.TextMode = TextBoxMode.SingleLine;
        txtPswConfirm.TextMode = TextBoxMode.SingleLine;

        if (useStrictPasswordRule)
            txtPsw.Text = StringUtility.GenerateStrictPasswordValue(12);
        else
            txtPsw.Text = StringUtility.GenerateLoosePasswordValue(10);

        txtPswConfirm.Text = txtPsw.Text;
        txtPsw.ReadOnly = true;
        PswConfirmArea.Visible = false;

        ////密碼暫時另存一份在備註
        //string remarkWoOldPsw = Regex.Replace(txtRemarks.Text, Resources.Lang.Account_lblDefaultPsw + @"[!@#$%^&*0-9A-Za-z]+", "");
        //txtRemarks.Text = Resources.Lang.Account_lblDefaultPsw + txtPsw.Text + remarkWoOldPsw;

        hidDefaultRandomPassword.Text = txtPsw.Text;
    }
}