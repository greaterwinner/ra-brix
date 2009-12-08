<%@ Assembly 
    Name="WhiteboardModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="WhiteboardModules.ViewWhiteboard" %>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    style="position:relative;margin-bottom:25px;">

    <ra:LinkButton
        runat="server"
        style="position:absolute;top:5px;right:5px;"
        CssClass="favouriteButton"
        OnClick="addRow_Click"
        id="addRow" />
    <ext:Grid
        runat="server"
        CssClass="grid"
        OnRowDeleted="grid_RowDeleted"
        OnAction="grid_Action"
        OnCellEdited="grid_CellEdited"
        id="grd" />

</ra:Panel>
