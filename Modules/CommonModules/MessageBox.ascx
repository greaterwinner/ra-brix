<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CommonModules.MessageBox" %>

<div style="position:relative;height:100%;">
    <div style="padding:5px;">
        <ra:Label 
            runat="server" 
            id="txt" />
        <ra:ExtButton 
            runat="server" 
            id="submit" 
            style="position:absolute;bottom:5px;right:5px;"
            Text='<%#LanguageRecords.Language.Instance["OK", null, "OK"] %>' 
            OnClick="submit_Click" />
    </div>
</div>

