<%@ Assembly 
    Name="InstalledAppsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="InstalledAppsModules.ViewAllApps" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnAction="grd_Action"
    id="grd" />
