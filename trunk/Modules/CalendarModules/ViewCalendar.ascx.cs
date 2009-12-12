/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Web.UI;
using CalendarRecords;
using HelperGlobals;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Effects;
using Ra.Widgets;
using System.Collections.Generic;
using Ra.Selector;
using Ra.Brix.Data;

namespace CalendarModules
{
    [ActiveModule]
    public class ViewCalendar : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel actWrp;
        protected global::Ra.Widgets.TextBox filter;
        protected global::Ra.Widgets.Panel pnlShowActivity;
        protected global::Ra.Widgets.Panel pnlShowActInner;
        protected global::Ra.Extensions.Widgets.InPlaceEdit txtHeader;
        protected global::Components.TextAreaEdit txtBody;
        protected global::Ra.Extensions.Widgets.DateTimePicker dateStart;
        protected global::Ra.Extensions.Widgets.DateTimePicker dateEnd;
        protected global::Ra.Widgets.LinkButton lblStart;
        protected global::Ra.Widgets.LinkButton lblEnd;
        protected global::Ra.Extensions.Widgets.ExtButton deleteBtn;

        private List<Activity> _activities;
        private static int _maxHeight;

        protected void Page_Load(object sender, EventArgs e)
        {
            BuildActivities();
            deleteBtn.DataBind();
        }

        private DateTime FirstDate
        {
            get { return ViewState["FirstDate"] == null ? GetDefaultDate() : (DateTime)ViewState["FirstDate"]; }
            set { ViewState["FirstDate"] = value; }
        }

        private DateTime GetDefaultDate()
        {
            DateTime retVal = DateTime.Now.Date;
            while(retVal.DayOfWeek != DayOfWeek.Monday)
            {
                retVal = retVal.AddDays(-1);
            }
            return retVal;
        }

        private void BuildActivities()
        {
            // Creating empty calendar
            CreateCalendar();

            // Poluating calendar with activities
            PopulateCalendarWithActivities();
        }

        private void PopulateCalendarWithActivities()
        {
            // Retrieving activites from Database...
            _activities = FillActivities();

            // Creating start, end and index values
            DateTime start = FirstDate;
            DateTime idxDate = start;
            DateTime endDate = start.AddDays(7);
            int dayNo = 0;

            // Looping through all the days we want top display
            while (idxDate < endDate)
            {
                PeriodCollection col = new PeriodCollection();
                foreach (Activity idxActivity in _activities)
                {
                    CreateOneActivityLabel(idxDate, dayNo, col, idxActivity);
                }
                idxDate = idxDate.AddDays(1);
                dayNo += 1;
            }
        }

        private void CreateOneActivityLabel(DateTime idxDate, int dayNo, PeriodCollection col, Activity idxActivity)
        {
            Period curDayPeriod = new Period(idxDate, idxDate.AddHours(23).AddMinutes(59));
            Period curActivityPeriod = new Period(idxActivity.Start, idxActivity.End);
            if (Period.Intersects(curDayPeriod, curActivityPeriod))
            {
                Label actLbl = new Label
                {
                    ID = "act" + idxActivity.ID + "x" + idxDate.DayOfYear,
                    Xtra = idxActivity.ID.ToString()
                };
                actLbl.Style[Styles.opacity] = "0.7";
                actLbl.CssClass = "activity";
                if (curActivityPeriod.End.Date != idxDate.Date)
                    actLbl.CssClass += " actOpenBottom";
                if (curActivityPeriod.Start.Date != idxDate.Date)
                    actLbl.CssClass += " actOpenTop";
                curActivityPeriod = CalculatePositionOfActivity(dayNo, col, idxActivity, curActivityPeriod, actLbl, curDayPeriod);

                // Checking to see if this actuvity is "selected"...
                if (!string.IsNullOrEmpty(CurrentActivityLabel) && 
                    actLbl.ID.IndexOf(CurrentActivityLabel.Substring(0, CurrentActivityLabel.IndexOf("x") + 1)) == 0)
                {
                    actLbl.Style[Styles.width] = "80px";
                    actLbl.Style[Styles.opacity] = "1.0";
                    actLbl.CssClass = actLbl.CssClass.Replace("activity", "activitySelected");
                    actLbl.Style[Styles.zIndex] = (int.Parse(actLbl.Style[Styles.zIndex]) + 10).ToString();
                }

                SetTextAndTooltipOfActivity(idxActivity, actLbl);

                // Trapping Click event to "bring to front"...
                actLbl.Click += delegate(object sender, EventArgs e)
                {
                    Label lb = sender as Label;
                    List<Label> lbls = FindRelatedLabels(lb);
                    if (lb != null)
                        BringActivityToFront(lbls, lb.ID.Substring(0, lb.ID.IndexOf("x") + 1).Replace("act", "").Replace("x", ""));
                };

                actWrp.Controls.Add(actLbl);
            }
            col.Add(curActivityPeriod);
        }

