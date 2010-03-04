<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_RichEdit" %>

<ra:Label 
    runat="server" 
    ID="lbl" 
    style="display:block;padding:5px;" />
<ra:RichEdit
    ID="editor" 
    CssClass="rich-edit"
    CtrlKeys="s,g"
    OnCtrlKey="editor_CtrlKey"
    runat="server" />