<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="WhiteboardModules.ViewAllWhiteboards" %>

<ext:Grid
    runat="server"
    CssClass="grid"
    PageSize="8"
    OnRowDeleted="grid_RowDeleted"
    AutoDeleteRowOnDeletion="false"
    OnAction="grid_Action"
    id="grd" />

