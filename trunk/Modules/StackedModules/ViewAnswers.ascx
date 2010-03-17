<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.ViewAnswers" %>

<style type="text/css">
.answer
{
    margin:35px 5px 5px 0;
    padding:35px 10px 10px 10px;
    border:dashed 1px #999;
	background-color:#d6f4ff;
	position:relative;
}

.user
{
	display:block;
	position:absolute;
	margin-top:-25px;
	top:5px;
	right:15px;
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
}

.user:hover
{
	background-color:#c8e6f7;
}
</style>

<ra:Panel 
    runat="server" 
    id="repWrp">
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <div class="answer">
                <a href='<%#GetUsernameLink(Eval("[Username].Value")) %>' class="user">
                    <img 
                        runat="server" 
                        id="gravatar" 
                        src='<%#GetGravatar(Eval("[Email].Value")) %>'
                        style="float:left;margin-right:15px;width:32px;height:32px;overflow:hidden;" />
                    <%#Eval("[Username].Value") %>
                </a>
                <%#Eval("[Body].Value") %>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</ra:Panel>