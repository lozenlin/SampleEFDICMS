using Common.LogicObject.DataAccessInterfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogicObject
{
    /// <summary>
    /// SQL Injection 過濾加強版,可測試運算式
    /// </summary>
    public class SQLInjectionFilterExt : SQLInjectionFilter
    {
        protected string dbErrMsg = "";
        protected IArticlePublisherDataAccess artPubDao;

        /// <summary>
        /// SQL Injection 過濾加強版,可測試運算式
        /// </summary>
        public SQLInjectionFilterExt(IArticlePublisherDataAccess artPubDao)
            : base()
        {
            if (artPubDao == null)
                throw new ArgumentNullException("artPubDao");

            this.artPubDao = artPubDao;
        }

        // DataAccess functions

        /// <summary>
        /// DB command 執行後的錯誤訊息
        /// </summary>
        public string GetDbErrMsg()
        {
            return dbErrMsg;
        }

        /// <summary>
        /// 測試運算式是否成立,能否被用來做SQL Injection
        /// </summary>
        protected override bool IsSQLInjectionExpr(string expr)
        {
            bool result = false;

            result = artPubDao.IsSQLInjectionExpr(expr);
            dbErrMsg = artPubDao.GetErrMsg();

            return result;
        }
    }
}
