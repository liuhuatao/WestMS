using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MSFramework.Common;
using MSFramework.Reflection;

namespace MSFramework.DependencyInjection
{
	/// <summary>
	/// <see cref="ServiceLifetime.Scoped"/>生命周期类型的服务映射查找器
	/// </summary>
	public class DependencyTypeFinder : IDependencyTypeFinder
	{
		public Dictionary<ServiceLifetime, Type[]> GetDependencyTypeDict()
		{
			if (Singleton<IAssemblyFinder>.Instance == null)
			{
				Singleton<IAssemblyFinder>.Instance = new AssemblyFinder();
			}

			var assemblies = Singleton<IAssemblyFinder>.Instance.GetAllAssemblyList();
			var scopeTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(type => typeof(IScopeDependency).IsAssignableFrom(type) && !type.IsAbstract &&
				               !type.IsInterface).ToArray();
			var singletonTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(type => typeof(ISingletonDependency).IsAssignableFrom(type) && !type.IsAbstract &&
				               !type.IsInterface).ToArray();
			var transientTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(type => typeof(ITransientDependency).IsAssignableFrom(type) && !type.IsAbstract &&
				               !type.IsInterface).ToArray();
			var dict = new Dictionary<ServiceLifetime, Type[]>
			{
				{ServiceLifetime.Scoped, scopeTypes}, {ServiceLifetime.Singleton, singletonTypes},
				{ServiceLifetime.Transient, transientTypes}
			};
			return dict;
		}
	}
}