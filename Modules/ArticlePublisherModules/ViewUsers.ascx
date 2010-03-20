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
}

.user:hover
{
	background-color:#c8e6f7;
}
</style>

<ra:Label 
    runat="server" 
    Tag="h1"
    id="header" />

<asp:Repeater 
    runat="server" 
    ID="rep">
    <ItemTemplate>
        <a href='<%#Eval("[URL].Value") %>' class="user">
            <img 
                runat="server" 
                id="gravatar" 
                src='<%#Eval("[ImageSrc].Value") %>'
                alt='<%#Eval("[Name].Value") %>'
                style="float:left;margin-right:15px;width:64px;height:64px;overflow:hidden;" />
            <div style="float:right;">
                <%#Eval("[Name].Value") %>
                <br />
                <%#LanguageRecords.Language.Instance["Score", null, "Score"] %>: 
                <%#Eval("[Score].Value") %>
            </div>
        </a>
    </ItemTemplate>
</asp:Repeater>
<br style="clear:both;" />