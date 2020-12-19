using System.Runtime.CompilerServices;

#if NET
using System.Text;
#endif

[assembly: InternalsVisibleTo("Cuture.Http.Test")]
#if NET

internal class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}

#endif