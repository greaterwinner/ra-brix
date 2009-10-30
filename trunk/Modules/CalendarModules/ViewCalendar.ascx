﻿<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CalendarModules.ViewCalendar" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="padding:5px;height:100%;position:relative;">
    <ra:Panel 
        runat="server" 
        CssClass="ShowActivity"
        style="opacity:0.5;"
        Visible="false"
        id="pnlShowActivity">
        <ra:Panel 
            runat="server" 
            id="pnlShowActInner">
            <ra:LinkButton 
                runat="server" 
                Text="X"
                style="position:absolute;top:5px;right:5px;"
                OnClick="closeAct_Click"
                id="closeAct" />
            <div style="padding:25px;">
                <ra:InPlaceEdit 
                    runat="server" 
                    CssClass="edit"
                    OnTextChanged="txtHeader_TextChanged"
                    id="txtHeader" />
                 <br />
                <ext:TextAreaEdit 
                    runat="server" 
                    CssClass="edit"
                    OnTextChanged="txtBody_TextChanged"
                    id="txtBody" />
                <br />
                <ra:LinkButton 
                    runat="server" 
                    OnClick="lblStart_Click"
                    id="lblStart" />
                <ra:DateTimePicker 
                    runat="server" 
                    OnDateClicked="dateStart_DateClicked"
                    style="display:none;position:absolute;z-index:1000;"
                    id="dateStart" />
                <br />
                <ra:LinkButton 
                    runat="server" 
                    OnClick="lblEnd_Click"
                    id="lblEnd" />
                <ra:DateTimePicker 
                    runat="server" 
                    OnDateClicked="dateEnd_DateClicked"
                    style="display:none;position:absolute;z-index:1000;"
                    id="dateEnd" />
                <ra:ExtButton 
                    runat="server" 
                    id="deleteBtn" 
                    OnClick="deleteBtn_Click"
                    style="position:absolute;bottom:5px;right:5px;"
                    Text='<%#Language.Instance["DeleteActivity", null, "Delete Activity"] %>' />
            </div>
            <ra:BehaviorUnveiler
                runat="server"
                MinOpacity="0.5"
                id="unveilAct" />
        </ra:Panel>
    </ra:Panel>
    <ra:TextBox 
        runat="server" 
        OnKeyUp="filter_KeyUp"
        Tooltip="Filter activities here"
        AccessKey="F"
        style="opacity:0.3;"
        CssClass="filterTextBox activityFilter"
        id="filter">
        <ra:BehaviorUnveiler 
            runat="server" 
            id="filterUnveiler" />
    </ra:TextBox>
    <ra:LinkButton 
        runat="server" 
        id="previous" 
        AccessKey="P"
        style="opacity:0.3;"
        OnClick="previous_Click"
        CssClass="previous-activities"
        Text="&nbsp;">
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unveil1" />
    </ra:LinkButton>
    <ra:Panel 
        runat="server" 
        id="actWrp" 
        CssClass="days" />
    <ra:LinkButton 
        runat="server" 
        id="next" 
        AccessKey="N"
        style="opacity:0.3;"
        OnClick="next_Click"
        CssClass="next-activities"
        Text="&nbsp;">
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unveil2" />
    </ra:LinkButton>
</div>