        private List<Label> FindRelatedLabels(Control lb)
        {
            string idOfLabels = lb.ID.Substring(0, lb.ID.IndexOf("x") + 1);
            List<Label> lbls = new List<Label>(Selector.Select<Label>(this,
                delegate(Control ctrl)
                {
                    return ctrl.ID.IndexOf(idOfLabels) == 0;
                }));
            return lbls;
        }

        private List<Activity> FillActivities()
        {
            List<Activity> activities = 
                new List<Activity>(
                    ActiveType<Activity>.Select(
                        Criteria.Mt("End", FirstDate.Date),
                        Criteria.Lt("Start", FirstDate.Date.AddDays(7))));
            activities.RemoveAll(
                delegate(Activity idx)
                {
                    return idx.Creator.Username != Users.LoggedInUserName;
                });

            // Then removing all that doesn't match the given filter (if filter is given)
            if (!string.IsNullOrEmpty(filter.Text))
            {
                string[] filterEntities = filter.Text.Split(' ');
                foreach (string idxFilter in filterEntities)
                {
                    if (idxFilter.Contains(":"))
                        continue;
                    string value = idxFilter;
                    activities.RemoveAll(
                        delegate(Activity idx)
                        {
                            return !idx.Header.ToLower().Contains(value);
                        });
                }
            }


            // Then sorting the activities
            activities.Sort(
                delegate(Activity left, Activity right)
                {
                    return left.Start.CompareTo(right.Start);
                });
            return activities;
        }

        private static void SetTextAndTooltipOfActivity(Activity idxActivity, Label actLbl)
        {
            actLbl.Tooltip = idxActivity.Header + " - " + idxActivity.Start.ToString("HH:mm") + " - " + idxActivity.End.ToString("HH:mm");
            if (actLbl.CssClass.IndexOf("actOpenTop") != -1)
                return;
            if (idxActivity.Header.Length > 20)
                actLbl.Text = idxActivity.Header.Substring(0, 17) + "...";
            else
                actLbl.Text = idxActivity.Header;
        }

        private static Period CalculatePositionOfActivity(int dayNo, IEnumerable<Period> col, Activity idxActivity, Period curActivityPeriod, RaWebControl actLbl, Period curDayPeriod)
        {
            if (_maxHeight == 0)
            {
                // Calculating max height...
                _maxHeight = (int)((((float)(DateTime.Now.Date.AddHours(23).AddMinutes(59) - DateTime.Now.Date).TotalMinutes) / 60F) * 15F) + 24;
            }
            // Current activity is intersecting with current rendering day
            int left = (dayNo * 105) + 10;
            int top;
            if (curDayPeriod.Start > idxActivity.Start)
                top = 29;
            else
                top = (int)((((float)(idxActivity.Start - curDayPeriod.Start).TotalMinutes) / 60F) * 15F) + 29;
            int height;
            if (idxActivity.Start.Date != curDayPeriod.Start)
                height = (int)((((float)(idxActivity.End - curDayPeriod.Start).TotalMinutes) / 60F) * 15F) - 6;
            else
                height = (int)((((float)(idxActivity.End - idxActivity.Start).TotalMinutes) / 60F) * 15F) - 6;
            height = Math.Min(height, _maxHeight - top);

            // Checking to see if it overlaps with a previously added activity
            int zIndex = 100;
            foreach (Period idxPreviouslyAdded in col)
            {
                if (!Period.Intersects(idxPreviouslyAdded, curActivityPeriod))
                    continue;
                zIndex += 1;
                left += 10;
            }
            actLbl.Style[Styles.left] = left + "px";
            actLbl.Style[Styles.top] = top + "px";
            actLbl.Style[Styles.height] = height + "px";
            actLbl.Style[Styles.zIndex] = zIndex.ToString();
            return curActivityPeriod;
        }

