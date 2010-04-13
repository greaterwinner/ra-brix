<%@ Assembly 
    Name="PromoModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="PromoModules.ApplyForPromoProgram" %>

<div style="padding:5px;">
    <h1 style="margin-bottom:25px;">Ra-Software Promo Program</h1>
    <ra:Panel 
        runat="server" 
        CssClass="promo"
        DefaultWidget="submit"
        id="pnlPromo">
        <ra:Label 
            runat="server" 
            id="lblErr" 
            Tag="p"
            style="color:Red;display:none;" />
        <table class="promoTable">
            <tr>
                <td>
                    <ra:TextBox 
                        runat="server" 
                        id="emailAdr" />
                </td>
            </tr>
            <tr>
                <td>
                    <ra:TextBox 
                        runat="server" 
                        id="wantedCode" />
                </td>
            </tr>
            <tr>
                <td>
                    <ra:RadioButton 
                        runat="server" 
                        GroupName="cause"
                        Checked="true"
                        Text="WikiPedia"
                        id="rdo1" />
                </td>
            </tr>
            <tr>
                <td>
                    <ra:RadioButton 
                        runat="server" 
                        GroupName="cause"
                        Text="OLPC"
                        id="rdo2" />
                </td>
            </tr>
            <tr>
                <td>
                    <ra:RadioButton 
                        runat="server" 
                        GroupName="cause"
                        Text="Rainforest Foundation US"
                        id="rdo3" />
                </td>
            </tr>
            <tr>
                <td>
                    <ra:RadioButton 
                        runat="server" 
                        GroupName="cause"
                        Text="KIVA"
                        id="rdo4" />
                </td>
            </tr>
            <tr>
                <td style="text-align:right;">
                    <ra:ExtButton 
                        runat="server" 
                        OnClick="submit_Click"
                        id="submit" />
                </td>
            </tr>
        </table>
    </ra:Panel>
    <ra:Panel 
        runat="server" 
        Visible="false"
        CssClass="promo"
        id="pnlSuccess">
        <%=LanguageRecords.Language.Instance["PromoCodeWasSuccess", null, @"
<p>
    Congratulations! Now you need to tell people about your promo code, and how they 
    will save $200.
    When someone buys a website with your code, we will send you
    an email asking where you want us to send your $200.
</p>
<p>
    Every consecutive time we sell a website, with your promo code, we will 
    automatically send you another $200, and your choice of nonprofit cause another $100.
</p>
<p>
    You should be getting an email shortly with further instructions.
</p>
"] %>
    </ra:Panel>
    <%=LanguageRecords.Language.Instance["RaPromoCodeInformation", null, @"
<h2>How it works</h2>
<ul>
    <li>You register your desired promo code</li>
    <li>You pass this promo code around</li>
    <li>Someone <strong>saves $200</strong> buying a website with your code</li>
    <li>You <strong>earn $200</strong> now</li>
    <li>We <strong>send $100 to your nonprofit</strong> of choice</li>
</ul>
<h2>What you're selling</h2>
<p>
    The appearance of the website you're selling, will
    resemble this website. Show this website to your friends if they
    need to see a reference of how their website will look like.
</p>

"] %>
    <p style="clear:both;">
        Want to have your personal favorite nonprofit on the list?
        <a href="http://rasoftwarefactory.com/contact-us">Contact us</a> with the name of 
        your cause, and an explanation of why this cause deserves to be on this list.
    </p>
</div>
