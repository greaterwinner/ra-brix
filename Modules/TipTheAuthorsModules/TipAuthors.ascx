<%@ Assembly 
    Name="TipTheAuthorsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="TipTheAuthorsModules.TipAuthors" %>

<div style="padding:5px;">
    <h1><%=LanguageRecords.Language.Instance["SubmitNewTip", null, "Submit new tip"] %></h1>
    <ra:Label 
        runat="server" 
        Tag="div"
        id="information" />
    <ra:Panel 
        runat="server" 
        id="pnl" 
        DefaultWidget="submit"
        style="width:380px;display:table;margin-left:auto;margin-right:auto;">
        <ra:TextBox 
            runat="server" 
            style="font-size:16px;"
            Text="http://"
            id="tip" />
        <ra:ExtButton 
            runat="server" 
            id="submit" 
            OnClick="submit_Click"
            Text='<%#LanguageRecords.Language.Instance["SubmitLink", null, "Submit link"] %>' />
        <ra:Label 
            runat="server" 
            Visible="false"
            Text='<%#LanguageRecords.Language.Instance["ThankYouForSubmittingTip2", null, "Thank you for submitting a tip, hopefully someone will find it interesting and write up something about it :)<br/>Feel free to submit more tips..."] %>'
            id="thankYou" />
    </ra:Panel>
</div>