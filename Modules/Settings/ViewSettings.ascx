<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="SettingsModules.ViewSettings" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnCellEdited="grid_CellEdited"
    OnRowDeleted="grid_RowDeleted"
    id="grd" />
