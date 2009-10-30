<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LanguageEditModules.EditLanguage" %>

<ext:Grid 
    runat="server" 
    CssClass="grid"
    PageSize="8"
    OnCellEdited="grid_CellEdited"
    id="grd" />
