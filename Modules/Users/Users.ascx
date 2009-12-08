<%@ Assembly 
    Name="UsersModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="UsersModules.Users" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnRowDeleted="grid_Deleted"
    OnAction="grid_Action"
    id="grd" />
