using ITOrm.Ms.Models.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ITOrm.Ms.Models.Helper
{
    /// <summary>
    /// 数据库:执行表单业务逻辑，和自定义的SQL语句
    /// </summary>
    public static class CustomHelper
    {
        #region ========== 表单业务逻辑Helper
        /// <summary>
        /// 拆分字符串获取对应的表单值 根据对应的表单名称，返回对应的值
        /// </summary>
        /// <param name="rval"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string GetFromValue(this string rval, string k)
        {
            string result = "";
            rval = Microsoft.JScript.GlobalObject.unescape(rval);//C#版的 Escape() 和 Unescape() 函数  转译js的escape函数
            string[] rArray = rval.Split(new string[] { "^&^" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in rArray)
            {
                string[] im = item.Split(new string[] { "^*^" }, StringSplitOptions.RemoveEmptyEntries);
                if (im.Length > 0)
                {
                    if (im[0] == k)
                    {
                        foreach (string asd in im)
                        {
                            if (k != asd)
                                result = asd;
                        }
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将要进行非空验证的实体字段字符串集合进行判断，看是否有存在空值的字段，只要存在即返回true,全部都不是空值返回false
        /// </summary>
        /// <param name="values">进行非空验证的实体字段字符串集合(,拼接)</param>
        /// <returns></returns>
        public static bool ValidationEmpty(this string rval, string values)
        {
            bool result = false;
            try
            {
                string[] strs = values.Split(','); ;//定义一数组
                for (int i = 0; i < strs.Length; i++)
                {
                    if (!strs[i].IsNULL())//字段抒写错误，存在异常则返回true
                    {
                        if (rval.GetFromValue(strs[i]).IsNULL())//存在空值，返回true
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch//存在异常返回true
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 判断是否存在Key值,存在返回true,不存在返回false,区分大小写
        /// </summary>
        /// <param name="rval">进行非空验证的实体字段字符串集合(,拼接)</param>
        /// <param name="key">Key</param>
        /// <returns>存在返回true,不存在返回false;</returns>
        public static bool ValidationKey(this string rval, string key)
        {
            bool result = false;
            rval = Microsoft.JScript.GlobalObject.unescape(rval);//C#版的 Escape() 和 Unescape() 函数  转译js的escape函数
            string[] rArray = rval.Split(new string[] { "^&^" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in rArray)
            {
                string[] im = item.Split(new string[] { "^*^" }, StringSplitOptions.RemoveEmptyEntries);
                if (im.Length > 0)
                {
                    if (im[0] == key)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// [OLD Code]返回一个新创建的实体对象，并给各个字段赋值
        /// </summary>
        /// <param name="typeName">完整实体名称， [命名空间.实体名称]</param>
        /// <param name="rval">表单集合字符串</param>
        /// <returns></returns>
        public static object GetFormEntity(this string rval, string typeName)
        {
            typeName = string.Format("{0}.{1}", ConfigHelper.GetAppSettings("EntitySpaceName"), typeName);
            TypeX EntityType = TypeX.Create(TypeX.GetType(typeName, true));//根据类的完整名称建立类的类型,用于动态创建类 如： Clump.Mobile.Models.NewsInfo
            Object objEntity = EntityType.CreateInstance();//动态建立类的对象 实体类的对象Object objEntity = EntityType.CreateInstance(true);意思是在创建实体对象时 A a = new A(true)

            PropertyInfo[] props = objEntity.GetType().GetProperties();//获取此对象的，字段集合
            object propertValue = String.Empty;
            foreach (PropertyInfo item in props)
            {
                propertValue = rval.GetFromValue(item.Name);
                if (!"".Equals(propertValue))
                {
                    //根据字段类型，转换格式
                    switch ((PropertyInfoX.Create(EntityType, item.Name)).Property.PropertyType.Name)
                    {
                        case "String": propertValue = Convert.ToString(propertValue); break;
                        case "Int64": propertValue = Convert.ToInt64(propertValue); break;
                        case "Int32": propertValue = Convert.ToInt32(propertValue); break;
                        case "Int16": propertValue = Convert.ToInt16(propertValue); break;
                        case "Byte": propertValue = Convert.ToByte(propertValue); break;
                        case "Single": propertValue = Convert.ToSingle(propertValue); break;
                        case "Double": propertValue = Convert.ToDouble(propertValue); break;
                        case "Boolean": propertValue = Convert.ToBoolean(propertValue); break;
                        case "DateTime": propertValue = Convert.ToDateTime(propertValue); break;
                        default: break;
                    }
                    PropertyInfoX.SetValue(objEntity, item.Name, propertValue);//给字段赋值
                }
            }
            PropertyInfoX.SetValue(objEntity, "CreateTime", DateTime.Now);//给CreateTime字段赋值,添加时间字段是每个数据表标配(未解决：MySql配置，暂时不能默认添加当前时间)
            return objEntity;
        }

        /// <summary>
        ///  将对象赋值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="rval">表单集合字符串</param>
        /// <param name="model">从数据库查询的实体对象</param>
        /// <returns></returns>
        public static T ConvertToModel<T>(this string rval, T model)
        {
            Type modelType = model.GetType();
            PropertyInfo[] props = modelType.GetProperties();//获取此对象的，字段集合
            object propertValue = null;
            string value = String.Empty;
            foreach (PropertyInfo item in props)
            {
                if (rval.ValidationKey(item.Name))
                {
                    value = rval.GetFromValue(item.Name);
                    propertValue = GetVal(propertValue, value, item.PropertyType.Name);
                    if (item.PropertyType.Name == "Double")//如果对象类型是Double数字，等于0则不予赋值
                    {
                        if (propertValue.ToString() != "0")
                        {
                            PropertyInfoX.SetValue(model, item.Name, propertValue);//给字段赋值
                        }
                    }
                    else
                    {
                        PropertyInfoX.SetValue(model, item.Name, propertValue);//给字段赋值
                    }
                }
            }
            return model;
        }


        /// <summary>
        ///  将对象赋值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="formModel">表单获取的实体对象</param>
        /// <param name="model">从数据库查询的实体对象</param>
        /// <returns></returns>
        public static T ConvertToModel<T>(T formModel, T model)
        {
            Type formModelType = formModel.GetType();
            Type modelType = model.GetType();
            PropertyInfo[] props = modelType.GetProperties();//获取此对象的，字段集合
            PropertyInfo[] formProps = formModelType.GetProperties();//获取此对象的，字段集合
            object propertValue = null;
            string value = String.Empty;
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].Name.Equals(formProps[i].Name) && props[i].CanWrite == true)
                {
                    propertValue = PropertyInfoX.GetValue(formModel, formProps[i].Name);
                    if (propertValue != null)
                    {
                        PropertyInfoX.SetValue(model, props[i].Name, propertValue);//给字段赋值
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 将value转换成tname类型，并返object值
        /// </summary>
        /// <param name="propertValue"></param>
        /// <param name="value"></param>
        /// <param name="tname"></param>
        /// <returns></returns>
        public static object GetVal(object propertValue, string value, string tname)
        {
            switch (tname)
            {
                case "String":
                    if (!value.IsNULL())
                        propertValue = Convert.ToString(value);
                    else
                        propertValue = null;
                    break;
                case "Int64":
                    if (!value.IsNULL())
                        propertValue = Convert.ToInt64(value);
                    else
                        propertValue = 0;
                    break;
                case "Int32":
                    if (!value.IsNULL())
                        propertValue = Convert.ToInt32(value);
                    else
                        propertValue = 0;
                    break;
                case "Int16":
                    if (!value.IsNULL())
                        propertValue = Convert.ToInt16(value);
                    else
                        propertValue = 0;
                    break;
                case "Byte":
                    if (!value.IsNULL())
                        propertValue = Convert.ToByte(value);
                    else
                        propertValue = 0;
                    break;
                case "Single":
                    if (!value.IsNULL())
                        propertValue = Convert.ToSingle(value);
                    else
                        propertValue = 0;
                    break;
                case "Double":
                    if (!value.IsNULL())
                        propertValue = Convert.ToDouble(value);
                    else
                        propertValue = 0;
                    break;
                case "Boolean":
                    if (!value.IsNULL())
                        propertValue = Convert.ToBoolean(value);
                    else
                        propertValue = false;
                    break;
                case "DateTime":
                    if (!value.IsNULL())
                        propertValue = Convert.ToDateTime(value);
                    else
                        propertValue = DateTime.Now;
                    break;
                default: break;
            }
            return propertValue;
        }

        #endregion

        /// <summary>
        /// 根据实体名称，获取此实体字段名称的数组List<string>
        /// </summary>
        /// <param name="typeName">完整实体名称， [命名空间.实体名称]</param>
        /// <returns></returns>
        public static List<string> GetEntityFieldList(this string tableName)
        {
            List<string> list = new List<string>();
            tableName = string.Format("{0}.{1}", ConfigHelper.GetAppSettings("EntitySpaceName"), tableName);
            TypeX EntityType = TypeX.Create(TypeX.GetType(tableName, true));//根据类的完整名称建立类的类型,用于动态创建类 如： Clump.Mobile.Models.NewsInfo
            Object objEntity = EntityType.CreateInstance();//动态建立类的对象 实体类的对象Object objEntity = EntityType.CreateInstance(true);意思是在创建实体对象时 A a = new A(true)
            PropertyInfo[] props = objEntity.GetType().GetProperties();//获取此对象的，字段集合
            object propertValue = String.Empty;
            foreach (PropertyInfo item in props)
            {
                list.Add(item.Name);
            }
            return list;
        }

    }
}
