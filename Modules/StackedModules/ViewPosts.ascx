<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.ViewPosts" %>

<style type="text/css">

.questions
{
	margin-top:35px;
}

.question
{
    margin:35px 5px 5px 0;
    padding:25px 25px 25px 25px;
    border:dashed 1px #999;
	background-color:#d6f4ff;
	position:relative;
	overflow:auto;
	display:block;
}

.question:hover
{
	background-color:#c8e6f7;
}

span.question:hover
{
	background-color:#d6f4ff;
}

.qNoAnswers
{
	display:block;
	float:left;
	width:70px;
}

.qDate
{
	display:block;
	float:left;
	width:110px;
}

.qHeader
{
	display:block;
	float:left;
	width:350px;
	font-weight:bold;
}

.headerStacked
{
	font-weight:bold;
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

<div style="padding:5px;position:relative;">
    <h1><%=LanguageRecords.Language.Instance["QuestionsAndAnswer", null, "Questions and Answers"] %></h1>
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
            <HeaderTemplate>
                <span 
                    class="question">
                    <span class="qNoAnswers headerStacked">
                        <%=LanguageRecords.Language.Instance["Answers", null, "Answers"]%>
                    </span>
                    <span class="qDate headerStacked">
                        <%=LanguageRecords.Language.Instance["Started", null, "Started"]%>
                    </span>
                    <span class="qDate headerStacked">
                        <%=LanguageRecords.Language.Instance["Activity", null, "Activity"]%>
                    </span>
                    <span class="qHeader headerStacked">
                        <%=LanguageRecords.Language.Instance["Header", null, "Header"]%>
                    </span>
                </span>
            </HeaderTemplate>
            <ItemTemplate>
                <ra:Panel 
                    style="clear:both;position:relative;"
                    runat="server">
                    <a 
                        class="question" 
                        href='<%#Eval("[URL].Value") %>'>
                        <span class="qNoAnswers">
                            <strong><%#Eval("[Answers].Value")%></strong>
                        </span>
                        <span class="qDate">
                            <%#HelperGlobals.DateFormatter.FormatDate((DateTime)Eval("[Asked].Value"))%>
                        </span>
                        <span class="qDate">
                            <%#HelperGlobals.DateFormatter.FormatDate((DateTime)Eval("[LastAnswer].Value"))%>
                        </span>
                        <span class="qHeader">
                            <%#Eval("[Header].Value") %>
                        </span>
                    </a>
                    <a href='<%#GetUsernameLink(Eval("[Username].Value")) %>' class="user">
                        <img 
                            runat="server" 
                            id="gravatar" 
                            src='<%#GetGravatar(Eval("[Email].Value")) %>'
                            style="float:left;margin-right:15px;width:32px;height:32px;overflow:hidden;" />
                        <div style="float:right;">
                            <%#Eval("[Username].Value") %>
                            <br />
                            <%#LanguageRecords.Language.Instance["Score", null, "Score"] %>: <%#Eval("[Score].Value") %>
                        </div>
                    </a>
                </ra:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </ra:Panel>
</div>