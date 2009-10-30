/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Collections.Generic;
using Ra.Widgets;
using Ra.Brix.Loader;
using Ra.Effects;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using Ra.Brix.Types;
using Ra.Selector;

namespace {0}
{{
    [ActiveModule]
    public partial class {1} : System.Web.UI.UserControl, IModule
    {{

        #region IModule Members

        public void InitialLoading(Node node)
        {{
            
        }}

        public string GetCaption()
        {{
            return string.Empty;
        }}

        #endregion
    }}
}}