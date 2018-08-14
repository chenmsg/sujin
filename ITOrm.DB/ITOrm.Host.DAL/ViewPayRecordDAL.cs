//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的...
//    生成时间 2018-08-14 10:52:56 By CClump
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using ITOrm.Core.Utility.PagerWebdiyer;
using ITOrm.Host.Models;
using ITOrm.Core.Dapper.Context;
namespace ITOrm.Host.DAL
{
    /// <summary>
    /// 备注接口
    /// </summary>
    public interface IViewPayRecordDAL
    {
		
        #region ==========查询单一实体

        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>ViewPayRecord实体对象</returns>
        ViewPayRecord Single(int id);

        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>ViewPayRecord实体对象</returns>
        ViewPayRecord Single(string where, object param = null);

        #endregion

        #region ==========查询列表集合

        /// <summary>
        /// 查询List集合
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        List<ViewPayRecord> GetQuery(string where, object param = null, string orderBy = null);



        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        List<ViewPayRecord> GetQuery(int top, string where, object param = null, string orderBy = null);


        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        int Count(string where, object param = null);


		/// <summary>
        /// 根据sql条件查询单一字段内容
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        object Scalar(string sql, object param = null);

        
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
        List<ViewPayRecord> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null);

        


        #endregion


    }

    /// <summary>
    /// 备注
    /// </summary>
    public class ViewPayRecordDAL : BaseViewAction<ViewPayRecord>, IViewPayRecordDAL
    {
	    #region ==========查询单一实体


        /// <summary>
        /// 根据主键ID取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>ViewPayRecord实体对象</returns>
        public ViewPayRecord Single(int id)
        {
					return base.Single(id, "ID");
		
		}


        /// <summary>
        /// 根据where条件取得单一实体,如果没有找到则返回null
        /// </summary>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <returns>ViewPayRecord实体对象</returns>
        public new  ViewPayRecord Single(string where, object param = null)
        {
			
			return base.Single(where, param);

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
        public List<ViewPayRecord> GetQuery(string where, object param = null, string orderBy = null)
        {
						return base.GetQuery(string.Format("SELECT ID,UserId,RequestId,Code,Amount,WithDrawAmount,ActualAmount,Fee,Fee3,Rate,CTime,RealName,Ip,HandleTime,Platform,Mobile,BankCard,BankCode,PayerPhone,ChannelType,BankName,State,Income,DrawIncome FROM View_PayRecord WITH(NOLOCK) WHERE {0} {1}", where, orderBy), param);
			        }


        /// <summary>
        /// 查询List集合的前几条
        /// </summary>
        /// <param name="top">前多少条</param>
        /// <param name="where">where语句</param>
        /// <param name="param">参数化对象</param>
        /// <param name="orderBy">排序语句[例子：order by id desc ]</param>
        /// <returns>List集合</returns>
        public List<ViewPayRecord> GetQuery(int top, string where, object param = null, string orderBy = null)
        {
						return base.GetQuery(string.Format("SELECT top {2} ID,UserId,RequestId,Code,Amount,WithDrawAmount,ActualAmount,Fee,Fee3,Rate,CTime,RealName,Ip,HandleTime,Platform,Mobile,BankCard,BankCode,PayerPhone,ChannelType,BankName,State,Income,DrawIncome FROM View_PayRecord WITH(NOLOCK) WHERE {0} {1} ", where, orderBy, top), param);
			        }


        /// <summary>
        /// 根据条件获取条数
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public new int Count(string where, object param = null)
        {
            return base.Count("SELECT COUNT(0) FROM View_PayRecord WITH(NOLOCK) WHERE " + where, param);
        }



		/// <summary>
        /// 根据sql条件查询单一字段内容
        /// </summary>
        /// <param name="where">sql条件</param>
        /// <param name="param">参数化</param>
        /// <returns></returns>
        public object Scalar(string sql, object param = null)
        {
            return base.GetScalar(sql, param);
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
        public List<ViewPayRecord> GetPaged(int pageSize, int pageIndex, out int totalCount, string where, object param = null, string orderBy = null)
        {
            string orderByNow = " ORDER BY ID DESC ";
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderByNow = orderBy;
            }
            totalCount = 0;
            int topNum = pageSize * (pageIndex - 1) + 1;
			string field="ID,UserId,RequestId,Code,Amount,WithDrawAmount,ActualAmount,Fee,Fee3,Rate,CTime,RealName,Ip,HandleTime,Platform,Mobile,BankCard,BankCode,PayerPhone,ChannelType,BankName,State,Income,DrawIncome";
            string sql = string.Format("SELECT {0} FROM View_PayRecord ", field);
            if (topNum <= 1)
            {
				sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER({1}) as 'RowNumber', {0} from View_PayRecord WITH(NOLOCK) {3} ) as temp WHERE (RowNumber between 0 and {2} ) ", field, orderByNow, pageSize, (!string.IsNullOrEmpty(where) ? " WHERE " + where : ""));
            }
            else
            {
                sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER({1}) as 'RowNumber', {0} from View_PayRecord WITH(NOLOCK) {4} ) as temp WHERE (RowNumber between {2} and {3} ) ", field, orderByNow, topNum, (topNum + pageSize-1), (!string.IsNullOrEmpty(where) ? " WHERE " + where : ""));
            }
            try
            {
                totalCount = Count(where, param);
								return base.GetQuery(sql, param);
				                
            }
            catch (Exception e)
            {
                log.Error("获取分页数据失败", e);
                return null;
            }
        }








        #endregion

		    }
}

