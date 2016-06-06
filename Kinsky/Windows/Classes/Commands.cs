using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace KinskyDesktopWpf
{
    public static class Commands
    {

        public static RoutedUICommand KompactCommand = new RoutedUICommand("Kompact Mode",
                                                               "Kompact",
                                                               typeof(Commands));

        public static RoutedUICommand RescanCommand = new RoutedUICommand("Rescan Network",
                                                               "Rescan",
                                                               typeof(Commands), 
                                                               new InputGestureCollection((new InputGesture[] {new KeyGesture(Key.F5)}).ToList()));

        public static RoutedUICommand OptionsCommand = new RoutedUICommand("Options...",
                                                               "Options",
                                                               typeof(Commands));

        public static RoutedUICommand DebugConsoleCommand = new RoutedUICommand("Debug Console",
                                                               "DebugConsole",
                                                               typeof(Commands));

        public static RoutedUICommand AboutCommand = new RoutedUICommand("About Kinsky",
                                                               "About",
                                                               typeof(Commands));

        public static RoutedUICommand OpenCommand = new RoutedUICommand("Open",
                                                               "Open",
                                                               typeof(Commands));

        public static RoutedUICommand PlayNowCommand = new RoutedUICommand("Play Now",
                                                               "PlayNow",
                                                               typeof(Commands));

        public static RoutedUICommand PlayNextCommand = new RoutedUICommand("Play Next",
                                                               "PlayNext",
                                                               typeof(Commands));

        public static RoutedUICommand PlayLaterCommand = new RoutedUICommand("Play Later",
                                                               "PlayLater",
                                                               typeof(Commands));

        public static RoutedUICommand DetailsCommand = new RoutedUICommand("Details",
                                                               "Details",
                                                               typeof(Commands));

        public static RoutedUICommand MoveUpCommand = new RoutedUICommand("Move Up",
                                                               "MoveUp",
                                                               typeof(Commands));

        public static RoutedUICommand MoveDownCommand = new RoutedUICommand("Move Down",
                                                               "MoveDown",
                                                               typeof(Commands));

        public static RoutedUICommand SaveCommand = new RoutedUICommand("Save",
                                                               "Save",
                                                               typeof(Commands));

        public static RoutedUICommand DeleteCommand = new RoutedUICommand("Delete",
                                                               "Delete",
                                                               typeof(Commands));

        public static RoutedUICommand OptionsPageResetCommand = new RoutedUICommand("Reset",
                                                               "OptionsPageReset",
                                                               typeof(Commands));

        public static RoutedUICommand ScrollToCurrentCommand = new RoutedUICommand("Scroll to now playing",
                                                               "ScrollToCurrent",
                                                               typeof(Commands));

        public static RoutedUICommand RenameCommand = new RoutedUICommand("Rename",
                                                               "Rename",
                                                               typeof(Commands));

        public static RoutedUICommand UpdateCheckCommand = new RoutedUICommand("Check for updates",
                                                               "UpdateCheck",
                                                               typeof(Commands));

        public static RoutedUICommand BookmarkCommand = new RoutedUICommand("Bookmark",
                                                               "Bookmark",
                                                               typeof(Commands));
    }
}