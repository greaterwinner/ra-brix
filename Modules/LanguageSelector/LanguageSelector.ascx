<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LanguageSelectorModules.LanguageSelector" %>
<%@ Import Namespace="LanguageRecords"%>

<div 
    runat="server" 
    style="padding:10px;"
    id="buttons">
    <ra:LinkButton 
        runat="server"
        ID="Arabian"
        CssClass="flags-arabian"
        Text="&nbsp;" 
        Xtra="ar-sa"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagArabianTooltip", null, "Arabic"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler1"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="Norwegian"
        CssClass="flags-norwegian"
        Text="&nbsp;" 
        Xtra="nb"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagNorwegianTooltip", null, "Norwegian"]%>'
        OnClick="SelectLanguage"
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler2"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="English"
        CssClass="flags-britain"
        Text="&nbsp;" 
        Xtra="en"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagEnglishTooltip", null, "English"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler3"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="German"
        CssClass="flags-german"
        Text="&nbsp;" 
        Xtra="de"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagGermanTooltip", null, "German"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler4"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="French"
        CssClass="flags-france"
        Text="&nbsp;" 
        Xtra="fr"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagFrenchTooltip", null, "French"]%>'
        OnClick="SelectLanguage"
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler5"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="Spanish"
        CssClass="flags-spain"
        Text="&nbsp;" 
        Xtra="es"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagSpanishTooltip", null, "Spanish"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler6"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="Swedish"
        CssClass="flags-sweden"
        Text="&nbsp;" 
        Xtra="se"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagSwedishTooltip", null, "Swedish"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler7"
            runat="server" />
    </ra:LinkButton>

    <ra:LinkButton 
        runat="server"
        ID="USA"
        CssClass="flags-USA"
        Text="&nbsp;" 
        Xtra="en-us"
        OnClick="SelectLanguage"
        Tooltip='<%#Language.Instance["LanguageSelectorFlagAmericanTooltip", null, "American"]%>'
        style="opacity:0.3;">
        <ra:BehaviorUnveiler 
            id="unveiler8"
            runat="server" />
    </ra:LinkButton>
</div>
