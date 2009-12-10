<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="InstalledAppsModules.InstallNewApp" %>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="submit"
    style="padding:5px;position:relative;height:100%;">
    <%=LanguageRecords.Language.Instance["FindAppZipFile", null, "Find an application ZIP file to upload"] %>
    <asp:FileUpload 
        runat="server" 
        ID="uploader" />
    <br />
    <asp:Button 
        runat="server" 
        style="position:absolute;right:5px;bottom:15px;"
        OnClick="submit_Click"
        Text='<%#LanguageRecords.Language.Instance["Submit", null, "Submit"] %>'
        id="submit" />
</ra:Panel>

