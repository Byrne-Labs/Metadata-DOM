using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IContainsSourceCode
    {
        string SourceCode { get; }
        string SourceFile { get; }
    }
}
