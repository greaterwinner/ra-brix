<%@ Assembly 
    Name="InstalledAppsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="InstalledAppsModules.ViewAppDetails" %>

<div style="padding:5px;">
    <h1 runat="server" id="header" />
    <p style="font-style:italic;color:#999;" runat="server" id="date" />
    <strong>
        <%=LanguageRecords.Language.Instance["FilesInApplication", null, "Files in application; "] %>
        <span 
            runat="server" 
            id="numberOfFiles" />
    </strong>
    <asp:Repeater runat="server" ID="rep">
        <HeaderTemplate>
            <div class="fileGrid">
        </HeaderTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <div class="fileGridItemWrp">
                <ra:Panel 
                    runat="server" 
                    CssClass="fileGridItem"
                    OnClick="MouseClick"
                    ToolTip='<%#LanguageRecords.Language.Instance["ClickToSeeDetails", null, "Click me to see details..."] %>'
                    Xtra='<%#Eval("Value") %>'
                    style="opacity:0.3;">
                    <div class="fileGridItemContent">
                        <div class='<%# GetCssClassForFile(Eval("Value")) %>'>&nbsp;</div>
                        <div class="fileGridTitle">
                            <%# GetCutFileName(Eval("Value")) %>
                        </div>
                        <div class="extraInformation">
                            <div>
                                <%#Eval("[\"FullPath\"].Value") %>
                            </div>
                            <div class='<%# GetClassForFileDate(Eval("[\"Created\"].Value")) %>'>
                                <%#((DateTime)Eval("[\"Created\"].Value")).ToString("dddd d. MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture)%>
                            </div>
                            <div>
                                <%#((long)Eval("[\"Size\"].Value")).ToString("### ### ###", System.Globalization.CultureInfo.InvariantCulture)%> bytes...
                            </div>
                        </div>
                    </div>
                    <ra:BehaviorUnveiler 
                        MinOpacity="0.3"
                        runat="server" />
                </ra:Panel>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <br style="clear:both;" />
    <strong 
        runat="server" 
        id="summaryKb" />
</div>
