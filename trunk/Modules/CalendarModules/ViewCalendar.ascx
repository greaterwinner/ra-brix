<%@ Assembly 
    Name="CalendarModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CalendarModules.ViewCalendar" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:5px;height:100%;position:relative;">
    <ra:Window 
        runat="server" 
        CssClass="light-window ShowActivity"
        Visible="false"
        style="left:25px;top:25px;width:400px;"
        id="pnlShowActivity">
        <ra:Panel 
            runat="server" 
            id="pnlShowActInner">
            <div>
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
                <%=Language.Instance["Start", null, "Start: "] %><ra:LinkButton 
                    runat="server" 
                    OnClick="lblStart_Click"
                    id="lblStart" />
                <ra:InPlaceEdit 
                    runat="server" 
                    Tag="span"
                    OnTextChanged="inplStart_TextChanged"
                    style="color:Blue;cursor:pointer;"
                    id="inplStart" />
                <ra:Calendar 
                    runat="server" 
                    OnDateClicked="dateStart_DateClicked"
                    style="display:none;position:absolute;z-index:1000;"
                    id="dateStart" />
                <br />
                <%=Language.Instance["End", null, "End: "] %><ra:LinkButton 
                    runat="server" 
                    OnClick="lblEnd_Click"
                    id="lblEnd" />
                <ra:InPlaceEdit 
                    runat="server" 
                    Tag="span"
                    OnTextChanged="inplEnd_TextChanged"
                    style="color:Blue;cursor:pointer;"
                    id="inplEnd" />
                <ra:Calendar 
                    runat="server" 
                    OnDateClicked="dateEnd_DateClicked"
                    style="display:none;position:absolute;z-index:1000;"
                    id="dateEnd" />
                <br />
                <div style="padding:5px;">
                    <ra:CheckBox 
                        runat="server" 
                        id="repeat" 
                        OnCheckedChanged="repeat_CheckedChanged"
                        Text='<%#Language.Instance["Repeat", null, "Repeat"] %>' />
                    <ra:Panel 
                        runat="server" 
                        style="border:solid 1px #999;padding:5px;margin:5px;"
                        Visible="false"
                        id="repeatPnl">
                        <ra:RadioButton 
                            runat="server" 
                            OnCheckedChanged="rdoRepeat_CheckedChanged"
                            GroupName="repetition"
                            Text='<%#Language.Instance["EveryWeek", null, "Every week"] %>'
                            id="rdoWeek" />
                        <br />
                        <ra:RadioButton 
                            runat="server" 
                            OnCheckedChanged="rdoRepeat_CheckedChanged"
                            GroupName="repetition"
                            Text='<%#Language.Instance["EveryMonth", null, "Every month"] %>'
                            id="rdoMonth" />
                        <br />
                        <ra:RadioButton 
                            runat="server" 
                            OnCheckedChanged="rdoRepeat_CheckedChanged"
                            GroupName="repetition"
                            Text='<%#Language.Instance["EveryYear", null, "Every year"] %>'
                            id="rdoYear" />
                        <br />
                    </ra:Panel>
                </div>
                <div style="position:absolute;bottom:15px;right:15px;">
                    <ra:ExtButton 
                        runat="server" 
                        id="deleteBtn" 
                        OnClick="deleteBtn_Click"
                        Text='<%#Language.Instance["Delete", null, "Delete"] %>' />
                    <ra:ExtButton 
                        runat="server" 
                        Text='<%#Language.Instance["OK",null,"OK"] %>'
                        OnClick="closeAct_Click"
                        id="closeAct" />
                </div>
            </div>
        </ra:Panel>
        <ra:BehaviorUnveiler
            runat="server"
            MinOpacity="0.5"
            id="unveilAct" />
    </ra:Window>
    <ra:Panel 
        runat="server" 
        DefaultWidget="btnFilter"
        id="pnlFilterWrp">
        <div>
            <ra:TextBox 
                runat="server" 
                Tooltip="Filter activities here"
                AccessKey="F"
                style="opacity:0.3;"
                CssClass="filterTextBox activityFilter"
                id="filter">
                <ra:BehaviorUnveiler 
                    runat="server" 
                    id="filterUnveiler" />
            </ra:TextBox>
            <ra:Button 
                runat="server" 
                id="btnFilter" 
                OnClick="filter_KeyUp"
                style="position:absolute;left:1px;margin-left:-200px;" />
        </div>
        <div class="grid">
            <ra:LinkButton 
                runat="server" 
                id="previous" 
                AccessKey="P"
                style="opacity:0.3;"
                OnClick="previous_Click"
                CssClass="previous"
                Text="&lt;&lt;">
                <ra:BehaviorUnveiler 
                    runat="server" 
                    id="unveil1" />
            </ra:LinkButton>
            <ra:LinkButton 
                runat="server" 
                id="next" 
                AccessKey="N"
                style="opacity:0.3;"
                OnClick="next_Click"
                CssClass="next"
                Text="&gt;&gt;">
                <ra:BehaviorUnveiler 
                    runat="server" 
                    id="unveil2" />
            </ra:LinkButton>
        </div>
    </ra:Panel>
    <ra:Panel 
        runat="server" 
        id="actWrp" 
        CssClass="days" />
</div>







