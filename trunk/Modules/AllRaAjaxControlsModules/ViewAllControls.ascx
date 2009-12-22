<%@ Assembly 
    Name="AllRaAjaxControlsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="AllRaAjaxControlsModules.ViewAllControls" %>


<ra:Window 
    runat="server" 
    style="width:250px;" 
    id="window"
    Caption="Sample Window">
    <div style="height:100px;">
        hjhbggiuyghiuyg
    </div>
</ra:Window>

<br />

<ra:TabControl 
    id="tab"
    runat="server">
    <ra:TabView 
        runat="server" 
        id="tabView1"
        Caption="Tab 1">
        This is the first tab...
    </ra:TabView>
    <ra:TabView 
        runat="server" 
        id="tabView2"
        Enabled="false" 
        Caption="Tab 2">
        Another tab :)
    </ra:TabView>
    <ra:TabView 
        runat="server" 
        id="TabView3"
        Caption="Tab 3">
        <ra:Calendar 
            runat="server" 
            id="cal3"
            style="width:170px;" />
    </ra:TabView>
</ra:TabControl>

<br />

<ra:Calendar 
    runat="server" 
    id="cal"
    style="width:170px;" />
    
<br />
    
<ra:Accordion 
    runat="server" 
    ID="acc">
    <ra:AccordionView 
        runat="server" 
        ID="acc1" 
        Caption="First accordion">
        Nullam a tellus sapien. Ut 
        dignissim, risus sit amet vestibulum gravida, mauris odio 
        malesuada nisl, eget fermentum ligula leo at nunc. Cras 
        a justo est, condimentum adipiscing metus. Quisque eu arcu 
        felis, sit amet auctor sapien. 
    </ra:AccordionView>

    <ra:AccordionView 
        runat="server" 
        ID="acc2" 
        Caption="Second accordion">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
        Etiam eu metus nisi, et venenatis eros. Nunc imperdiet 
        nulla ut diam sagittis congue. Pellentesque vel dui est, 
        sit amet imperdiet sapien.  
    </ra:AccordionView>

    <ra:AccordionView 
        runat="server" 
        ID="acc3" 
        Caption="Third accordion">
        <p>
            Here's an example of embedding controls within 
            an AccordionView.
        </p>
    </ra:AccordionView>

</ra:accordion>

<br />

<ra:InPlaceEdit 
    runat="server" 
    ID="edit1" 
    Text="Click this area and edit text" />

<br />

<ra:DateTimePicker 
    runat="server" 
    style="width:200px;" 
    ID="cal2" />

<br />

<ra:RichEdit
    ID="editor" 
    CssClass="editor"
    Text="howdy"
    runat="server" />


