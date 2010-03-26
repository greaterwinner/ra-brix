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
using System.Web.UI;
using System.IO;
using Ra.Brix.Data;
using System.Text;
using System.Web;
using Ra;

public class RaBrixPageStatePersister : PageStatePersister
{
    private Guid _session;

    public RaBrixPageStatePersister(Page page)
        : base(page)
    {
        if (page.IsPostBack)
        {
            _session = new Guid(page.Request["__VIEWSTATE_KEY"]);
        }
        else
        {
            _session = Guid.NewGuid();
        }
        if (!AjaxManager.Instance.IsCallback)
        {
            LiteralControl lit = new LiteralControl();
            lit.Text = string.Format(@"
<input type=""hidden"" value=""{0}"" name=""__VIEWSTATE_KEY"" />", _session);
            page.Form.Controls.Add(lit);
        }
    }

    public override void Load()
    {
        IPersistViewState state = Ra.Brix.Data.Internal.Adapter.Instance as IPersistViewState;
        LosFormatter formatter = new LosFormatter();
        Pair pair = formatter.Deserialize(state.Load(_session.ToString(), Page.Request.Url.ToString())) as Pair;
        ViewState = pair.First;
        ControlState = pair.Second;
    }

    public override void Save()
    {
        IPersistViewState state = Ra.Brix.Data.Internal.Adapter.Instance as IPersistViewState;
        LosFormatter formatter = new LosFormatter();
        StringBuilder builder = new StringBuilder();
        using (StringWriter writer = new StringWriter(builder))
        {
            formatter.Serialize(writer, new Pair(ViewState, ControlState));
        }
        state.Save(_session.ToString(), Page.Request.Url.ToString(), builder.ToString());
    }
}
