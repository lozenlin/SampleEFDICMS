// ===============================================================================
// DataAccessBase of SampleEFDICMS
// https://github.com/lozenlin/SampleEFDICMS
//
// DataAccessBase.cs
//
// ===============================================================================
// Copyright (c) 2019 lozenlin
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// ===============================================================================

using Common.Data.Domain.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Common.DataAccess.EF
{
    public interface IDataAccessBase
    {
        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        bool Delete<TEntity>(TEntity entity) where TEntity : class;
        TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity Get<TEntity>(params object[] pkValues) where TEntity : class;
        int GetCount<TEntity>() where TEntity : class;
        int GetCount<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity GetEmptyEntity<TEntity>(object requiredPropValues) where TEntity : class;
        string GetErrMsg();
        IQueryable<TEntity> GetList<TEntity>() where TEntity : class;
        IQueryable<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        int GetSqlErrNumber();
        int GetSqlErrState();
        InsertResult Insert<TEntity>(TEntity entity) where TEntity : class;
        bool Update();
        bool UpdateAllCols<TEntity>(TEntity entity) where TEntity : class;
    }
}