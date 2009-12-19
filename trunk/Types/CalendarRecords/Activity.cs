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
using Ra.Brix.Data;
using UserRecords;

namespace CalendarRecords
{
    [ActiveType]
    public class Activity : ActiveType<Activity>, ICloneable
    {
        public enum RepetitionPattern
        {
            None,
            Weekly,
            Monthly,
            Yearly
        };

        public Activity()
        {
            Repetition = RepetitionPattern.None;
        }

        public RepetitionPattern Repetition
        {
            get
            {
                return (RepetitionPattern)Enum.Parse(typeof(RepetitionPattern), RepPatterImpl);
            }
            set
            {
                RepPatterImpl = value.ToString();
            }
        }

        [ActiveField]
        public string RepPatterImpl { get; set; }

        [ActiveField(IsOwner = false)]
        public User Creator { get; set; }

        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime Start { get; set; }

        [ActiveField]
        public DateTime End { get; set; }

        [ActiveField]
        private string BundleID { get; set; }

        private void SaveImpl()
        {
            base.Save();
        }

        public override void Save()
        {
            bool hasError = false;
            if (Start >= End)
            {
                End = Start.AddMinutes(1);
                hasError = true;
            }
            if (ID == 0)
            {
                // Initial save, need to create a BundleID for repetition pattern...
                BundleID = Guid.NewGuid().ToString();
            }
            else
            {
                // Need to delete previous BundleID records...
                foreach (Activity idx in Select(Criteria.Eq("BundleID", BundleID), Criteria.Mt("Start", Start.AddMinutes(-1))))
                {
                    if (idx.ID != ID)
                        idx.DeleteImpl();
                }
            }
            if (Repetition == RepetitionPattern.Weekly)
            {
                SaveImpl();
                DateTime idx = Start;
                while (idx - Start < new TimeSpan(365, 0, 0, 0))
                {
                    idx = idx.AddDays(7);
                    Activity n = ((ICloneable)this).Clone() as Activity;
                    n.Start += (idx - Start);
                    n.End += (idx - Start);
                    n.SaveImpl();
                }
            }
            if (Repetition == RepetitionPattern.Monthly)
            {
                SaveImpl();
                DateTime idx = Start;
                while (idx - Start < new TimeSpan(365, 0, 0, 0))
                {
                    idx = idx.AddMonths(1);
                    Activity n = ((ICloneable)this).Clone() as Activity;
                    n.Start += (idx - Start);
                    n.End += (idx - Start);
                    n.SaveImpl();
                }
            }
            if (Repetition == RepetitionPattern.Yearly)
            {
                SaveImpl();
                DateTime idx = Start;
                while (idx - Start < new TimeSpan(3650, 0, 0, 0))
                {
                    idx = idx.AddYears(1);
                    Activity n = ((ICloneable)this).Clone() as Activity;
                    n.Start += (idx - Start);
                    n.End += (idx - Start);
                    n.SaveImpl();
                }
            }
            else
            {
                SaveImpl();
            }
            if (hasError)
            {
                throw new ArgumentException(
                    "Cannot have the start date be equal or higher than the end date of the activity");
            }
        }

        public override void Delete()
        {
            // Need to delete previous BundleID records...
            foreach (Activity idx in Select(Criteria.Eq("BundleID", BundleID), Criteria.Mt("Start", Start.AddMinutes(-1))))
            {
                idx.DeleteImpl();
            }
        }

        private void DeleteImpl()
        {
            base.Delete();
        }

        object ICloneable.Clone()
        {
            Activity retVal = new Activity();
            retVal.Body = Body;
            retVal.BundleID = BundleID;
            retVal.Creator = Creator;
            retVal.End = End;
            retVal.Header = Header;
            retVal.Repetition = Repetition;
            retVal.Start = Start;
            return retVal;
        }
    }
}