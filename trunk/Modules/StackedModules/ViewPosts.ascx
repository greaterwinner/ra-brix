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
    border:solid 1px #999;
	background-color:#d6f4ff;
	position:relative;
	overflow:auto;
	display:block;
	font-weight:bold;
	-webkit-box-shadow: 3px 3px 3px #777;
	-moz-box-shadow: 3px 3px 3px #777;
	-moz-border-radius: 10px;
	-webkit-border-radius: 10px;
}

.question:visited
{
	font-weight:normal;
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
	right:35px;
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
	-webkit-box-shadow: 3px 3px 3px #777;
	-moz-box-shadow: 3px 3px 3px #777;
	-moz-border-radius: 10px;
	-webkit-border-radius: 10px;
}

.user:hover
{
	background-color:#c8e6f7;
}

.questionsHeader
{
	margin-top:-15px;
	padding:10px 25px 15px 25px;
}

.deleteQuestionBtn
{
	position:absolute;
	top:5px;
	right:5px;
	margin-top:0;
	margin-right:5px;
	background:transparent url(media/skins/Gold/Gold/sprites.png) no-repeat 0 -1248px;
	width:16px;
	height:16px;
	z-index:500;
	-webkit-box-shadow: 2px 2px 3px #777;
	-moz-box-shadow: 2px 2px 3px #777;
	-moz-border-radius: 8px;
	-webkit-border-radius: 8px;
}

.deleteQuestionBtn:hover
{
	background:transparent url(media/skins/Gold/Gold/sprites.png) no-repeat 0 -1264px;
}
</style>

<ra:Panel
    runat="server" 
    DefaultWidget="searchBtn"
    style="padding:5px;position:relative;margin-bottom:45px;">
    <h1 runat="server" id="header" />
    <ra:TextBox 
        runat="server" 
        CssClass="filter"
        AccessKey="F"
        style="opacity:0.3;top:5px;right:5px;position:absolute;"
        id="filter">
        <ra:BehaviorUnveiler 
            runat="server" />
    </ra:TextBox>
    <ra:Button 
        runat="server" 
        OnClick="searchBtn_Click"
        style="margin-left:-2000px;position:absolute;top:1px;"
        id="searchBtn" />
    <ra:Panel 
        runat="server" 
        CssClass="questions"
        id="repWrp">
        <asp:Repeater 
            runat="server" 
            ID="rep">
            <HeaderTemplate>
                <span 
                    class="question questionsHeader">
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
                    <ra:LinkButton 
                        runat="server" 
                        OnClick="DeletePost"
                        Text="&nbsp;"
                        Xtra='<%#Eval("[ID].Value")%>'
                        ToolTip='<%#LanguageRecords.Language.Instance["DeleteQuestion", null, "Delete question"] %>'
                        CssClass="deleteQuestionBtn"
                        Visible='<%#CanDelete %>' />
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
    <ra:ExtButton 
        runat="server" 
        id="ask" 
        style="position:absolute;bottom:5px;right:5px;margin-bottom:-45px;"
        OnClick="ask_Click"
        Text='<%#LanguageRecords.Language.Instance["Ask", null, "Ask..."] %>' />
</ra:Panel>

