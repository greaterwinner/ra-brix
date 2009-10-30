/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Collections.Generic;
using FavouritesRecords;
using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Brix.Data;
using System.Web;

namespace FavouritesController
{
    [ActiveController]
    public class FavouritesController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonAppl", "Applications");
            Language.Instance.SetDefaultValue("ButtonFavourites", "Favorites");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonFavourites"].Value = "Menu-ViewFavourites";
        }

        private static Tuple<Tuple<string, string>, string>? LastClickedMenu
        {
            get
            {
                return (Tuple<Tuple<string, string>, string>?)
                       HttpContext.Current.Session["FavouritesController.Controller.LastClickedMenu"];
            }
            set
            {
                HttpContext.Current.Session["FavouritesController.Controller.LastClickedMenu"] = value;
            }
        }

        [ActiveEvent(Name = "MenuItemClicked")]
        protected void MenuItemClicked(object sender, ActiveEventArgs e)
        {
            string menuEventToRaise = e.Params["MenuEventName"].Get<string>();
            string param = e.Params["Params"].Get<string>();
            if (menuEventToRaise != "Menu-ViewFavourites")
            {
                string menuText = e.Params["MenuText"].Get<string>();
                LastClickedMenu = new Tuple<Tuple<string, string>, string>(
                    new Tuple<string, string>(param, menuText), menuEventToRaise);
            }
        }

        [ActiveEvent(Name="AddFavourite")]
        protected void AddFavourite(object sender, ActiveEventArgs e)
        {
            if (!LastClickedMenu.HasValue)
            {
                Node node = new Node();
                const string message = @"You need to open something before you can bookmark...";
                node["Message"].Value = Language.Instance["FavouritesBookmarkInfo", null, message];
                node["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            else
            {
                List<Criteria> crits = new List<Criteria>
               {
                   Criteria.Eq("EventName", LastClickedMenu.Value.Right)
               };
                if( !string.IsNullOrEmpty(LastClickedMenu.Value.Left.Left))
                    crits.Add(Criteria.Eq("Param", LastClickedMenu.Value.Left.Left));
                if (ActiveRecord<Favourite>.CountWhere(crits.ToArray()) > 0)
                {
                    Node node = new Node();
                    const string message = @"Item already bookmarked";
                    node["Message"].Value = Language.Instance["FavouritesItemAlreadyInfo", null, message];
                    node["Duration"].Value = 2000;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        node);
                    e.Params["Failure"].Value = true;
                    return;
                }

                Favourite favourite = new Favourite
                {
                    EventName = LastClickedMenu.Value.Right,
                    Name = LastClickedMenu.Value.Left.Right,
                    Username = Users.LoggedInUserName,
                    Param = LastClickedMenu.Value.Left.Left
                };
                favourite.Save();
            }
            e.Params["Grid"]["Columns"]["MenuItem"]["Caption"].Value = Language.Instance["FavouritesCaption", null, "Menu Item"];
            e.Params["Grid"]["Columns"]["MenuItem"]["ControlType"].Value = "LinkButton";

            int idxNo = 0;
            foreach (Favourite idx in ActiveRecord<Favourite>.Select(Criteria.Eq("Username", Users.LoggedInUserName)))
            {
                e.Params["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                e.Params["Grid"]["Rows"]["Row" + idxNo]["MenuItem"].Value = idx.Name;
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "OpenFavourite")]
        protected void OpenFavourite(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["ID"].Get<string>());
            Favourite fav = ActiveRecord<Favourite>.SelectByID(id);
            Node node = new Node();
            node["Params"].Value = fav.Param;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                fav.EventName,
                node);
        }

        [ActiveEvent(Name = "DeleteFavourite")]
        protected void DeleteFavourite(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["ID"].Get<string>());
            ActiveRecord<Favourite>.SelectByID(id).Delete();
        }

        [ActiveEvent(Name = "Menu-ViewFavourites")]
        protected void ViewFavourites(object sender, ActiveEventArgs e)
        {
            Node node = new Node();

            node["Message"].Value = Language.Instance["FavouritesViewInfo", null, @"To view favourites: 
Move the cursor over the bull's eye on the right side of this information message."];
            node["Duration"].Value = 5000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                node);

            node["TabCaption"].Value = "Favoritter";
            node["Width"].Value = 500;
            node["Height"].Value = 380;

            List<Favourite> lst = 
                new List<Favourite>(
                    ActiveRecord<Favourite>.Select(Criteria.Eq("Username", Users.LoggedInUserName)));
            lst.Sort(
                delegate(Favourite left, Favourite right)
                    {
                        return left.ID.CompareTo(right.ID);
                    });
           
            node["ModuleSettings"]["Grid"]["Columns"]["MenuItem"]["Caption"].Value = "Menu Item";
            node["ModuleSettings"]["Grid"]["Columns"]["MenuItem"]["ControlType"].Value = "LinkButton";

            int idxNo = 0;
            foreach (Favourite idx in lst)
            {
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["MenuItem"].Value = idx.Name;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "FavouritesModules.Favourites",
                "dynPopup2",
                node);
        }
    }
}