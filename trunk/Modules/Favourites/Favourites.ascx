<%@ Assembly 
    Name="FavouritesModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="FavouritesModules.Favourites" %>

<ra:LinkButton 
    runat="server"
    ID="addFavourites"
    Text="&nbsp;"
    CssClass="favouriteButton"
    OnClick="addFavourites_Click"
    style="position:absolute;right:5px;top:5px;z-index:100;"/>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnAction="grid_Action"
    OnRowDeleted="grid_Deleted"
    id="grd" />
