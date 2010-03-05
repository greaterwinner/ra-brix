<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_WebMethod" %>

<script type="text/javascript">
/* Will contain the ID of the UserControl the ServerMethod is within */
window.userControlName = '';
window.foo = function() {
  Ra.Control.callServerMethod(window.userControlName + '.foo', {
    onSuccess: function(retVal) {
      alert(retVal);
    },
    onError: function(status, fullTrace) {
      alert(fullTrace);
    }
  }, [Ra.$('txt').value]);
}

</script>

<input 
    type="text" 
    id="txt" 
    value="John Doe" />

<input 
    type="button" 
    value="Go server side..." 
    onclick="window.foo();" />