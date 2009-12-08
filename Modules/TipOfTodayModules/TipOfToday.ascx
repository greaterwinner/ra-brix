<%@ Assembly 
    Name="TipOfTodayModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="TipOfTodayModules.TipOfToday" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:5px;">
    <ra:Panel 
        runat="server" 
        style="display:none;"
        id="pnl">
        <h1><%= Language.Instance["DidYouKnow", null, "Did you know...?"] %></h1>
        <p>
            <%= GetTipOfToday() %>
        </p>
    </ra:Panel>
</div>
