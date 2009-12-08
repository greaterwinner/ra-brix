<%@ Assembly 
    Name="ChangeSkinModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChangeSkinModules.UploadNewSkin" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:15px;">
    <asp:FileUpload runat="server" ID="file" />
    <br />
    <asp:Button 
        runat="server" 
        id="submit" 
        OnClick="submit_Click"
        Text='<%# Language.Instance["Submit", null, "Submit"] %>' />
</div>

