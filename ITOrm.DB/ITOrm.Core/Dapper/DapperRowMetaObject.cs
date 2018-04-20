﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace ITOrm.Core.Dapper
{
    sealed partial class DapperRowMetaObject : DynamicMetaObject
    {
        static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
        static readonly MethodInfo setValueMethod = typeof(DapperRow).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });

        public DapperRowMetaObject(Expression expression, BindingRestrictions restrictions)
            : base(expression, restrictions)
        {
        }

        public DapperRowMetaObject(
            Expression expression,
            BindingRestrictions restrictions,
            object value
            )
            : base(expression, restrictions, value)
        {
        }

        DynamicMetaObject CallMethod(
            MethodInfo method,
            Expression[] parameters
            )
        {
            var callMethod = new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    method,
                    parameters),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType)
                );
            return callMethod;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[]
                                     {
                                         Expression.Constant(binder.Name)
                                     };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        // Needed for Visual basic dynamic support
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            var parameters = new Expression[]
                                     {
                                         Expression.Constant(binder.Name)
                                     };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var parameters = new Expression[]
                                     {
                                         Expression.Constant(binder.Name),
                                         value.Expression,
                                     };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }
    }
}
