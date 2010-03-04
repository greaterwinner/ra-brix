<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_Label" %>

<ra:Label 
    runat="server" 
    ID="lbl1" 
    OnMouseOver="MouseOver"
    Tag="div"
    style="padding:15px;border:dashed 2px #999;background-color:#eee;"
    OnMouseOut="MouseOut"
    Text="Hover over me" />

