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

public class RaBrixPageStatePersister : PageStatePersister
{
    public RaBrixPageStatePersister(Page page)
        : base(page)
    { }

    public override void Load()
    {
        IPersistViewState state = Ra.Brix.Data.Internal.Adapter.Instance as IPersistViewState;
        LosFormatter formatter = new LosFormatter();
        Pair pair = formatter.Deserialize(state.Load(Page.Session.SessionID, Page.Request.Url.ToString())) as Pair;
        ViewState = pair.First;
        ControlState = pair.Second;
    }

    public override void Save()
    {
        if (ViewState != null || ControlState != null)
        {
            if (Page.Session != null)
            {
                IPersistViewState state = Ra.Brix.Data.Internal.Adapter.Instance as IPersistViewState;
                LosFormatter formatter = new LosFormatter();
                StringBuilder builder = new StringBuilder();
                using (StringWriter writer = new StringWriter(builder))
                {
                    formatter.Serialize(writer, new Pair(ViewState, ControlState));
                }
                state.Save(Page.Session.SessionID, Page.Request.Url.ToString(), builder.ToString());
            }
            else
                throw new InvalidOperationException("Session needed for RaBrixPageStatePersister...");
        }
    }
}
