using System.Threading;
using System.Collections.Generic;
using System;

namespace Linn
{
    public interface IUserLogListener
    {
        void Write(string aMessage);
        void WriteLine(string aMessage);
    }

    public class UserLog
    {
        public static void Write(string aMessage)
        {
            lock (iLock)
            {
                foreach (IUserLogListener listener in iListenerList)
                {
                    try
                    {
                        listener.Write(aMessage);
                    }
                    catch (Exception) { }
                }

                iLogText += aMessage;
                ClipLogText();
            }
        }

        public static void WriteLine(string aMessage)
        {
            lock (iLock)
            {
                foreach (IUserLogListener listener in iListenerList)
                {
                    try
                    {
                        listener.WriteLine(aMessage);
                    }
                    catch (Exception) { }
                }

                iLogText += aMessage + Environment.NewLine;
                ClipLogText();
            }
        }

        public static void AddListener(IUserLogListener aListener)
        {
            lock (iLock)
            {
                iListenerList.Add(aListener);
            }
        }

        public static void RemoveListener(IUserLogListener aListener)
        {
            lock (iLock)
            {
                iListenerList.Remove(aListener);
            }
        }

        public static string Text
        {
            get
            {
                lock (iLock)
                {
                    return iLogText;
                }
            }
        }

        private static void ClipLogText()
        {
            if (iLogText.Length >= kMaxTextLength)
            {
                iLogText = iLogText.Remove(0, iLogText.Length - kMaxTextLength);
            }
        }

        private static readonly int kMaxTextLength = 32767;

        private static string iLogText = "";
        private static object iLock = new object();
        private static List<IUserLogListener> iListenerList = new List<IUserLogListener>();
    }
} // Linn