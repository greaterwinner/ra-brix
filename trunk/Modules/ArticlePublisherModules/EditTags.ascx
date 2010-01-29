<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.EditTags" %>

<ra:Panel 
    runat="server" 
    id="repWrp">

    <asp:Repeater runat="server" ID="rep">
        <ItemTemplate>
            <div style="float:left;width:100px;border:dashed 1px #999;margin:5px;padding:5px;">
                <a 
                    style="display:block;white-space:nowrap;padding:5px;"
                    target="_blank"
                    href='<%#Eval("[URL].Value") %>'>
                    <%#Eval("[Name].Value") %>
                </a>
                <ra:CheckBox 
                    runat="server" 
                    OnCheckedChanged="StickyChanged"
                    style="display:block;"
                    Xtra='<%#Eval("[ID].Value") %>'
                    ToolTip='<%#LanguageRecords.Language.Instance["StickyMeans", null, "Sticky means that the tag will be visible on the main landing page as a category"] %>'
                    Checked='<%#Eval("[Sticky].Value") %>'
                    Text='<%#LanguageRecords.Language.Instance["Sticky", null, "Sticky"] %>' />
                <ra:ExtButton 
                    runat="server" 
                    Xtra='<%#Eval("[ID].Value") %>'
                    Text='<%#LanguageRecords.Language.Instance["Delete", null, "Delete"] %>'
                    OnClick="DeleteTag" />
            </div>
        </ItemTemplate>
    </asp:Repeater>

</ra:Panel>


