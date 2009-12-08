<%@ Assembly 
    Name="LoginOpenIDModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LoginOpenIDModules.ReportAbuse" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div runat ="server" id="header" style="padding:5px;">
    <h1>
    <%#Language.Instance["OpenIDAbuseInfoHeader", null, "Report abuse..."]%>
    </h1>
    <%#Language.Instance["OpenIDAbuseInfo", null, @"<p>You have come here because you think someone 
        have abused your credentials on this portal. This might happen due to several reasons. 
        One being that you've logged in from another person's computer, and then either you've 
        allowed your OpenID provider to authenticate you every time from that computer. 
        Or something similar. If you do this then there's a high risk of that the next person 
        on that computer can actually log in as you.</p> <p>For the reasons stated above it's 
        therefor highly important that you explicitly choose to Logout when you're done with your 
        session on this portal, in addition to that you also close all instances of the browser 
        when finished browsing this portal. </p>"]%>
        
    <h2>
        <%#Language.Instance["OpenIDAbuseFixInfoHeader", null, @"How to fix"]%>
    </h2>
    <p>
        <%#Language.Instance["OpenIDAbuseFixInfo", null, @"The way to fix this is to first click 
        the button below to temporary block your user. Then to change your password on your OpenID provider.
        Before requesting an unblock by the administrator of this portal. It's also important to 
        use passwords which are not easy to guess, like for instance your birthday or something similar."] %>
    </p>
    <ul class="list">
        <li><%#Language.Instance["OpenIDAbuseList1", null, @"Temporary block your user by clicking the button below"]%></li>
        <li><%#Language.Instance["OpenIDAbuseList2", null, @"Change your password at your OpenID provider"]%></li>
        <li><%#Language.Instance["OpenIDAbuseList3", null, @"Request an unblock by the administrator of this portal"]%></li>
    </ul>
    <ra:ExtButton 
        runat="server" 
        id="blockBtn" 
        OnClick="blockBtn_Click"
        Text='<%#Language.Instance["OpenIDBlockButton", null, "Block my user"] %>' />
</div>