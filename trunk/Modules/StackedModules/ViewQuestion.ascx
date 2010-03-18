<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.ViewQuestion" %>

<style type="text/css">
.userBig
{
	border:solid 1px #999;
	padding:5px;
	background-color:#d0edff;
	float:right;
	display:block;
	margin-left:15px;
}

.userBig:hover
{
	background-color:#c8e6f7;
}
</style>

<div style="padding:5px;position:relative;" class="forumMainBody">
    <a href='<%=GetUsernameLink() %>' class="userBig">
        <img 
            src='<%=GetGravatar() %>'
            style="float:left;margin-right:15px;width:48px;height:48px;overflow:hidden;" />
        <%=Username %>
        <br />
        <%=LanguageRecords.Language.Instance["Score", null, "Score"] %>: <%=Score %>
    </a>
    <h1 runat="server" id="header" />
    <div runat="server" id="body" />
</div>