        private void BringActivityToFront(IList<Label> lbls, string id)
        {
            if (!string.IsNullOrEmpty(CurrentActivityLabel))
            {
                string idOfPreviousLabel = CurrentActivityLabel.Substring(0, CurrentActivityLabel.IndexOf("x") + 1);
                foreach (Label oldLb in Selector.Select<Label>(this,
                    delegate(Control idxCtrl)
                    {
                        return idxCtrl.ID.IndexOf(idOfPreviousLabel) == 0;
                    }))
                {
                    oldLb.Style[Styles.zIndex] = (int.Parse(oldLb.Style[Styles.zIndex]) - 10).ToString();
                    oldLb.Style[Styles.opacity] = "0.7";
                    oldLb.CssClass = oldLb.CssClass.Replace("activitySelected", "activity");
                    oldLb.Style[Styles.width] = "";
                }
                if (lbls[0].ID == CurrentActivityLabel)
                {
                    CurrentActivityLabel = null;
                    new EffectFadeOut(pnlShowActInner, 200)
                        .ChainThese(
                            new EffectSize(pnlShowActivity, 500, 0, 0)
                                .JoinThese(new EffectFadeOut()))
                        .Render();
                    return; // Short circuting to make it possible to "de-select" all labels...
                }
            }
            pnlShowActivity.Visible = true;
            pnlShowActivity.Style[Styles.display] = "none";
            pnlShowActivity.Style[Styles.width] = "0px";
            pnlShowActivity.Style[Styles.height] = "0px";
            pnlShowActivity.Style[Styles.left] = "25px";
            pnlShowActivity.Style[Styles.top] = "25px";
            new EffectFadeIn(pnlShowActInner, 200)
                .ChainThese(
                    new EffectSize(pnlShowActivity, 500, 250, 400)
                        .JoinThese(new EffectFadeIn()))
                .Render();
            Activity a = ActiveType<Activity>.SelectByID(int.Parse(id));
            txtHeader.Text = a.Header;
            txtBody.Text = string.IsNullOrEmpty(a.Body) ? "[null]" : a.Body;
            lblStart.Text = a.Start.ToString("d MMM - HH:mm");
            lblEnd.Text = a.End.ToString("d MMM - HH:mm");
            dateStart.Value = a.Start;
            dateEnd.Value = a.End;
            CurrentActivityLabel = lbls[0].ID;
            foreach (Label lb in lbls)
            {
                lb.Style[Styles.zIndex] = (int.Parse(lb.Style[Styles.zIndex]) + 10).ToString();
                lb.Style[Styles.opacity] = "1.0";
                lb.CssClass = lb.CssClass.Replace("activity", "activitySelected");
            }
            Effect root = new EffectHighlight(lbls[0], 400);
            root.JoinThese(new EffectSize(-1, 80));
            Effect idxEffect = root;
            for (int idx = 1; idx < lbls.Count; idx++)
            {
                Effect tmpEff = new EffectHighlight(lbls[idx], 400);
                tmpEff.JoinThese(new EffectSize(-1, 80));
                idxEffect.Chained.Add(tmpEff);
                idxEffect = tmpEff;
            }
            root.Render();
        }

        protected void lblStart_Click(object sender, EventArgs e)
        {
            if (dateStart.Style[Styles.display] == "none")
            {
                new EffectFadeIn(dateStart, 500)
                    .Render();
            }
        }

        protected void lblEnd_Click(object sender, EventArgs e)
        {
            if (dateEnd.Style[Styles.display] == "none")
            {
                new EffectFadeIn(dateEnd, 500)
                    .Render();
            }
        }

        protected void deleteBtn_Click(object sender, EventArgs e)
        {
            int id = int.Parse(CurrentActivityLabel.Substring(3).Split('x')[0]);
            Activity a = ActiveType<Activity>.SelectByID(id);
            a.Delete();
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
            new EffectFadeOut(pnlShowActInner, 200)
                .ChainThese(
                    new EffectSize(pnlShowActivity, 500, 0, 0)
                        .JoinThese(new EffectFadeOut()))
                .Render();
        }

        protected void closeAct_Click(object sender, EventArgs e)
        {
            new EffectFadeOut(pnlShowActInner, 200)
                .ChainThese(
                    new EffectSize(pnlShowActivity, 500, 0, 0)
                        .JoinThese(new EffectFadeOut()))
                .Render();
            string idOfPreviousLabel = CurrentActivityLabel.Substring(0, CurrentActivityLabel.IndexOf("x") + 1);
            foreach (Label oldLb in Selector.Select<Label>(this,
                delegate(Control idxCtrl)
                {
                    return idxCtrl.ID.IndexOf(idOfPreviousLabel) == 0;
                }))
            {
                oldLb.Style[Styles.zIndex] = (int.Parse(oldLb.Style[Styles.zIndex]) - 10).ToString();
                oldLb.Style[Styles.opacity] = "0.7";
                oldLb.CssClass = oldLb.CssClass.Replace("activitySelected", "activity");
                oldLb.Style[Styles.width] = "";
            }
            CurrentActivityLabel = "";
        }

        protected void dateStart_DateClicked(object sender, EventArgs e)
        {
            int id = int.Parse(CurrentActivityLabel.Substring(3).Split('x')[0]);
            Activity a = ActiveType<Activity>.SelectByID(id);
            a.Start = dateStart.Value;
            a.Save();
            lblStart.Text = a.Start.ToString("d MMM - HH:mm");
            new EffectFadeOut(dateStart, 500)
                .Render();
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
        }

