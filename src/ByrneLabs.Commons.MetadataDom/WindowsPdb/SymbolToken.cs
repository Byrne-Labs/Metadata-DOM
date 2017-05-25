// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
/*============================================================
** 
** Class:  SymbolToken 
**
** Small value class used by the SymbolStore package for passing 
** around metadata tokens.
**
===========================================================*/

using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    [ComVisible(true)]
    public struct SymbolToken
    {
        internal int m_token;

        public SymbolToken(int val)
        {
            m_token = val;
        }

        public int GetToken() => m_token;

        public override int GetHashCode() => m_token;

        public override bool Equals(object obj)
        {
            if (obj is SymbolToken)
            {
                return Equals((SymbolToken) obj);
            }

            return false;
        }

        public bool Equals(SymbolToken obj) => obj.m_token == m_token;

        public static bool operator ==(SymbolToken a, SymbolToken b) => a.Equals(b);

        public static bool operator !=(SymbolToken a, SymbolToken b) => !(a == b);
    }
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
