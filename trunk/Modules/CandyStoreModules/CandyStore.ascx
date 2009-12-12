﻿<%@ Assembly 
    Name="CandyStoreModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CandyStoreModules.CandyStore" %>

<h1>
    <%=LanguageRecords.Language.Instance["RaBrixMarketPlace", null, "Ra-Brix Market Place"] %>
</h1>
<p>
    <%=LanguageRecords.Language.Instance["MarketPlaceDescription", null, @"
From here you can download and install new applications from <a target=""_blank"" 
href=""http://ra-brix.org/MarketPlace.aspx"">the Ra-Brix Market Place</a>, 
some will be free and others might cost you money. Please notice that many of these 
applications are not created or maintained by Ra-Software, but other independent vendors 
that have created Ra-Brix applications which we are hosting in our Market Place for 
those vendors."] %>
</p>

<ra:Panel 
    runat="server" 
    id="filterWrp" 
    DefaultWidget="doFilter"
    style="width:200px;overflow:hidden;position:relative;">
    <ra:Button 
        runat="server" 
        style="margin-left:-100px;position:absolute;"
        OnClick="doFilter_Click"
        id="doFilter" />
    <ra:TextBox 
        runat="server" 
        id="filter" />
</ra:Panel>

<ra:Panel 
    runat="server" 
    id="repWrp">
    <asp:Repeater runat="server" ID="rep">

        <ItemTemplate>
            <ra:Panel 
                runat="server" 
                OnClick="SelectModule"
                Xtra='<%#Eval("[\"CandyName\"].Value") %>'
                CssClass='<%#"candy " + GetCssClassAccordingToIsInstalled(Eval("[\"Installed\"].Value")) %>'
                ToolTip='<%#GetToolTip(Eval("[\"Installed\"].Value")) %>'
                style="opacity:0.3;">
                <ra:ExtButton 
                    runat="server" 
                    Text='<%#LanguageRecords.Language.Instance["Install", null, "Install"] %>'
                    style="position:absolute;top:15px;left:220px;"
                    OnClick="ClickToInstallModule" />
                <strong 
                    style="margin-bottom:10px;display:block;position:absolute;top:5px;left:5px;">
                    <%#Eval("[\"CandyName\"].Value").ToString().Replace(".zip", "") %>
                </strong>
                <img 
                    src='<%#Eval("[\"CandyUrl\"].Value") %>' 
                    style="position:absolute;top:40px;left:55px;"
                    alt='<%#Eval("[\"CandyName\"].Value") %>' />
                <div style="position:absolute;top:140px;">
                    <p 
                        style="font-style:italic;position:absolute;left:5px;font-size:10px;">
                        <%# ((DateTime)Eval("[\"Date\"].Value")).ToString("dddd d. MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture)%>
                    </p>
                    <p>&nbsp;</p>
                    <ra:Label 
                        runat="server" 
                        Tag="div"
                        style="display:none;text-align:justify;border:dashed 1px #999;overflow:auto;height:100px;width:90%;padding:10px;font-size:10px;"
                        Text='<%# Eval("[\"Description\"].Value")%>' />
                </div>
                <ra:BehaviorUnveiler 
                    runat="server" />
            </ra:Panel>
        </ItemTemplate>

    </asp:Repeater>
</ra:Panel>
<br style="clear:both;" />
<p>
    <%=LanguageRecords.Language.Instance["ContributeToMarketPlace", null, @"
If you want to create Ra-Brix applications yourself and have all users of Ra-Brix be
able to download and install them, either for money or for free, then read more about
the Ra-Brix Market Place <a target=""_blank"" href=""http://ra-brix.org/MarketPlace.aspx"">here</a>."] %>
</p>