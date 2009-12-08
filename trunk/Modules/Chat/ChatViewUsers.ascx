<%@ Assembly 
    Name="ChatModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChatModules.ChatViewUsers" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnAction="grid_Action"
    id="grd" />
