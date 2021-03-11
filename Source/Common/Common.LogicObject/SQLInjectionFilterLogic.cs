using Common.DataAccess.EF;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.LogicObject
{
    public class SQLInjectionFilterLogic
    {
        protected ILog logger = null;
        protected string dbErrMsg = "";
        protected IArticlePublisherDataAccess artPubDao;
        public SQLInjectionFilterLogic(IArticlePublisherDataAccess artPubDao)
        {
            if (artPubDao == null)
                throw new ArgumentNullException("artPubDao");

            logger = LogManager.GetLogger(this.GetType());
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
        /// 測試運算式是否成立,能否被用來做 SQL Injection
        /// </summary>
        public bool IsSQLInjectionExpr(string expr)
        {
            bool result = false;

            result = artPubDao.IsSQLInjectionExpr(expr);
            dbErrMsg = artPubDao.GetErrMsg();

            return result;
        }
    }
}
