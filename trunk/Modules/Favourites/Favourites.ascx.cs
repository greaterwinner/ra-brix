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
using LanguageRecords;
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Components;

namespace FavouritesModules
{
    [ActiveModule]
    public class Favourites : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void addFavourites_Click(object sender, EventArgs e)
        {
            Node init = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AddFavourite",
                init);
            if (init["Failure"].Value == null)
            {
                grd.DataSource = init["Grid"];
                grd.Rebind();
            }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can add Favorites which are applications you use very frequently and that will be accessible
as shortcuts from the Favorites application. Then to switch between these different applications
can be easily done by starting the Favorites module and then switching applications from that.

Notice however that the Favorites module will create a Favorite of your last opened application 
and not your currently active one (necessarily).
";
            e.Params["Tip"]["TipOfFavorites"].Value = Language.Instance["TipOfFavorites", null, tmp];
        }

        protected void grid_Deleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteFavourite",
                node);
        }

        protected void grid_Action(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "OpenFavourite",
                node);
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];
        }

        public string GetCaption()
        {
            return "";
        }
    }
}