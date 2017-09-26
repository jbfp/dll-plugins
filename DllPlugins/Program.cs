using ClassLibrary1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DllPlugins
{
    public sealed class Program
    {
        private static readonly HashSet<Assembly> assemblies =
            new HashSet<Assembly>();

        private static readonly ConcurrentDictionary<string, Plugin> plugins =
            new ConcurrentDictionary<string, Plugin>();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadPluginsFrom(assembly);
            }

            ImportDll("/app/bin/Debug/netcoreapp2.0/MyPlugin.dll");

            Thread.Sleep(5000);

            foreach (var (key, plugin) in plugins)
            {
                Console.WriteLine($"Shutting down {key}");

                try
                {
                    plugin.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void ImportDll(string path)
        {
            Assembly.LoadFrom(path);
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs e)
        {
            LoadPluginsFrom(e.LoadedAssembly);
        }

        private static async void LoadPluginsFrom(Assembly assembly)
        {
            var basePluginType = typeof(Plugin);

            var pluginTypes = assembly
                .GetExportedTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => basePluginType.IsAssignableFrom(t))
                .ToList();

            if (pluginTypes.Count == 0)
            {
                return;
            }

            lock (assemblies)
            {
                if (!assemblies.Add(assembly))
                {
                    return;
                }
            }

            Console.WriteLine($"Loaded assembly {assembly.FullName}, found {pluginTypes.Count} plugin implementations.");

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var pluginAttribute = pluginType.GetCustomAttribute<PluginAttribute>();
                    var pluginName = $"{pluginAttribute.Name ?? pluginType.Name}, {assembly.GetName()}";
                    var pluginInstance = (Plugin)Activator.CreateInstance(pluginType);

                    if (!plugins.TryAdd(pluginName, pluginInstance))
                    {
                        Console.WriteLine($"Found duplicate plugin by the key: {pluginName}");
                        continue;
                    }

                    Console.WriteLine($"Starting plugin {pluginAttribute.Name ?? pluginType.Name}.");

                    try
                    {
                        await pluginInstance.OnStartAsync(CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        plugins.Remove(pluginName, out _);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
