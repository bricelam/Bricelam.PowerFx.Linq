using System.Diagnostics;
using System.Reflection;

#pragma warning disable IDE0130

namespace Microsoft.PowerFx;

static class PowerFxConfigExtensions
{
    static readonly Type _builtinFunctionsCoreType = Type.GetType(
        "Microsoft.PowerFx.Core.Texl.BuiltinFunctionsCore, Microsoft.PowerFx.Core",
        throwOnError: true)!;
    static readonly FieldInfo? _isUTCTodayField = _builtinFunctionsCoreType.GetField("IsUTCToday");
    static readonly FieldInfo? _utcNowField = _builtinFunctionsCoreType.GetField("UTCNow");
    static readonly FieldInfo? _utcTodayField = _builtinFunctionsCoreType.GetField("UTCToday");
    static readonly Type _texlFunctionType = Type.GetType(
        "Microsoft.PowerFx.Core.Functions.TexlFunction, Microsoft.PowerFx.Core",
        throwOnError: true)!;
    static readonly MethodInfo? _addFunctionMethod = typeof(PowerFxConfig).GetMethod(
        "AddFunction",
        BindingFlags.Instance | BindingFlags.NonPublic,
        [_texlFunctionType]);

    public static void EnableUTCFunctions(this PowerFxConfig config)
    {
        var isUTCToday = _isUTCTodayField?.GetValue(null);
        Debug.Assert(isUTCToday is not null);

        var utcNow = _utcNowField?.GetValue(null);
        Debug.Assert(utcNow is not null);

        var utcToday = _utcTodayField?.GetValue(null);
        Debug.Assert(utcToday is not null);

        Debug.Assert(_addFunctionMethod is not null);
        _addFunctionMethod.Invoke(config, [isUTCToday]);
        _addFunctionMethod.Invoke(config, [utcNow]);
        _addFunctionMethod.Invoke(config, [utcToday]);
    }
}
