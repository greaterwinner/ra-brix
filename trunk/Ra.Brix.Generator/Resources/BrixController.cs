using System;
using System.Collections.Generic;
using System.Text;
using Ra.Brix.Loader;

namespace {0}Controller
{{
    [ActiveController]
    public class {0}Controller
    {{
        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {{
            e.Params["Button{0}"].Value = "Menu-Fire{0}";
        }}

        [ActiveEvent("Menu-Fire{0}")]
        private void FireWebObserver(object sender, ActiveEventArgs e)
        {{
            ActiveEvents.Instance.RaiseLoadControl(
                "{1}",
                "dynMid");
        }}
    }}
}}
