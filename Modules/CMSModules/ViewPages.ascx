<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.ViewPages" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="width:98%;height:94%;padding-left:1%;padding-right:1%;padding-top:1%;">
    <div style="width:20%;height:100%;float:left;">
        <ra:Tree 
            runat="server" 
            ClientSideExpansion="false"
            CssClass="tree CMSTree"
            OnSelectedNodeChanged="menu_SelectedNodeChanged"
            ID="tree">

            <ra:TreeNodes 
                runat="server" 
                ID="root" />

        </ra:Tree>
    </div>
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
            OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
            CtrlKeys="s,g"
            OnCtrlKey="editor_CtrlKeys"
            OnGetExtraToolbarControls="editor_GetExtraToolbarControls"
            runat="server" />
    </ra:Panel>
</div>
