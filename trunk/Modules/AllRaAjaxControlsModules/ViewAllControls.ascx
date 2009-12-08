<%@ Assembly 
    Name="AllRaAjaxControlsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="AllRaAjaxControlsModules.ViewAllControls" %>


<ra:window 
    
    runat="server" 
    
    style="width:250px;" 
    
    Caption="Sample Window">
    <div style="height:100px;">
        hjhbggiuyghiuyg
    </div>
</ra:window>

<p>
    <br />
</p>

<ra:tabcontrol 
    
    runat="server">
    <ra:TabView runat="server" Caption="Tab 1">
        ijuhiuhiuhiuh
    </ra:TabView>
    <ra:TabView runat="server" Caption="Tab 2">
        ijuhiuhiuhiuh
    </ra:TabView>
    <ra:TabView runat="server" Caption="Tab 3">
        ijuhiuhiuhiuh
    </ra:TabView>
</ra:tabcontrol>

<p>
    <br />
</p>

<ra:calendar 
    
    runat="server" 
    
    style="width:170px;" />
    
<p>
    <br />
</p>
    
<ra:tree 
    
    runat="server" 
    
    style="width:250px;height:200px;overflow:auto;" 
    
    ID="tree">

    <ra:TreeNodes 
        runat="server" 
        ID="root">

        <ra:TreeNode runat="server" ID="file" Text="File">

            <ra:TreeNodes runat="server" Caption="Some files" ID="files">
                <ra:TreeNode runat="server" ID="open" Text="Open" />
                <ra:TreeNode runat="server" ID="save" Text="Save" />
                <ra:TreeNode runat="server" ID="saveAs" Text="Save as...">
                    <ra:TreeNodes runat="server" ID="saveAsDocument">
                        <ra:TreeNode runat="server" ID="pdf" Text="PDF" />
                        <ra:TreeNode runat="server" ID="odf" Text="ODF">
                            <ra:TreeNodes runat="server" ID="odfs">
                                <ra:TreeNode 
                                    runat="server" 
                                    ID="odf1" 
                                    Text="ODF1" />
                                <ra:TreeNode 
                                    runat="server" 
                                    ID="odf2" 
                                    Text="ODF2" />
                            </ra:TreeNodes>
                        </ra:TreeNode>
                    </ra:TreeNodes>
                </ra:TreeNode>
            </ra:TreeNodes>
        </ra:TreeNode>

        <ra:TreeNode runat="server" ID="edit" Text="Edit">
            <ra:TreeNodes runat="server" ID="edits">
                <ra:TreeNode runat="server" ID="copy" Text="Copy" />
                <ra:TreeNode runat="server" ID="paste" Text="Paste" />
                <ra:TreeNode runat="server" ID="cut" Text="Cut" />
            </ra:TreeNodes>
        </ra:TreeNode>

        <ra:TreeNode runat="server" ID="settings" Text="Settings" />

    </ra:TreeNodes>

</ra:tree>


<p>
 
</p>


<ra:accordion 
    
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
        <ra:Calendar 
            runat="server" 
            style="width:200px;"
            ID="cal" />
        <p>
            Here's an example of embedding controls within 
            an AccordionView.
        </p>
    </ra:AccordionView>

</ra:accordion>
<p>
 
</p>
<ra:InPlaceEdit 
    runat="server" 
    ID="edit1" 
    Text="Click this area and edit text" />
<p>
 
</p>
<ra:DateTimePicker 
    runat="server" 
    style="width:200px;" 
    ID="cal2" />
<p>
 
</p>
<ra:RichEdit
    ID="editor" 
    CssClass="editor"
    runat="server" />