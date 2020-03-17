using Autofac;
using MFramework.Web.Common.Helpers;
using System;

namespace MFramework.Web.Common
{
    public partial class Startup
    {
        private readonly Action<ContainerBuilder> Options;
        public ContainerBuilder Builder;

        public Startup(Action<ContainerBuilder> options = null)
        {
            Options = options;
            Builder = new ContainerBuilder();
        }

        public virtual Startup ConfigureServices()
        {
            Builder.RegisterType<RestHelperBase>().As<IRestHelperBase>().SingleInstance();
            Builder.RegisterType<CacheHelperBase>().As<ICacheHelperBase>().SingleInstance();
            Builder.RegisterType<ConfigHelperBase>().As<IConfigHelperBase>().SingleInstance();
            Builder.RegisterType<TagHelperBase>().As<ITagHelperBase>().InstancePerLifetimeScope();

            Options?.Invoke(Builder);

            return this;
        }

        public virtual Startup BuildIoCContainer()
        {
            AppScope.Container = Builder.Build();
            return this;
        }
    }
}