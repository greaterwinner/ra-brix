<%@ Assembly 
    Name="InstalledAppsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="InstalledAppsModules.ViewAllApps" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="10"
    id="grd" />
