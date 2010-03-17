<%@ Assembly 
    Name="StackedModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="StackedModules.AskQuestion" %>

<div style="padding:5px;">
    <ra:InPlaceEdit 
        runat="server" 
        id="header" 
        Tag="h3"
        CssClass="edit"
        Text='<%#LanguageRecords.Language.Instance["HeaderOfQuestionText", null, "Header of question"] %>' />
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
</div>




