<%@ Assembly 
    Name="HistoryModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="HistoryModules.History" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnAction="grid_Action"
    id="grd" />
