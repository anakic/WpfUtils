using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Thingie.WPF
{
    internal static class CustomInitializeComponent
    {
        public static void AssemblySensitive_InitializeComponent(this ContentControl contentControl, string componentString)
        {
            // Strictly speaking this check from the generated code should also be
            // implemented, which we could do by using a dependency property.
            //if (_contentLoaded)
            //{
            //    return;
            //}
            //_contentLoaded = true;

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var shortName = asm.GetName().Name;
            var publicKeyToken = GetPublicKeyTokenFromAssembly(asm);
            var version = asm.GetName().Version.ToString();
            System.Uri resourceLocater = new System.Uri($"/{shortName};V{version};{publicKeyToken};{componentString}", System.UriKind.Relative);

            System.Windows.Application.LoadComponent(contentControl, resourceLocater);
        }

        /// <summary>
        /// Gets a public key token from a provided assembly, and returns it as a string.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <remarks>Adapted from https://stackoverflow.com/questions/3045033/getting-the-publickeytoken-of-net-assemblies</remarks>
        private static string GetPublicKeyTokenFromAssembly(System.Reflection.Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "None";

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += string.Format("{0:x2}", bytes[i]);

            return publicKeyToken;
        }
    }
}
