<%@ Assembly 
    Name="LanguageImportExportModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LanguageImportExportModules.ImportLanguage" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<asp:FileUpload 
     runat="server"
     id="upload" />

<ext:ExtButtonPost
     runat="server"
     id="btn"
     style="position:absolute;right:15px;bottom:10px;"
     Text='<%# Language.Instance["LanguageImportButton", null, "Import"] %>'
     OnClick="btn_Click"/>
 
