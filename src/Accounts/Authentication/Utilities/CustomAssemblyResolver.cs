﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.Commands.Profile.Utilities
{
    public static class CustomAssemblyResolver
    {
        private static IDictionary<string, Version> NetFxPreloadAssemblies =
            new Dictionary<string, Version>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Azure.Core", new Version("1.4.1.0")},
                {"Microsoft.Bcl.AsyncInterfaces", new Version("1.0.0.0")},
                {"Microsoft.IdentityModel.Clients.ActiveDirectory", new Version("3.19.2.6005")},
                {"Microsoft.IdentityModel.Clients.ActiveDirectory.Platform", new Version("3.19.2.6005")},
                {"Newtonsoft.Json", new Version("10.0.0.0")},
                {"System.Buffers", new Version("4.0.2.0")},
                {"System.Diagnostics.DiagnosticSource", new Version("4.0.4.0")},
                {"System.Memory", new Version("4.0.1.1")},
                {"System.Net.Http.WinHttpHandler", new Version("4.0.2.0")},
                {"System.Numerics.Vectors", new Version("4.1.3.0")},
                {"System.Private.ServiceModel", new Version("4.1.2.1")},
                {"System.Reflection.DispatchProxy", new Version("4.0.3.0")},
                {"System.Runtime.CompilerServices.Unsafe", new Version("4.0.5.0")},
                {"System.Security.AccessControl", new Version("4.1.1.0")},
                {"System.Security.Permissions", new Version("4.0.1.0")},
                {"System.Security.Principal.Windows", new Version("4.1.1.0")},
                {"System.ServiceModel.Primitives", new Version("4.2.0.0")},
                {"System.Text.Encodings.Web", new Version("4.0.4.0")},
                {"System.Text.Json", new Version("4.0.0.0")},
                {"System.Threading.Tasks.Extensions", new Version("4.2.0.0")},
                {"System.Xml.ReaderWriter", new Version("4.1.0.0")}
            };

        private static string PreloadAssemblyFolder { get; set; }

        public static void Initialize()
        {
            //This function is call before loading assemblies in PreloadAssemblies folder, so NewtonSoft.Json could not be used here
            var accountFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PreloadAssemblyFolder = Path.Combine(accountFolder, "PreloadAssemblies");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        /// <summary>
        /// When the resolution of an assembly fails, if will try to redirect to the higher version
        /// </summary>
        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                AssemblyName name = new AssemblyName(args.Name);
                if (NetFxPreloadAssemblies.TryGetValue(name.Name, out Version version))
                {
                    if (version >= name.Version && version.Major == name.Version.Major)
                    {
                        string requiredAssembly = Path.Combine(PreloadAssemblyFolder, $"{name.Name}.dll");
                        return Assembly.LoadFrom(requiredAssembly);
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
