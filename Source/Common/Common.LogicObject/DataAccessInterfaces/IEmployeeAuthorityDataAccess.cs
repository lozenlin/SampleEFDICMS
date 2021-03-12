// ===============================================================================
// EmployeeAuthorityDataAccess of SampleEFDICMS
// https://github.com/lozenlin/SampleEFDICMS
//
// EmployeeAuthorityDataAccess.cs
//
// ===============================================================================
// Copyright (c) 2019 lozenlin
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// ===============================================================================

using Common.Data.Domain.Model;
using Common.Data.Domain.QueryParam;
using System.Collections.Generic;

namespace Common.LogicObject.DataAccessInterfaces
{
    public interface IEmployeeAuthorityDataAccess : IDataAccessBase
    {
        bool DecreaseOperationSortNo(int opId, string mdfAccount);
        bool DeleteDepartmentData(int deptId);
        bool DeleteEmployeeRoleData(int roleId);
        bool DeleteOperationData(int opId);
        List<BackEndLogForBackend> GetBackEndLogList(BackEndLogListQueryParams param);
        DepartmentForBackend GetDepartmentDataForBackend(int deptId);
        List<DepartmentForBackend> GetDepartmentListForBackend(DeptListQueryParams param);
        int GetDepartmentMaxSortNo();
        string GetEmployeeAccountOfId(int empId);
        EmployeeForBackend GetEmployeeDataForBackend(int empId);
        EmployeeForBackend GetEmployeeDataForBackend(string empAccount);
        EmployeeToLogin GetEmployeeDataToLogin(string empAccount);
        List<EmployeeForBackend> GetEmployeeListForBackend(AccountListQueryParams param);
        EmployeeRoleForBackend GetEmployeeRoleDataForBackend(int roleId);
        List<EmployeeRoleForBackend> GetEmployeeRoleListForBackend(RoleListQueryParams param);
        int GetEmployeeRoleMaxSortNo();
        EmployeeRoleOperationsDesc GetEmployeeRoleOperationsDescDataOfOp(string roleName, int opId);
        OperationForBackend GetOperationDataForBackend(int opId);
        List<OperationLevelInfo> GetOperationLevelInfo(int opId);
        List<OperationForBackend> GetOperationListForBackend(OpListQueryParams param);
        int GetOperationMaxSortNo(int parentId);
        OperationOpInfo GetOperationOpInfoByCommonClass(string commonClass);
        OperationOpInfo GetOperationOpInfoByLinkUrl(string linkUrl);
        OperationOpInfo GetOperationOpInfoByLinkUrl(string linkUrl, bool isNewWindow);
        string GetRoleNameOfEmp(string empAccount);
        bool IncreaseOperationSortNo(int opId, string mdfAccount);
        InsertResult InsertDepartmentData(Department entity);
        InsertResult InsertEmployeeRoleData(EmployeeRole entity, string copyPrivilegeFromRoleName);
        bool SaveListOfEmployeeRolePrivileges(List<EmployeeRoleOperationsDesc> empRoleOps);
        bool UpdateDepartmentData(Department entity);
    }
}