// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.SymbolStore.SymAddressKind
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 9B2B058A-65AD-49D7-A55F-02BD3A7D0D68
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    /// <summary>Specifies address types for local variables, parameters, and fields in the methods <see cref="M:System.Diagnostics.SymbolStore.ISymbolWriter.DefineLocalVariable(System.String,System.Reflection.FieldAttributes,System.Byte[],System.Diagnostics.SymbolStore.SymAddressKind,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)" />, <see cref="M:System.Diagnostics.SymbolStore.ISymbolWriter.DefineParameter(System.String,System.Reflection.ParameterAttributes,System.Int32,System.Diagnostics.SymbolStore.SymAddressKind,System.Int32,System.Int32,System.Int32)" />, and <see cref="M:System.Diagnostics.SymbolStore.ISymbolWriter.DefineField(System.Diagnostics.SymbolStore.SymbolToken,System.String,System.Reflection.FieldAttributes,System.Byte[],System.Diagnostics.SymbolStore.SymAddressKind,System.Int32,System.Int32,System.Int32)" /> of the <see cref="T:System.Diagnostics.SymbolStore.ISymbolWriter" /> interface.</summary>
    public enum SymAddressKind
    {
        ILOffset = 1,
        NativeRVA = 2,
        NativeRegister = 3,
        NativeRegisterRelative = 4,
        NativeOffset = 5,
        NativeRegisterRegister = 6,
        NativeRegisterStack = 7,
        NativeStackRegister = 8,
        BitField = 9,
        NativeSectionOffset = 10,
    }
}
