using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ITOrm.Core.Helper
{
    public class MssqlProvider : IMssqlProvider
    {
        public DbProviderFactory Instance()
        {
            return SqlClientFactory.Instance;
        }

        public void DeriveParameters(IDbCommand cmd)
        {
            if ((cmd as SqlCommand) != null)
            {
                SqlCommandBuilder.DeriveParameters(cmd as SqlCommand);
            }
        }

        public DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size)
        {
            SqlParameter param;

            if (Size > 0)
                param = new SqlParameter(ParamName, (SqlDbType)DbType, Size);
            else
                param = new SqlParameter(ParamName, (SqlDbType)DbType);

            return param;
        }

        public bool IsFullTextSearchEnabled()
        {
            return true;
        }

        public bool IsCompactDatabase()
        {
            return true;
        }

        public bool IsBackupDatabase()
        {
            return true;
        }

        public string GetLastIdSql()
        {
            return "SELECT SCOPE_IDENTITY()";
        }

        public bool IsDbOptimize()
        {
            return false;
        }

        public bool IsShrinkData()
        {
            return true;
        }

        public bool IsStoreProc()
        {
            return true;
        }
    }

    public interface IMssqlProvider
    {
        /// <summary>
        /// ����DbProviderFactoryʵ��
        /// </summary>
        /// <returns></returns>
        DbProviderFactory Instance();
        /// <summary>
        /// ����SQL������Ϣ�����
        /// </summary>
        /// <param name="cmd"></param>
        void DeriveParameters(IDbCommand cmd);

        /// <summary>
        /// ����SQL����
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size);
        /// <summary>
        /// �Ƿ�֧��ȫ������
        /// </summary>
        /// <returns></returns>
        bool IsFullTextSearchEnabled();

        /// <summary>
        /// �Ƿ�֧��ѹ�����ݿ�
        /// </summary>
        /// <returns></returns>
        bool IsCompactDatabase();

        /// <summary>
        /// �Ƿ�֧�ֱ������ݿ�
        /// </summary>
        /// <returns></returns>
        bool IsBackupDatabase();

        /// <summary>
        /// ���ظղ����¼������IDֵ, �粻֧����Ϊ""
        /// </summary>
        /// <returns></returns>
        string GetLastIdSql();
        /// <summary>
        /// �Ƿ�֧�����ݿ��Ż�
        /// </summary>
        /// <returns></returns>
        bool IsDbOptimize();
        /// <summary>
        /// �Ƿ�֧�����ݿ�����
        /// </summary>
        /// <returns></returns>
        bool IsShrinkData();
        /// <summary>
        /// �Ƿ�֧�ִ洢����
        /// </summary>
        /// <returns></returns>
        bool IsStoreProc();
    }
}
