<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ADGroups2RolesModules.ViewMappings" %>
                
<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnCellEdited="grid_CellEdited"
    OnRowDeleted="grid_RowDeleted"
    id="grd" />
