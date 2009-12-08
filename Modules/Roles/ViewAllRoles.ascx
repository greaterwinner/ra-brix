<%@ Assembly 
    Name="RolesModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RolesModules.ViewAllRoles" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnRowDeleted="grid_Deleted"
    id="grd" />
