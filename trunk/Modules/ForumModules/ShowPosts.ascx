<%@ Assembly 
    Name="ForumModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ForumModules.ShowPosts" %>

<div style="position:relative;">
    <ra:Panel 
        runat="server" 
        CssClass="forumPost"
        id="pnlWrp">
        <strong><%#LanguageRecords.Language.Instance["HeaderOfComment", null, "Header of Comment"] %></strong>
        <br />
        <ra:TextBox 
            runat="server" 
            CssClass="forumHeaderTxt"
            Text='<%#LanguageRecords.Language.Instance["HeaderOfComment", null, "Header of Comment"] %>'
            id="header" />
        <strong><%#LanguageRecords.Language.Instance["BodyOfComment", null, "Comment"] %></strong>
        <br />
        <ra:TextArea
            runat="server"
            CssClass="forumBodyTxt"
            id="body" />
        <br />
        <strong><%#LanguageRecords.Language.Instance["NameLogInToShowReal", null, "Name - login to show verified username"] %></strong>
        <br />
        <ra:TextBox 
            runat="server" 
            CssClass="forumHeaderTxt"
            Text="Anonymous Coward"
            id="anonTxt" />
        <ra:Label 
            runat="server" 
            style="font-style:italic;"
            id="lblLoggedInUsername" />
        <br />
        <ra:ExtButton 
            runat="server" 
            CssClass="button forumSubmitBtn"
            id="submit" 
            OnClick="submit_Click"
            Text='<%#LanguageRecords.Language.Instance["Submit", null, "Submit"] %>' />
    </ra:Panel>

    <ra:Tree 
        runat="server" 
        UseRichAnimations="true"
        CssClass="tree no-lines"
        Expansion="SingleClickPlusSign"
        style="position:relative;"
        OnSelectedNodeChanged="menu_SelectedNodeChanged"
        ID="tree">

        <ra:TreeNodes 
            runat="server" 
            style="margin-bottom:25px;"
            ID="root" />
        <ra:Label 
            runat="server" 
            Tag="div"
            style="position:absolute;bottom:5px;text-align:center;width:100%;font-style:italic;color:#999;"
            id="count" />

    </ra:Tree>

    <ra:Window 
        runat="server" 
        Visible="false"
        style="z-index:1000;position:fixed;top:150px;left:150px;width:450px;"
        id="replyWnd">
        <div style="padding:15px 0 35px 0;position:relative;margin-left:auto;margin-right:auto;">
            <strong><%#LanguageRecords.Language.Instance["HeaderOfComment", null, "Header of Comment"] %></strong>
            <br />
            <ra:TextBox 
                runat="server" 
                style="width:100%;"
                OnEscPressed="headerReply_EscPressed"
                Text='<%#LanguageRecords.Language.Instance["HeaderOfComment", null, "Header of Comment"] %>'
                id="headerReply" />
            <br />
            <strong><%#LanguageRecords.Language.Instance["BodyOfComment", null, "Comment"] %></strong>
            <br />
            <ra:TextArea
                runat="server"
                style="width:100%;height:200px;"
                id="bodyReply" />
            <br />
            <ra:ExtButton 
                runat="server" 
                style="position:absolute;right:5px;bottom:5px;"
                id="submitComment" 
                OnClick="submitComment_Click"
                Text='<%#LanguageRecords.Language.Instance["Submit", null, "Submit"] %>' />
        </div>
        <ra:BehaviorObscurable 
            runat="server" 
            id="obs" />
    </ra:Window>
</div>


