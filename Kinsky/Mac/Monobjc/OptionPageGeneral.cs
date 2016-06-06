using Linn;

namespace KinskyDesktop
{
	internal class OptionPageGeneral : OptionPage
    {
        public OptionPageGeneral(string aName)
            : base(aName)
        {
            iOptionShowTechnicalInfo = new OptionBool("trackinfo", "Extended track info",
                                         			  "Show extended track information for the current track",
                                         			  true);
            Add(iOptionShowTechnicalInfo);
			
			iOptionShowToolTips = new OptionBool("tooltips", "Show tooltips",
                                         		 "Show tooltips for the application",
                                         		 true);
			Add(iOptionShowToolTips);

            iOptionEnableRocker = new OptionBool("rocker", "Enable rocker controls",
                                                 "Enable rocker controls for controlling volume and seeking",
                                                 false);
            Add(iOptionEnableRocker);

            iOptionPlaylistGrouping = new OptionBool("groupplaylist", "Group playlist items by album",
                                                     "Allow grouping of items within the playlist window",
                                                     true);
            Add(iOptionPlaylistGrouping);
        }
		
		public OptionBool ShowToolTips
        {
            get
            {
                return iOptionShowToolTips;
            }
        }

        public OptionBool ShowTechnicalInfo
        {
            get
            {
                return iOptionShowTechnicalInfo;
            }
        }

        public OptionBool EnableRocker
        {
            get
            {
                return iOptionEnableRocker;
            }
        }

        public OptionBool PlaylistGrouping
        {
            get
            {
                return iOptionPlaylistGrouping;
            }
        }

		private OptionBool iOptionShowToolTips;
        private OptionBool iOptionShowTechnicalInfo;
        private OptionBool iOptionEnableRocker;
        private OptionBool iOptionPlaylistGrouping;
    }
}