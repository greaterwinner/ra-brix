﻿<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.ViewPages" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="width:100%;height:100%;">
    <ra:Panel 
        runat="server"
        id="infoWrp"
        CssClass="CMSInfoLabel">
        <strong>
            <%=Language.Instance["SelectPageToEdit", null, "Select a page from the floating window to edit it"] %>
        </strong>
    </ra:Panel>
    <ra:Panel 
        runat="server"
        id="editWrp"
        Visible="false"
        CssClass="CMSEditPage">
        <ra:InPlaceEdit 
            runat="server" 
            CssClass="edit"
            style="position:absolute;top:8px;right:8px;z-index:5;"
            id="header" />
        <ra:Panel 
            runat="server" 
            style="position:absolute;top:8px;left:8px;z-index:5;"
            id="url">
            <%=Language.Instance["Preview", null, "Preview"] %> <a 
                runat="server" 
                target="_blank"
                id="hyperlink" />
        </ra:Panel>
        <ra:RichEdit
            ID="editor" 
            CssClass="editor"
            OnGetImageDialog="editor_GetImageDialog"
            OnGetResourceDialog="editor_GetResourceDialog"
            OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
            CtrlKeys="s"
            OnCtrlKey="editor_CtrlKeys"
            OnGetExtraToolbarControls="editor_GetExtraToolbarControls"
            runat="server" />
    </ra:Panel>
</div>
