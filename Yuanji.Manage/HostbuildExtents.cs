using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

using YS.Yuanji.Commom;
using YS.Yuanji.Drive;

namespace Yuanji.Manage
{
    public static class HostbuildExtents
    {

        //实现IhostService注册服务扩展方法
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            Configure(services);

            // 注册你的服务
            services.AddHostedService<HostSevice>();
            services.AddSingleton<ControllerManage>();
            services.AddSingleton<MqttController>();
            services.AddSingleton<StarupSevice>();
            services.AddSingleton<HttpClientService>();
			services.AddSingleton<Func<string,Type,object>>(serviceProvider => (className, ty) =>
           {
               var service = serviceProvider.CreateScope().ServiceProvider.GetServices(ty);
               var type = service.FirstOrDefault(t =>t != null && t.GetType() != null && t.GetType().Name.Contains(className, StringComparison.OrdinalIgnoreCase));
               return type;
           });

           return services;
        }


        private static void Configure(IServiceCollection services)
        {
            // 获取所有相关程序集
            var assemblies = GetRelevantAssemblies();
            // 查找当前加载程序类中符合的服务进行注册
            var serviceTypes = assemblies
                .Where(a => !a.IsDynamic && a.GetName().Name != null)
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(t => (typeof(IDevice).IsAssignableFrom(t)||typeof(IChanlel).IsAssignableFrom(t)) && t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                if (typeof(IDevice).IsAssignableFrom(serviceType))
                {
                    services.AddScoped(typeof(IDevice), serviceType);
                }

                if (typeof(IChanlel).IsAssignableFrom(serviceType))
                {
                    services.AddScoped(typeof(IChanlel), serviceType);
                }
            }

        }
        private static List<Assembly> GetRelevantAssemblies()
        {
            var assemblies = new List<Assembly>();

            try
            {
                // 添加当前程序集
                assemblies.Add(Assembly.GetExecutingAssembly());

                // 添加入口程序集
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                    assemblies.Add(entryAssembly);
             
                //加载当前运行目录下所有dll
                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                foreach (var file in files)
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        assemblies.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // 添加已加载的程序集
                assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && a.GetName().Name != null));
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取程序集时发生错误: {ex.Message}");
            }

            return assemblies.Distinct().ToList();
        }
    }
}
