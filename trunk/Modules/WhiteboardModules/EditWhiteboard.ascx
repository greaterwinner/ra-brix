<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="WhiteboardModules.EditWhiteboard" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="padding:35px 0px 0px 0px;position:relative;margin-bottom:25px;">
    <ra:LinkButton
        runat="server"
        style="position:absolute;top:5px;right:5px;"
        CssClass="favouriteButton"
        OnClick="addColumn_Click"
        id="addColumn" />
    <ra:InPlaceEdit 
        runat="server" 
        style="font-size:118%;float:left;width:20%;margin-left:27px;margin-top:-15px;"
        CssClass="edit"
        OnTextChanged="header_TextChanged"
        id="header" />

    <ra:Panel
        runat="server"
        style="opacity:0.3;border:dashed 1px #666;position:absolute;top:5px;right:40px;width:150px;padding:5px;"
        id="settingsWrp">
        <ra:CheckBox
            runat="server"
            style="display:block;"
            id="enableFiltering"
            OnCheckedChanged="enableFiltering_CheckedChanged"
            Tooltip='<%#Language.Instance["WhiteboardEnableFilteringTooltip", null, "Whether or not the Whiteboard should allow the user to filter items or not..."] %>'
            Text='<%#Language.Instance["WhiteboardEnableFiltering", null, "Enable filtering"] %>' />
        <ra:InPlaceEdit
            runat="server"
            CssClass="edit"
            Tooltip='<%#Language.Instance["WhiteboardPageSizeToolTip", null, "Number of records to show in Whiteboard before clicking next-button is needed. -1 means never..."] %>'
            OnTextChanged="txtPageSize_TextChanged"
            id="txtPageSize" />
        <ra:BehaviorUnveiler
            runat="server"
            id="unveilSettings" />
    </ra:Panel>

    <ra:Panel
        runat="server"
        style="opacity:0.3;border:dashed 1px #666;position:absolute;top:5px;right:210px;width:150px;padding:5px;"
        id="settingsHeaders">
        <ra:CheckBox
            runat="server"
            style="display:block;"
            id="chkEnableHeader"
            OnCheckedChanged="chkEnableHeader_CheckedChanged"
            Tooltip='<%#Language.Instance["WhiteboardEnableColumnsTooltip", null, "Whether or not the Whiteboard should have headers for columns that can sort on columns etc..."] %>'
            Text='<%#Language.Instance["WhiteboardEnableGridColumns", null, "Enable Headers"] %>' />
        <ra:CheckBox
            runat="server"
            style="display:block;"
            id="chkEnableDeletion"
            OnCheckedChanged="chkEnableDeletion_CheckedChanged"
            Tooltip='<%#Language.Instance["WhiteboardEnableDeletionTooltip", null, "Whether or not the Whiteboard allows for deletion of records"] %>'
            Text='<%#Language.Instance["WhiteboardEnableGridDeletion", null, "Enable Deletion"] %>' />
        <ra:BehaviorUnveiler
            runat="server"
            id="unveilSettingsHeaders" />
    </ra:Panel>

    <ext:Grid
        runat="server"
        CssClass="grid"
        style="clear:both;margin-top:15px;"
        OnRowDeleted="grid_RowDeleted"
        OnCellEdited="grid_CellEdited"
        PageSize="-1"
        EnableFilter="false"
        id="grd" />
</div>
