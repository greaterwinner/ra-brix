<%@ Assembly 
    Name="ChatModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChatModules.ChatChannel" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    style="height:100%;width:100%;"
    DefaultWidget="btnSubmitNewChat"
    id="chatWrp">
    <ra:Timer 
        runat="server" 
        id="chatTimer" 
        OnTick="chatTimer_Tick" />
    <ra:Label 
        runat="server" 
        style="height:80%;display:block;overflow:auto;"
        CssClass="wholeChatAre"
        id="chatOutput" />
    <div style="width:90%;margin-left:auto;margin-right:auto;">
        <ra:TextBox 
            runat="server" 
            id="txtNewChat" 
            style="width:80%;display:block;float:left;font-size:25px;margin-top:5px;" />
        <ra:ExtButton 
            runat="server" 
            id="btnSubmitNewChat" 
            OnClick="btnSubmitNewChat_Click"
            style="width:15%;display:block;float:left;"
            Text='<%#Language.Instance["ChatChannelButton", null, "Submit"]%>' />
    </div>
</ra:Panel>


