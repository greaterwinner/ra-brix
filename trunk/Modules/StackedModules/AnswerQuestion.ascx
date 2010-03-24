<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.AnswerQuestion" %>

<ra:Panel 
    runat="server" 
    style="float:right;"
    id="answerBtnWrp">
    <ra:ExtButton 
        runat="server" 
        id="showAnswer" 
        AccessKey="A"
        OnClick="showAnswer_Click"
        Text='<%#LanguageRecords.Language.Instance["AnswerDotDotDot", null, "Answer..."] %>' />
</ra:Panel>

<ra:Panel 
    runat="server" 
    style="clear:both;"
    Visible="false"
    id="allWrp">

    <ra:RichEdit
        ID="editor" 
        CssClass="editor"
        OnGetImageDialog="editor_GetImageDialog"
        OnGetResourceDialog="editor_GetResourceDialog"
        OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
        CtrlKeys="s,g"
        OnCtrlKey="editor_CtrlKeys"
        OnGetExtraToolbarControls="editor_GetExtraToolbarControls"
        runat="server" />
</ra:Panel>