        protected void dateEnd_DateClicked(object sender, EventArgs e)
        {
            int id = int.Parse(CurrentActivityLabel.Substring(3).Split('x')[0]);
            Activity a = ActiveType<Activity>.SelectByID(id);
            a.End = dateEnd.Value;
            a.Save();
            lblEnd.Text = a.End.ToString("d MMM - HH:mm");
            new EffectFadeOut(dateEnd, 500)
                .Render();
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
        }

        protected void txtHeader_TextChanged(object sender, EventArgs e)
        {
            int id = int.Parse(CurrentActivityLabel.Substring(3).Split('x')[0]);
            Activity a = ActiveType<Activity>.SelectByID(id);
            a.Header = txtHeader.Text;
            a.Save();
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
        }

        protected void txtBody_TextChanged(object sender, EventArgs e)
        {
            int id = int.Parse(CurrentActivityLabel.Substring(3).Split('x')[0]);
            Activity a = ActiveType<Activity>.SelectByID(id);
            a.Body = string.IsNullOrEmpty(txtBody.Text) ? "" : txtBody.Text;
            a.Save();
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
        }

        private string CurrentActivityLabel
        {
            get { return ViewState["CurrentActivityLabel"] == null ? "" : ViewState["CurrentActivityLabel"].ToString(); }
            set { ViewState["CurrentActivityLabel"] = value; }
        }

        private void CreateCalendar()
        {
            DateTime start = FirstDate;
            DateTime idxDate = start;
            DateTime endDate = start.AddDays(7);
            int dayNo = 0;

            // Looping through all the days we want top display
            while (idxDate < endDate)
            {
                // Creating "one day panel"
                Panel day = new Panel {ID = "day" + idxDate.DayOfYear, CssClass = "day"};
                day.Style[Styles.left] = ((dayNo * 105) + 5) + "px";

                // Creating header of day
                Label header = new Label
                {
                    ID = "header" + idxDate.ToString("dd.MM.yy"),
                    Text = idxDate.ToString("ddd d.MMM"),
                    CssClass = "header"
                };
                if (idxDate.DayOfWeek == DayOfWeek.Saturday || idxDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    header.CssClass += " weekEnd";
                }
                day.Controls.Add(header);

                // Creating hours of day
                for (int idxHour = 0; idxHour < 24; idxHour++)
                {
                    Label hour = new Label
                    {
                        ID = day.ID + idxHour,
                        Xtra = idxDate.Date.ToString("yyyy:MM:dd:") + idxHour,
                        CssClass = "hour"
                    };
                    hour.Click += hour_Click;
                    if (idxHour >= 8 && idxHour <= 15)
                        hour.CssClass += " work";
                    if (idxDate.AddHours(idxHour) == DateTime.Now.Date.AddHours(DateTime.Now.Hour))
                    {
                        hour.CssClass += " now";
                        hour.Tooltip = "This is now";
                    }
                    hour.Text = idxHour.ToString("00");
                    day.Controls.Add(hour);
                }

                // Adding day to activities wrapper Panel
                actWrp.Controls.Add(day);

                // Incrementing counters
                idxDate = idxDate.AddDays(1);
                dayNo += 1;
            }
        }

        void hour_Click(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            if (lb == null)
            {
                return;
            }
            string[] when = lb.Xtra.Split(':');
            int whenYear = int.Parse(when[0]);
            int whenMonth = int.Parse(when[1]);
            int whenDay = int.Parse(when[2]);
            int whenHour = int.Parse(when[3]);
            DateTime dt = new DateTime(whenYear, whenMonth, whenDay, whenHour, 0, 0);
            Node node = new Node();
            node["Date"].Value = dt;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UserRequestsToCreateActivity",
                node);

            // Refreshing activities...
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400).Render();
        }

        protected void previous_Click(object sender, EventArgs e)
        {
            FirstDate = FirstDate.AddDays(-7);
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400).Render();
        }

        protected void next_Click(object sender, EventArgs e)
        {
            FirstDate = FirstDate.AddDays(7);
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400).Render();
        }

        protected void filter_KeyUp(object sender, EventArgs e)
        {
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400)
                .Render();
            filter.Focus();
            filter.Select();
        }

        [ActiveEvent(Name = "RefreshActivities")]
        protected void RefreshActivities(object sender, EventArgs e)
        {
            actWrp.Controls.Clear();
            BuildActivities();
            actWrp.ReRender();
            new EffectHighlight(actWrp, 400).Render();
        }

        public void InitialLoading(Node node)
        {
        }

        public string GetCaption()
        {
            return "";
        }
    }
}