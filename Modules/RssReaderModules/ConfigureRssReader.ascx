<%@ Assembly 
    Name="RssReaderModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RssReaderModules.ConfigureRssReader" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:5px;position:relative;">
    <ra:LinkButton
        runat="server"
        style="position:absolute;top:0;right:5px;opacity:0.3;"
        CssClass="favouriteButton"
        OnClick="addColumn_Click"
        id="addColumn">
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unvel" />
    </ra:LinkButton>
    <h2><%=Language.Instance["ConfigureRssModule", null, "Configure RSS module"]%></h2>
    <ext:Grid 
        runat="server" 
        CssClass="grid"
        PageSize="8"
        OnCellEdited="grid_CellEdited"
        OnRowDeleted="grid_RowDeleted"
        id="grd" />
</div>


