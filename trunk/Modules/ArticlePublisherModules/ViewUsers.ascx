<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewUsers" %>

<style type="text/css">
.user
{
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
	display:block;
	float:left;
	margin:10px;
	overflow:auto;
	min-width:250px;
}

.user:hover
{
	background-color:#c8e6f7;
}
</style>

<ra:Label 
    runat="server" 
    Tag="h1"
    style="display:table;margin-left:auto;margin-right:auto;"
    id="header" />

<asp:Repeater 
    runat="server" 
    ID="rep">
    <HeaderTemplate>
        <div style="display:table;margin-left:auto;margin-right:auto;">
    </HeaderTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
    <ItemTemplate>
        <a href='<%#Eval("[URL].Value") %>' class="user">
            <img 
                runat="server" 
                id="gravatar" 
                src='<%#Eval("[ImageSrc].Value") %>'
                alt='<%#Eval("[Name].Value") %>'
                style="float:left;margin-right:15px;width:64px;height:64px;overflow:hidden;" />
            <div style="float:left;">
                <%#Eval("[Name].Value") %>
                <br />
                <%#LanguageRecords.Language.Instance["Score", null, "Score"] %>: 
                <%#Eval("[Score].Value") %>
            </div>
        </a>
    </ItemTemplate>
</asp:Repeater>
<br style="clear:both;" />