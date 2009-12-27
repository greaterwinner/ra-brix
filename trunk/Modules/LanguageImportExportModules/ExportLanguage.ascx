<%@ Assembly 
    Name="LanguageImportExportModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LanguageImportExportModules.ExportLanguage" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="btnSubmit"
    style="position:relative;width:100%;height:100%;">
    <div style="padding:15px;">
        <ext:ExtButtonPost 
            runat="server" 
            id="btnSubmit" 
            style="position:absolute;right:15px;bottom:10px;"
            OnClick="btnSubmit_Click"
            Text='<%#Language.Instance["LanguageExportSaveButton", null, "Save"]%>' />
    </div>
</ra:Panel>

