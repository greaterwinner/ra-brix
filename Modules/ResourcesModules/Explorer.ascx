<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ResourcesModules.Explorer" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="width:98%;height:94%;padding:1%;">
    <div style="width:30%;height:100%;float:left;">
        <ra:Tree 
            runat="server" 
            CssClass="tree"
            ClientSideExpansion="false"
            OnSelectedNodeChanged="tree_SelectedNodeChanged"
            ID="tree">

            <ra:TreeNodes 
                runat="server" 
                ID="root" />

        </ra:Tree>
    </div>
    <ra:Panel
        runat="server"
        id="grdWrapper"
        Visible="false"
        CssClass="resourceExplorerFiles">
        <div>
            <%=Language.Instance["UploadFile", null, "Upload File: "] %>
            <asp:FileUpload 
                runat="server"
                ID="fileUpload" />
            <asp:Button 
                runat="server" 
                ID="btnSaveFile" 
                OnClick="btnSaveFile_Click"
                Text='<%#Language.Instance["Save", null, "Save"] %>' />
        </div>
        <ext:Grid 
            runat="server" 
            CssClass="grid"
            PageSize="4"
            OnAction="grd_Action"
            OnRowDeleted="grid_RowDeleted"
            id="grd" />
    </ra:Panel>
</div>


