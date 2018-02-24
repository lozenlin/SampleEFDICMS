﻿using Common.DataAccess;
using Common.DataAccess.EF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.LogicObject
{
    public class RolePrivilegeParams
    {
        public string RoleName;
        public List<RoleOpPvg> pvgChanges;
        public string PostAccount;

        public RolePrivilegeParams()
        {
            pvgChanges = new List<RoleOpPvg>();
        }

        public List<EmployeeRoleOperationsDesc> GenEmpRoleOps()
        {
            List<EmployeeRoleOperationsDesc> empRoleOps = pvgChanges.Select(c =>
            {
                EmployeeRoleOperationsDesc ro = new EmployeeRoleOperationsDesc()
                {
                    RoleName = c.RoleName,
                    OpId = c.OpId,
                    PostAccount = this.PostAccount,
                    PostDate = DateTime.Now,
                    MdfAccount = this.PostAccount,
                    MdfDate = DateTime.Now
                };

                ro.CanRead = (c.PvgOfItem & 1) == 1;
                ro.CanEdit = (c.PvgOfItem & 2) == 2;

                ro.CanReadSubItemOfSelf = (c.PvgOfSubitemSelf & 1) == 1;
                ro.CanEditSubItemOfSelf = (c.PvgOfSubitemSelf & 2) == 2;
                ro.CanAddSubItemOfSelf = (c.PvgOfSubitemSelf & 4) == 4;
                ro.CanDelSubItemOfSelf = (c.PvgOfSubitemSelf & 8) == 8;

                ro.CanReadSubItemOfCrew = (c.PvgOfSubitemCrew & 1) == 1;
                ro.CanEditSubItemOfCrew = (c.PvgOfSubitemCrew & 2) == 2;
                ro.CanDelSubItemOfCrew = (c.PvgOfSubitemCrew & 8) == 8;

                ro.CanReadSubItemOfOthers = (c.PvgOfSubitemOthers & 1) == 1;
                ro.CanEditSubItemOfOthers = (c.PvgOfSubitemOthers & 2) == 2;
                ro.CanDelSubItemOfOthers = (c.PvgOfSubitemOthers & 8) == 8;

                return ro;
            }).ToList();


            return empRoleOps;
        }

        public List<RoleOpDescParamsDA> GetRoleOpsOfDA()
        {
            List<RoleOpDescParamsDA> roleOps;

            roleOps = pvgChanges.Select(c =>
            {
                RoleOpDescParamsDA roleOp = new RoleOpDescParamsDA()
                {
                    RoleName = c.RoleName,
                    OpId = c.OpId,
                    PostAccount = this.PostAccount
                };

                roleOp.CanRead = (c.PvgOfItem & 1) == 1;
                roleOp.CanEdit = (c.PvgOfItem & 2) == 2;

                roleOp.CanReadSubItemOfSelf = (c.PvgOfSubitemSelf & 1) == 1;
                roleOp.CanEditSubItemOfSelf = (c.PvgOfSubitemSelf & 2) == 2;
                roleOp.CanAddSubItemOfSelf = (c.PvgOfSubitemSelf & 4) == 4;
                roleOp.CanDelSubItemOfSelf = (c.PvgOfSubitemSelf & 8) == 8;

                roleOp.CanReadSubItemOfCrew = (c.PvgOfSubitemCrew & 1) == 1;
                roleOp.CanEditSubItemOfCrew = (c.PvgOfSubitemCrew & 2) == 2;
                roleOp.CanDelSubItemOfCrew = (c.PvgOfSubitemCrew & 8) == 8;

                roleOp.CanReadSubItemOfOthers = (c.PvgOfSubitemOthers & 1) == 1;
                roleOp.CanEditSubItemOfOthers = (c.PvgOfSubitemOthers & 2) == 2;
                roleOp.CanDelSubItemOfOthers = (c.PvgOfSubitemOthers & 8) == 8;

                return roleOp;
            }).ToList();

            return roleOps;
        }
    }
}
