﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;
using YangKai.BlogEngine.Domain;
using YangKai.BlogEngine.Service;

namespace YangKai.BlogEngine.Web.Mvc.BootStrapper
{
    internal class RssBuildBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            IMethodReturn msg = getNext()(input, getNext);

            if (input.MethodBase.Name == "Create")
            {
                var parameters = input.MethodBase.GetParameters();
                if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType == typeof(Post))
                    {
                        Task.Factory.StartNew(Rss.Current.BuildPost);
                    }
                    if (parameters[0].ParameterType == typeof(Comment))
                    {
                        Task.Factory.StartNew(Rss.Current.BuildComment);
                    }
                }
            }

            return msg;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}