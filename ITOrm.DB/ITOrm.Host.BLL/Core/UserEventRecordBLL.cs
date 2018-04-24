//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-04-23 19:54:33 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using ITOrm.Host.Models;
using ITOrm.Core.Dapper.Context;
using ITOrm.Host.DAL;
using ITOrm.Core.Utility.Castle;
namespace ITOrm.Host.BLL
{
    

    /// <summary>
    /// 备注
    /// </summary>
    public partial  class UserEventRecordBLL
    {
		private readonly IUserEventRecordDAL dal = ContainerHelper.Get<IUserEventRecordDAL>();
	    #region ==========增删改数据

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public  int Insert(UserEventRecord entity)
        {
			return dal.Insert(entity);
        }

        /// <summary>
        /// Update的对象，必须通过Single()获取重置属性后操作！传入实体修改，根据传入的实体主健修改，如果是new出来的实体，则要把Single()的对象赋给他才可更新
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public  bool Update(UserEventRecord entity)
        {
			return dal.Update(entity);
        }

        /// <summary>
        /// Delete，根据实体对象删除
        /// </summary>
        /// <returns>返回,成功:true,失败:false</returns>
        public  bool Delete(UserEventRecord entity)
        {
            return dal.Update(entity);
        }

        /// <summary>
        /// 根据主键ID删除数据
        /// </summary>
        /// <param name="id">主键ID的值</param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        #endregion
		#region ==========查询单一实体


        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>UserEventRecord实体对象</returns>
        public UserEventRecord Single(int id)
        {
		return dal.Single(id);

		}


        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>UserEventRecord实体对象</returns>
        public  UserEventRecord Single(string where, object param = null)
        {
			return dal.Single(where,param);
        }
        #endregion

        #region ==========查询列表集合

        /// <summary>
        /// 查询List集合
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public List<UserEventRecord> GetQuery(string where, object param = null, string orderBy = null)
        {
			return dal.GetQuery(where,param,orderBy);
        }


        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public List<UserEventRecord> GetQuery(int top, string where, object param = null, string orderBy = null)
        {
			return dal.GetQuery(top,where,param,orderBy);
        }


        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public  int Count(string where, object param = null)
        {
            return dal.Count(where,param);
        }



		/// <summary>
        /// 根据sql条件查询单一字段内容
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public object Scalar(string sql, object param = null)
        {
            return dal.Scalar(sql,param);
        }

        

		

		/// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="where">查询条件</param>
        /// <param name="param">条件语句参数化</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public List<UserEventRecord> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null)
        {
            return dal.GetPaged(pageSize,pageIndex,out totalCount,where,param,orderBy);
        }








        #endregion

    }
}

