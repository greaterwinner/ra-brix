<%@ Assembly 
    Name="ChangeSkinModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChangeSkinModules.UploadNewSkin" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="position:relative;height:100%;">
    <asp:FileUpload 
        runat="server" 
        ID="file" />
    <ext:ExtButtonPost 
        runat="server" 
        id="submit" 
        OnClick="submit_Click"
        style="position:absolute;right:5px;bottom:5px;"
        Text='<%# Language.Instance["Submit", null, "Submit"] %>' />
</div>

