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
    padding:35px 10px 15px 10px;
    border:dashed 1px #999;
	background-color:#d6f4ff;
	position:relative;
	min-height:80px;
}

.user
{
	display:block;
	position:absolute;
	margin-top:-25px;
	top:5px;
	right:20px;
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
}

.user:hover
{
	background-color:#c8e6f7;
}

.answerVote
{
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
	margin:0px 10px 10px 10px;
	float:left;
}

.answerVote .answerVotePlus,
.answerVote .answerVoteMinus
{
	background:transparent url(media/skins/Gold/Images/resource-icons.png) no-repeat 0 0;
	display:block;
	width:24px;
	height:24px;
	border:solid 1px #d0edff;
}

.answerVote .answerVotePlus:hover,
.answerVote .answerVoteMinus:hover
{
	border:solid 1px #999;
}

.answerVote .answerVotePlus
{
	background-position:-336px 0;
}

.answerVote .answerVoteMinus
{
	background-position:-336px -24px;
}

.answerVote .activePlus
{
	background-position:-360px 0;
}

.answerVote .activeMinus
{
	background-position:-360px -24px;
}

.votes
{
	display:block;
	position:absolute;
	margin-top:-25px;
	top:5px;
	left:20px;
	border:solid 1px #999;
	padding:10px;
	background-color:#d0edff;
	min-width:16px;
	text-align:center;
}

.bad
{
	border-color:#c99;
}

.neutral
{
	border-color:#999;
}

.good
{
	border-color:#9c9;
}

.votesLbl
{
	font-size:18px;
	font-weight:bold;
}

.deleteQuestionBtn
{
	position:absolute;
	top:5px;
	right:5px;
	margin-top:-13px;
	margin-right:-10px;
	background:transparent url(media/skins/Gold/Gold/sprites.png) no-repeat 0 -1248px;
	width:16px;
	height:16px;
	z-index:500;
}

.deleteQuestionBtn:hover
{
	background:transparent url(media/skins/Gold/Gold/sprites.png) no-repeat 0 -1264px;
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
                <ra:LinkButton 
                    runat="server" 
                    OnClick="DeletePost"
                    Text="&nbsp;"
                    Xtra='<%#Eval("[ID].Value")%>'
                    ToolTip='<%#LanguageRecords.Language.Instance["DeleteQuestion", null, "Delete answer"] %>'
                    CssClass="deleteQuestionBtn"
                    Visible='<%#CanDelete %>' />
                <a href='<%#GetUsernameLink(Eval("[Username].Value")) %>' class="user">
                    <img 
                        runat="server" 
                        id="gravatar" 
                        src='<%#GetGravatar(Eval("[Email].Value")) %>'
                        style="float:left;margin-right:15px;width:32px;height:32px;overflow:hidden;" />
                    <%#Eval("[Username].Value") %>
                    <br />
                    <%#LanguageRecords.Language.Instance["Score", null, "Score"] %>: <%#Eval("[Score].Value") %>
                </a>
                <div class="answerVote">
                    <ra:LinkButton 
                        runat="server" 
                        CssClass='<%#GetCssClassForVoteIcon("answerVotePlus", Eval("[CurrentVote].Value")) %>'
                        Xtra='<%#Eval("[ID].Value") %>'
                        OnClick="VoteUp"
                        id="plus" />
                    <ra:LinkButton 
                        runat="server" 
                        CssClass='<%#GetCssClassForVoteIcon("answerVoteMinus", Eval("[CurrentVote].Value")) %>'
                        Xtra='<%#Eval("[ID].Value") %>'
                        OnClick="VoteDown"
                        id="minus" />
                </div>
                <div class='<%#GetCssClassForVotes(Eval("[Votes].Value"))%>'>
                    <ra:Label 
                        runat="server" 
                        Text='<%#Eval("[Votes].Value") %>'
                        CssClass="votesLbl" />
                </div>
                <%#Eval("[Body].Value") %>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</ra:Panel>