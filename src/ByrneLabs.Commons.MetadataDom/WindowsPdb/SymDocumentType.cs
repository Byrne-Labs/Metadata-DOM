﻿// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
/*============================================================
** 
** Class:  SymDocumentType 
**
** 
[System.Runtime.InteropServices.ComVisible(true)]
** A class to hold public guids for document types to be used with the
** symbol store.
** 
**
===========================================================*/

using System;
using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    // Only statics does not need to be marked with the serializable attribute
    
    [ComVisible(true)]
    public class SymDocumentType
    {
        public static readonly Guid Text = new Guid(0x5a869d0b, 0x6611, 0x11d3, 0xbd, 0x2a, 0x0, 0x0, 0xf8, 0x8, 0x49, 0xbd);
    }
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
