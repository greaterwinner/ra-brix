<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.ViewPosts" %>

<style type="text/css">

.questions
{
	margin-top:40px;
}

.question
{
	padding:5px;
	margin:5px;
	border:solid 1px #999;
	display:block;
	overflow:auto;
}

.question:hover
{
	background-color:#d6f4ff;
}

.qDate
{
	display:block;
	float:left;
	width:120px;
}

.qHeader
{
	display:block;
	float:left;
	width:350px;
}

</style>

<div style="padding:5px;position:relative;">
    <ra:ExtButton 
        runat="server" 
        id="ask" 
        style="position:absolute;top:5px;right:5px;"
        OnClick="ask_Click"
        Text='<%#LanguageRecords.Language.Instance["Ask", null, "Ask..."] %>' />
    <ra:Panel 
        runat="server" 
        CssClass="questions"
        id="repWrp">
        <asp:Repeater 
            runat="server" 
            ID="rep">
            <ItemTemplate>
                <ra:Panel 
                    style="clear:both;"
                    runat="server">
                    <a 
                        class="question" 
                        href='<%#Eval("[URL].Value") %>'>
                        <span class="qDate">
                            <%#HelperGlobals.DateFormatter.FormatDate((DateTime)Eval("[Asked].Value")) %>
                        </span>
                        <span class="qHeader">
                            <%#Eval("[Header].Value") %>
                        </span>
                    </a>
                </ra:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </ra:Panel>
</div>