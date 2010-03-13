<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ImportRSS" %>

<ra:Panel 
    runat="server" 
    id="wrp" 
    DefaultWidget="submit"
    style="padding:5px;">
    <%=LanguageRecords.Language.Instance["TypeInUrlToRSS", null, "Type in a URL to an RSS feed"] %>
    <br />
    <ra:TextBox 
        runat="server" 
        Text="URL..."
        id="url" />
    <ra:ExtButton 
        runat="server" 
        id="submit" 
        OnClick="submit_Click"
        Text='<%#LanguageRecords.Language.Instance["Import", null, "Import"] %>' />
</ra:Panel>