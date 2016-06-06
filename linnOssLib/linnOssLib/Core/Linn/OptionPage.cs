using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;


namespace Linn
{
    public class EventArgsOption : EventArgs
    {
        public EventArgsOption(Option aOption)
        {
            iOption = aOption;
        }

        public Option Option
        {
            get
            {
                return iOption;
            }
        }

        private Option iOption;
    }

    public interface IOptionPage
    {
        string Name { get; }
        ReadOnlyCollection<Option> Options { get; }

        void Insert(int aIndex, Option aOption);
        void Remove(Option aOption);

        event EventHandler<EventArgsOption> EventOptionAdded;
        event EventHandler<EventArgsOption> EventOptionRemoved;
        event EventHandler<EventArgs> EventChanged;
    }

    public class OptionPage : IOptionPage
    {
        public OptionPage(string aName)
        {
            iName = aName;
            iOptions = new List<Option>();
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        public ReadOnlyCollection<Option> Options
        {
            get
            {
                List<Option> result = new List<Option>();
                lock (iOptions)
                {
                    foreach (Option o in iOptions)
                    {
                        result.Add(o);
                    }
                }
                return result.AsReadOnly();
            }
        }

        public void Insert(int aIndex, Option aOption)
        {
            lock (iOptions)
            {
                iOptions.Insert(aIndex, aOption);
            }

            aOption.EventValueChanged += ValueChanged;

            if(EventOptionAdded != null)
            {
                EventOptionAdded(this, new EventArgsOption(aOption));
            }

            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        public void Remove(Option aOption)
        {
            lock (iOptions)
            {
                iOptions.Remove(aOption);
            }

            if(EventOptionRemoved != null)
            {
                EventOptionRemoved(this, new EventArgsOption(aOption));
            }

            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgsOption> EventOptionAdded;
        public event EventHandler<EventArgsOption> EventOptionRemoved;
        public event EventHandler<EventArgs> EventChanged;

        public void Add(Option aOption)
        {
            lock (iOptions)
            {
                iOptions.Add(aOption);
            }

            aOption.EventValueChanged += ValueChanged;

            if(EventOptionAdded != null)
            {
                EventOptionAdded(this, new EventArgsOption(aOption));
            }

            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private string iName;
        private List<Option> iOptions;
    }

}

