using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogicObject
{
    /// <summary>
    /// Null object of IAuthenticationConditionProvider
    /// </summary>
    /// 2021/03/15, lozenlin, add
    public class NullAuthenticationConditionProvider : IAuthenticationConditionProvider
    {
        public Guid GetArticleId()
        {
            return Guid.Empty;
        }

        public int GetDeptId()
        {
            return 0;
        }

        public string GetEmpAccount()
        {
            return null;
        }

        public int GetOpIdOfPage()
        {
            return 0;
        }

        public string GetRoleName()
        {
            return null;
        }

        public bool IsInRole(string roleName)
        {
            return false;
        }
    }
}
