<%@ Assembly 
    Name="ForumModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ForumModules.ShowPostsFromUser" %>

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
        Text="Count"
        style="position:absolute;bottom:5px;text-align:center;width:100%;font-style:italic;color:#999;"
        id="count" />

</ra:Tree>
