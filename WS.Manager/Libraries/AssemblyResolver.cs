using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace WS.Manager.Libraries
{
    /// <summary>
    /// Resolver for missing assemblies.
    /// </summary>
    public class AssemblyResolver
    {
        private readonly IDictionary<string, byte[]> _assemblies = new Dictionary<string, byte[]>()
        {
            { "System.Windows.Interactivity, PublicKeyToken=31bf3856ad364e35", AssemblyResources.System_Windows_Interactivity },
            { "System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", AssemblyResources.System_Windows_Interactivity }
        };

        /// <summary>
        /// Activate handler.
        /// </summary>
        public void Activate()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;
            if (_assemblies.ContainsKey(assemblyName))
            {
                return Assembly.Load(_assemblies[assemblyName]);
            }

            return null;
        }
    }
}