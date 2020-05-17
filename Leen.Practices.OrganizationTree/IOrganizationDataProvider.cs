﻿using Leen.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 提供获取组织机构业务实体的数据接口。
    /// </summary>
    public interface IOrganizationDataProvider
    {
        /// <summary>
        /// 获取所有组织结构数据实体。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetOrganizations();

        /// <summary>
        /// 获取所有根组织结构数据实体。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetRootOrganizations();

        /// <summary>
        /// 获取指定Id组织结构数据实体。
        /// </summary>
        /// <param name="id">组织机构Id。</param>
        /// <returns></returns>
        Task<INamedDataEntity> GetOrganization(string id);

        /// <summary>
        /// 获取指定组织结构Id下的直属组织结构集合。
        /// </summary>
        /// <param name="parentId">父组织机构Id。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetOrganizations(string parentId);

        /// <summary>
        /// 递归获取指定组织结构Id下的所有组织结构集合。
        /// </summary>
        /// <param name="parentId">父组织机构Id。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetOrganizationsRecursive(string parentId);
    }
}
