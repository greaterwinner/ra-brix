
using Ra.Brix.Loader;

namespace AllRaAjaxControlsModules
{
    [ActiveController]
    public class ViewAllControlsController
    {
        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["All Ra Ajax"].Value = "ViewAllRaAjaxControls";
        }

        [ActiveEvent(Name = "ViewAllRaAjaxControls")]
        protected void ViewAllRaAjaxControls(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseLoadControl(
                "AllRaAjaxControlsModules.ViewAllControls", 
                "dynMid"); 
        }
    }
}
