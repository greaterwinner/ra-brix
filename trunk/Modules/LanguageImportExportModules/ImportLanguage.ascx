<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LanguageImportExportModules.ImportLanguage" %>
<%@ Import Namespace="LanguageRecords"%>

<asp:FileUpload 
     runat="server"
     id="upload" />

<asp:Button 
     runat="server"
     id="btn"
     Text='<%# Language.Instance["LanguageImportButton", null, "Import Language"] %>'
     OnClick="btn_Click"/>
 
