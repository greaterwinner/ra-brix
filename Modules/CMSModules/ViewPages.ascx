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
            style="position:absolute;top:10px;right:10px;z-index:5;"
            id="header" />
        <ra:Panel 
            runat="server" 
            style="position:absolute;top:30px;right:10px;z-index:5;"
            id="url">
            Link: <a 
                runat="server" 
                target="_blank"
                id="hyperlink" />
        </ra:Panel>
        <ext:RichEdit
            ID="editor" 
            CssClass="editor"
            OnGetImageDialog="editor_GetImageDialog"
            OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
            OnGetPluginControls="editor_GetPluginControls"
            runat="server" />
        <ra:ExtButton 
            runat="server" 
            id="delete" 
            OnClick="delete_Click"
            style="position:absolute;bottom:0;right:0;margin-right:50px;margin-bottom:-4px;"
            Text='<%#Language.Instance["Delete", null, "Delete"] %>' />
        <ra:ExtButton 
            runat="server" 
            id="submit" 
            OnClick="submit_Click"
            style="position:absolute;bottom:0;right:0;margin-right:-8px;margin-bottom:-4px;"
            Text='<%#Language.Instance["Save", null, "Save"] %>' />
    </ra:Panel>
</div>
