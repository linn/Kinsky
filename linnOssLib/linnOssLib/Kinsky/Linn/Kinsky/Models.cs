using Linn.Topology;
using System;
using System.Collections.Generic;
using System.Net;
using Upnp;
using System.Collections.ObjectModel;
using Linn.ControlPoint;

namespace Linn.Kinsky
{
    /// <summary>
    /// Generic event args for item update/delete notifications.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgsItem<T> : EventArgs
    {
        /// <summary>
        /// Constructor for EventArgsItem
        /// </summary>
        /// <param name="aItem">The item which has been updated/deleted.</param>
        public EventArgsItem(T aItem)
        {
            Item = aItem;
        }
        /// <summary>
        /// The item which has been updated/deleted.
        /// </summary>
        public T Item { get; set; }
    }
    /// <summary>
    /// Generic event args for ordered item insertion in lists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgsItemInsert<T> : EventArgs
    {
        /// <summary>
        /// Constructor for EventArgsItemInsert.
        /// </summary>
        /// <param name="aIndex">The index within a list that the new item has been inserted at.</param>
        /// <param name="aItem">The new item which has been inserted.</param>
        public EventArgsItemInsert(int aIndex, T aItem)
        {
            Index = aIndex;
            Item = aItem;
        }
        /// <summary>
        /// The new item which has been inserted.
        /// </summary>
        public T Item { get; set; }
        /// <summary>
        /// The index within a list that the new item has been inserted at.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Defines a model of the home as an ordered collection of <see cref="Linn.Kinsky.IRoom"/>s.  
    /// Each room normally equates to one preamp (on a typically configured network), although this is not compulsary.
    /// </summary>
    /// <example>
    /// <code>
    /// 
    /// IPAddress networkInterface = ...; // get an adapter from somewhere
    /// IHouse myHouse = ...; // get hold of an instance of IHouse from somewhere (e.g. <see cref="Linn.Kinsky.House"/>).
    /// myHouse.EventRoomInserted += EventRoomInsertedHandler;
    /// myHouse.Start(networkInterface);
    /// ...
    /// private void EventRoomAddedHandler(object sender, EventArgsItemInsert<IRoom> e)
    /// {
    ///     Console.WriteLine(String.Format("Room added {0}", e.Item.Name));
    /// }
    /// </code>
    /// </example>
    public interface IHouse
    {
        /// <summary>
        /// Starts Room discovery on a given network interface.
        /// </summary>
        /// <param name="aInterface">The <see cref="IPAddress"/> to listen for new rooms being announced.</param>
        void Start(IPAddress aInterface);
        /// <summary>
        /// Stops Room discovery and clears the existing list of rooms.
        /// </summary>
        void Stop();

        /// <summary>
        /// Ordered collection of IRooms present within the house.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        ReadOnlyCollection<IRoom> Rooms { get; }

        /// <summary>
        /// Raised when a new room has been discovered.
        /// </summary>
        event EventHandler<EventArgsItemInsert<IRoom>> EventRoomInserted;
        /// <summary>
        /// Raised when a room has disappeared from the network.
        /// </summary>
        event EventHandler<EventArgsItem<IRoom>> EventRoomRemoved;
    }

    /// <summary>
    /// Defines a room as an ordered collection of <see cref="Linn.Kinsky.ISource"/>s with standby and volume state as well as current track and media time information.
    /// Generally a room defines a single preamp with attached sources on a properly configured system, although it is possible to configure a system with more than one preamp in the room.
    /// At any time, the volume/track/time info reflects the state of the preamp attached to the currently playing source.
    /// </summary>
    /// <remarks>Important: the Volume/Track/Time properties will only return a meaningful value when their EventXXXOpened event is subscribed to and has been raised (e.g. <see cref="EventVolumeOpened"/> and <see cref="EventVolumeClosed"/>).</remarks>
    /// <example>
    /// <code>
    /// IHouse myHouse = ...
    /// ...
    /// ...
    /// ...
    /// IRoom myRoom = myHouse.Rooms[0]; //assume a room has been added
    /// // subscribe to change notifications on volume
    /// myRoom.EventVolumeChanged += EventVolumeChangedHandler;
    /// // must subscribe to these next 2 events for reliable volume indication of the room.
    /// myRoom.EventVolumeOpened += EventVolumeOpenedHandler;
    /// myRoom.EventVolumeClosed += EventVolumeClosedHandler;
    /// ...
    /// private void EventVolumeOpenedHandler(object sender, EventArgs e)
    /// {
    ///     // only now is it safe to use Volume property.
    ///     iVolumeOpen = true;
    /// }
    /// private void EventVolumeClosedHandler(object sender, EventArgs e)
    /// {
    ///     // no longer safe to rely on Volume property.
    ///     iVolumeOpen = false;
    /// }
    /// private void EventVolumeChangedHandler(object sender, EventArgs e)
    /// {
    ///     // check it is safe to rely on Volume property.
    ///     if (iVolumeOpen){
    ///         Console.WriteLine(String.Format("Current volume in room {0} is {1}", myRoom.Name, myRoom.Volume));
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IRoom
    {
        /// <summary>
        /// Opens the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Open();

        /// <summary>
        /// Closes the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Close();

        /// <summary>
        /// The name of the room (configured on device).
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        string Name { get; }

        /// <summary>
        /// Gets or sets the current Standby state of the devices in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool Standby { get; set; }
        /// <summary>
        /// Raised when the standby state of the room changes.
        /// </summary>
        event EventHandler<EventArgs> EventStandbyChanged;

        /// <summary>
        /// Gets the currently selected source (this is set indirectly via <see cref="Linn.Kinsky.ISource.Select"/>).
        /// </summary> 
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        ISource Current { get; }

        /// <summary>
        /// Gets the currently selected preamp.
        /// </summary> 
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        IPreamp Preamp { get; }

        /// <summary>
        /// Ordered collection of ISources present within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        ReadOnlyCollection<ISource> Sources { get; }

        /// <summary>
        /// Raised when a source is added to the room.
        /// </summary>
        event EventHandler<EventArgsItemInsert<ISource>> EventSourceInserted;
        /// <summary>
        /// Raised when a source is removed from a room.
        /// </summary>
        event EventHandler<EventArgsItem<ISource>> EventSourceRemoved;
        /// <summary>
        /// Raised when the currently selected source changes.
        /// </summary>
        event EventHandler<EventArgs> EventCurrentChanged;

        IRoomInfo Info { get; }

        IRoomTime Time { get; }

        IRoomVolume Volume { get; }

    }

    public interface IRoomInfo
    {

        /// <summary>
        /// The current item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        upnpObject Track { get; }
        /// <summary>
        /// The current metatext of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        upnpObject Metatext { get; }
        /// <summary>
        /// The current bitrate of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        uint Bitrate { get; }
        /// <summary>
        /// The current sample rate of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        float SampleRate { get; }
        /// <summary>
        /// The current bit depth of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        uint BitDepth { get; }
        /// <summary>
        /// The current codec of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        string Codec { get; }
        /// <summary>
        /// True if the current format of the item playing within the room uses lossless encoding.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Info service is currently being subscribed to (<see cref="EventInfoOpened"/> and <see cref="EventInfoClosed"/>).</remarks>
        bool Lossless { get; }

        /// <summary>
        /// This event should be handled in client code and must have been raised by the room before the <see cref="Track"/>/<see cref="Metatext"/>/<see cref="Bitrate"/>/<see cref="SampleRate"/>/<see cref="BitDepth"/>/<see cref="Codec"/> and <see cref="Lossless"/> properties are guaranteed to contain valid data.
        /// </summary>
        event EventHandler<EventArgs> EventOpened;
        /// <summary>
        /// This event is raised when the room's current source has changed. 
        /// </summary>
        event EventHandler<EventArgs> EventClosed;
        /// <summary>
        /// Raised when the room's <see cref="Track"/> property has changed.
        /// </summary>
        event EventHandler<EventArgs> EventTrackChanged;
        /// <summary>
        /// Raised when the room's <see cref="Metatext"/> property has changed.
        /// </summary>
        event EventHandler<EventArgs> EventMetatextChanged;
        /// <summary>
        /// Raised when any of the room's track details properties have changed (<see cref="Bitrate"/>/<see cref="SampleRate"/>/<see cref="BitDepth"/>/<see cref="Codec"/> and <see cref="Lossless"/>).
        /// </summary>
        event EventHandler<EventArgs> EventDetailsChanged;

    }

    public interface IRoomTime
    {
        /// <summary>
        /// The current duration in seconds of the item playing within the room (or 0 if unknown/unlimited).
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Time service is currently being subscribed to (<see cref="EventTimeOpened"/> and <see cref="EventTimeClosed"/>).</remarks>
        uint Duration { get; }
        /// <summary>
        /// The current elapsed time in seconds of the item playing within the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the Time service is currently being subscribed to (<see cref="EventTimeOpened"/> and <see cref="EventTimeClosed"/>).</remarks>
        uint Seconds { get; }
        /// <summary>
        /// This event should be handled in client code and must have been raised by the room before the <see cref="Duration"/> and <see cref="Seconds"/> properties are guaranteed to contain valid data.
        /// </summary>
        event EventHandler<EventArgs> EventOpened;
        /// <summary>
        /// This event is raised when the room's current source has changed. 
        /// </summary>
        event EventHandler<EventArgs> EventClosed;
        /// <summary>
        /// Raised when the room's <see cref="Seconds"/> property has changed.
        /// </summary>
        event EventHandler<EventArgs> EventSecondsChanged;
        /// <summary>
        /// Raised when the room's <see cref="Duration"/> property has changed.
        /// </summary>
        event EventHandler<EventArgs> EventDurationChanged;
    }

    public interface IRoomVolume
    {
        /// <summary>
        /// Increases the volume by a predefined amount on the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void IncrementVolume();
        /// <summary>
        /// Decreases the volume by a predefined amount on the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void DecrementVolume();
        /// <summary>
        /// Toggles the mute state of the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void ToggleMute();
        /// <summary>
        /// Returns the maximum volume of the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the preamp is currently being subscribed to (<see cref="EventVolumeOpened"/> and <see cref="EventVolumeClosed"/>).</remarks>
        uint VolumeLimit { get; }
        /// <summary>
        /// Gets or sets the current volume of the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the preamp is currently being subscribed to (<see cref="EventVolumeOpened"/> and <see cref="EventVolumeClosed"/>).</remarks>
        uint Volume { get; set; }
        /// <summary>
        /// Gets or sets the current mute state of the currently selected preamp in the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <remarks>Important: this property will only return a meaningful value when the preamp is currently being subscribed to (<see cref="EventVolumeOpened"/> and <see cref="EventVolumeClosed"/>).</remarks>
        bool Mute { get; set; }

        /// <summary>
        /// The first subscription to this event causes the room to subscribe to the Volume service on the current preamp.  
        /// This event must be handled in client code and must have been raised by the room before the <see cref="Volume"/>/<see cref="VolumeLimit"/> and <see cref="Mute"/> properties are guaranteed to contain valid data.
        /// </summary>
        event EventHandler<EventArgs> EventOpened;

        /// <summary>
        /// This event is raised when the room's preamp has changed. 
        /// </summary>
        event EventHandler<EventArgs> EventClosed;
        /// <summary>
        /// Raised when the room's <see cref="VolumeLimit"/> has changed.
        /// </summary>
        event EventHandler<EventArgs> EventVolumeLimitChanged;
        /// <summary>
        /// Raised when the room's <see cref="Volume"/> has changed.
        /// </summary>
        event EventHandler<EventArgs> EventVolumeChanged;
        /// <summary>
        /// Raised when the room's <see cref="Mute"/> state has changed.
        /// </summary>
        event EventHandler<EventArgs> EventMuteChanged;

    }

    /// <summary>
    /// Defines a single source of music within a room.  For example a playlist, radio receiver or aux input on a preamp.
    /// </summary>
    /// <remarks>Important: the TransportState property will only return a meaningful value when the EventOpened event is subscribed to and has been raised (see <see cref="EventOpened"/> and <see cref="EventClosed"/>).</remarks>
    /// <example>
    /// <code>
    /// IHouse myHouse = ...; // assume a house has been created and started
    /// IRoom myRoom = myHouse.Rooms[0]; //assume a room inserted event has previously been received from the house
    /// ISource mySource = myRoom.Sources[0]; // assume a source inserted event has previously been received from the room
    /// // subscribe to change notifications on TransportState
    /// mySource.EventTransportStateChanged += EventTransportStateChangedHandler;
    /// // must subscribe to these next 2 events for reliable volume indication of the room.
    /// mySource.EventOpened += EventOpenedHandler;
    /// mySource.EventClosed += EventClosedHandler;
    /// ...
    /// private void EventOpenedHandler(object sender, EventArgs e)
    /// {
    ///     // only now is it safe to use TransportState property.
    ///     iOpen = true;
    /// }
    /// private void EventClosedHandler(object sender, EventArgs e)
    /// {
    ///     // no longer safe to rely on TransportState property.
    ///     iOpen = false;
    /// }
    /// private void EventTransportStateChangedHandler(object sender, EventArgs e)
    /// {
    ///     // check it is safe to rely on TransportState property.
    ///     if (iOpen){
    ///         Console.WriteLine(String.Format("Current transport state in source {0} is {1}", mySource.Name, mySource.TransportState));
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ISource
    {
        /// <summary>
        /// The unique name of the source on the network.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        string Udn { get; }
        /// <summary>
        /// The human readable name of the source.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        string Name { get; }
        /// <summary>
        /// The type of source (see <see cref="Linn.Kinsky.Source"/>).
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        string Type { get; }
        /// <summary>
        /// Raised when the source's name changes.
        /// </summary>
        event EventHandler<EventArgs> EventNameChanged;

        /// <summary>
        /// The <see cref="Linn.Kinsky.IRoom"/> that this source belongs to.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        IRoom Room { get; }

        /// <summary>
        /// Sets this source to be the current source of music for the room.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Select();

        /// <summary>
        /// The first subscription to this event causes the source to subscribe to services on the currently playing device.  
        /// This event must be handled in client code and must have been raised by the source before the <see cref="TransportState"/> property is guaranteed to contain valid data.
        /// </summary>
        event EventHandler<EventArgs> EventOpened;
        /// <summary>
        /// Removing the last subscription to this event causes the source to unsubscribe from services on the currently playing device.  
        /// If this event been raised by the source since the last EventOpened was raised, then the<see cref="TransportState"/> property is no longer guaranteed to contain valid data.
        /// </summary>
        event EventHandler<EventArgs> EventClosed;

        /// <summary>
        /// Seeks to the specified location within the currently playing track.
        /// </summary>
        /// <param name="aSeconds">The location in seconds to seek.</param>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSeeking"/> returns false.</exception>
        void Seek(uint aSeconds);
        /// <summary>
        /// Pauses the currently playing track.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSetTransportState"/> returns false or when the source type does not allow pausing.</exception>
        void Pause();
        /// <summary>
        /// Plays the currently selected track.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSetTransportState"/> returns false.</exception>
        void Play();
        /// <summary>
        /// Stops the currently selected track.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSetTransportState"/> returns false.</exception>
        void Stop();
        /// <summary>
        /// Skips to the previous item if available (e.g. Track on a Playlist source, Preset on a radio source or ISender on a Receiver Source).
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSkipping"/> returns false.</exception>
        bool Previous();
        /// <summary>
        /// Skips to the next item if available (e.g. Track on a Playlist source, Preset on a radio source or ISender on a Receiver Source).
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowSkipping"/> returns false.</exception>
        bool Next();

        /// <summary>
        /// Retrieves and enqueues the specified media for immediate playing, then plays the source.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowPlayNowNextLater"/> returns false.</exception>
        /// <param name="aRetriever">Retrieves the media to play.</param>
        void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback);
        /// <summary>
        /// Retrieves and enqueues the specified media for subsequent playing.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowPlayNowNextLater"/> returns false.</exception>
        /// <param name="aRetriever">Retrieves the media to play.</param>
        void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback);
        /// <summary>
        /// Retrieves and enqueues the specified media for future playing.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        /// <exception cref="System.InvalidOperationException">Raised when <see cref="AllowPlayNowNextLater"/> returns false.</exception>
        /// <param name="aRetriever">Retrieves the media to play.</param>
        void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback);

        /// <summary>
        /// Indicates whether the source allows <see cref="Seek"/> to be called.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool AllowSeeking { get; }
        /// <summary>
        /// Indicates whether the source allows <see cref="Previous"/> and <see cref="Next"/>to be called.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool AllowSkipping { get; }
        /// <summary>
        /// Indicates whether the source allows <see cref="Play"/>, <see cref="Pause"/> and <see cref="Stop"/>to be called.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool AllowSetTransportState { get; }
        /// <summary>
        /// Indicates whether the source allows <see cref="PlayNow"/>, <see cref="PlayNext"/> and <see cref="PlayLater"/>to be called.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool AllowPlayNowNextLater { get; }
        /// <summary>
        /// Returns the current transport (i.e. play) state of the source.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        ETransportState TransportState { get; }
        event EventHandler<EventArgs> EventTransportStateChanged;

    }

    /// <summary>
    /// Indicates a source which exposes an ordered collection of items and a single 'current selected' item.
    /// </summary>
    /// <typeparam name="T">The type of item exposed.</typeparam>
    public interface IItemsProvider<T>
    {
        /// <summary>
        /// Currently selected item.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        T Current { get; set; }
        /// <summary>
        /// Raised when the currently selected item changes.
        /// </summary>
        event EventHandler<EventArgs> EventCurrentChanged;
        /// <summary>
        /// Collection of items.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        ReadOnlyCollection<T> Items { get; }
        /// <summary>
        /// Raised when the collection of items changes.
        /// </summary>
        event EventHandler<EventArgs> EventItemsChanged;
    }

    /// <summary>
    /// Indicates a source which exposes <see cref="Linn.Topology.Channel"/> state.
    /// </summary>
    public interface IChannelProvider
    {
        /// <summary>
        /// Getter for current Channel.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        Channel Channel { get; }
        /// <summary>
        /// Setter for current Channel
        /// </summary>
        /// <param name="aDidlLite">Provided in <see cref="DidlLite"/> format.</param>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void SetChannel(DidlLite aDidlLite, bool aPlay);
        /// <summary>
        /// Raised when the current channel has changed.
        /// </summary>
        event EventHandler<EventArgs> EventChannelChanged;
    }

    /// <summary>
    /// Indicates a source which exposes PlayMode (i.e. Shuffle/Repeat) state.
    /// </summary>
    public interface IPlayModeProvider
    {
        /// <summary>
        /// Indicates whether shuffle mode is enabled.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool Shuffle { get; set; }
        /// <summary>
        /// Raised when shuffle mode changes.
        /// </summary>
        event EventHandler<EventArgs> EventShuffleChanged;

        /// <summary>
        /// Indicates whether repeat mode is enabled.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        bool Repeat { get; set; }
        /// <summary>
        /// Raised when repeat mode changes.
        /// </summary>
        event EventHandler<EventArgs> EventRepeatChanged;
    }

    /// <summary>
    /// Playlist or UpnpAv source.  Exposes additional methods for playlist order manipulation.
    /// </summary>
    public interface IPlaylistSource : ISource, IPlayModeProvider, IItemsProvider<MrItem>
    {
        /// <summary>
        /// Adds items to playlist.
        /// </summary>
        /// <param name="aInsertAfterId">The Id of the item after which to insert content.</param>
        /// <param name="aMediaRetriever">Retrieves the media to insert.</param>
        /// <returns>The number of items inserted into the playlist.</returns>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Insert(uint aInsertAfterId, IMediaRetriever aMediaRetriever, Action<uint> aCallback);
        /// <summary>
        /// Removes a list of items from their current location within a playlist and inserts them elsewhere.
        /// </summary>
        /// <param name="aInsertAfterId">The Id of the item after which to insert content.</param>
        /// <param name="aPlaylistItems">The items to move.</param>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Move(uint aInsertAfterId, IList<MrItem> aPlaylistItems, System.Action aCallback);
        /// <summary>
        /// Deletes items from a playlist.
        /// </summary>
        /// <param name="aPlaylistItems">The items to delete.</param>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Delete(IList<MrItem> aPlaylistItems);
        /// <summary>
        /// Clears a playlist of all items.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void DeleteAll();
    }

    /// <summary>
    /// Disc player source.
    /// </summary>
    public interface IDiscSource : ISource, IPlayModeProvider
    {
        /// <summary>
        /// Ejects the CD tray.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        void Eject();
    }
    /// <summary>
    /// Radio source.
    /// </summary>
    public interface IRadioSource : ISource, IChannelProvider, IItemsProvider<MrItem>
    { }
    /// <summary>
    /// Receiver source (implements the Openhome Songcast Receiver service which allows easy streaming of media within the home).
    /// </summary>
    public interface IReceiverSource : ISource, IChannelProvider, IItemsProvider<IModelSender>
    { }

    /// <summary>
    /// Default implementation of <see cref="Linn.Kinsky.IHouse"/>.
    /// </summary>
    public class House : IHouse
    {
        /// <summary>
        /// Creates a new instance of House.
        /// </summary>
        /// <param name="aHouse">Topology house.</param>
        /// <param name="aInvoker">An instance of <see cref="Linn.Kinsky.IInvoker"/> which ensures a single threaded mode of operation.</param>
        /// <param name="aModelSenders">Provides a list of Openhome Songcast senders.</param>
        public House(Linn.Topology.IHouse aHouse, IInvoker aInvoker, IModelSenders aModelSenders)
        {
            iLockObject = new object();
            iModelSenders = aModelSenders;
            iInvoker = aInvoker;
            iHouse = aHouse;
            iRooms = new List<Room>();
            iScheduler = new Scheduler("KinskyHouse", 1);
        }

        #region IHouse Members
        /// <summary>
        /// Starts Room discovery on a given network interface.
        /// </summary>
        /// <param name="aInterface">The <see cref="IPAddress"/> to listen for new rooms being announced.</param>
        public void Start(IPAddress aInterface)
        {
            lock (iLockObject)
            {
                iOpen = true;
                Trace.WriteLine(Trace.kKinsky, "Kinsky.House Start()");
                iHouse.EventRoomAdded += RoomAdded;
                iHouse.EventRoomRemoved += RoomRemoved;
                iHouse.Start(aInterface);
            }
        }
        /// <summary>
        /// Stops Room discovery and clears the existing list of rooms.
        /// </summary>
        private delegate void DRemoveRoom(Room aRoom);
        public void Stop()
        {
            lock (iLockObject)
            {
                Trace.WriteLine(Trace.kKinsky, "Kinsky.House Stop()");
                iOpen = false;
                iHouse.EventRoomAdded -= RoomAdded;
                iHouse.EventRoomRemoved -= RoomRemoved;
                iHouse.Stop();
                List<Room> rooms = new List<Room>(iRooms);
                foreach (Room r in rooms)
                {
                    iRooms.Remove(r);
                    iInvoker.BeginInvoke(new DRemoveRoom(delegate(Room aRoom)
                    {
                        r.Dispose();
                        OnEventRoomRemoved(aRoom);
                    }), r);
                }
                Assert.Check(iRooms.Count == 0);
            }
        }


        /// <summary>
        /// Ordered collection of IRooms present within the house.
        /// </summary>
        /// <exception cref="Linn.Kinsky.InvocationException">This code must only be called by client code on the application Invoke thread (see <see cref="Linn.Kinsky.IInvoker"/>).</exception>
        public ReadOnlyCollection<IRoom> Rooms
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                List<IRoom> rooms = new List<IRoom>();
                lock (iLockObject)
                {
                    foreach (Room r in iRooms)
                    {
                        rooms.Add(r);
                    }
                }
                return new ReadOnlyCollection<IRoom>(rooms);
            }
        }

        /// <summary>
        /// Raised when a new room has been discovered.
        /// </summary>
        public event EventHandler<EventArgsItemInsert<IRoom>> EventRoomInserted;
        private void OnEventRoomInserted(int aIndex, IRoom aRoom)
        {
            EventHandler<EventArgsItemInsert<IRoom>> eventHandler = EventRoomInserted;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgsItemInsert<IRoom>(aIndex, aRoom));
            }
        }

        /// <summary>
        /// Raised when a room has disappeared from the network.
        /// </summary>
        public event EventHandler<EventArgsItem<IRoom>> EventRoomRemoved;
        private void OnEventRoomRemoved(IRoom aRoom)
        {
            EventHandler<EventArgsItem<IRoom>> eventHandler = EventRoomRemoved;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgsItem<IRoom>(aRoom));
            }
        }


        #endregion

        public void RemoveDevice(Device aDevice)
        {
            iHouse.RemoveDevice(aDevice);
        }

        #region Helper Methods

        private delegate void DRoomAdded(object sender, Topology.EventArgsRoom e);
        private void RoomAdded(object sender, Topology.EventArgsRoom e)
        {
            Delegate del = new DRoomAdded(delegate(object s, Topology.EventArgsRoom args)
                   {
                       lock (iLockObject)
                       {
                           if (iOpen)
                           {
                               Room newRoom = null;
                               int insertIndex = 0;
                               Trace.WriteLine(Trace.kKinsky, "Kinsky.House: Room added: " + args.Room);

                               newRoom = new Room(this, e.Room, iInvoker, iHouse.ModelFactory, iModelSenders, iScheduler);
                               Room existing = null;
                               foreach (Room r in iRooms)
                               {
                                   if (r.Name == args.Room.Name)
                                   {
                                       existing = r;
                                   }
                                   if (newRoom.CompareTo(r) < 0)
                                   {
                                       break;
                                   }
                                   insertIndex += 1;
                               }
                               if (existing != null)
                               {
                                   insertIndex -= 1;
                                   iRooms.Remove(existing);
                                   existing.Dispose();
                                   OnEventRoomRemoved(existing);
                               }
                               iRooms.Insert(insertIndex, newRoom);
                               OnEventRoomInserted(insertIndex, newRoom);
                           }
                       }
                   });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private delegate void DRoomRemoved(object sender, Topology.EventArgsRoom e);
        private void RoomRemoved(object sender, Topology.EventArgsRoom e)
        {
            Delegate del = new DRoomRemoved(delegate(object s, Topology.EventArgsRoom args)
                      {
                          lock (iLockObject)
                          {
                              if (iOpen)
                              {
                                  Room found = null;
                                  Trace.WriteLine(Trace.kKinsky, "Kinsky.House: Room removed: " + e.Room);
                                  foreach (Room r in iRooms)
                                  {
                                      if (r.TopologyRoom == e.Room)
                                      {
                                          found = r;
                                          break;
                                      }
                                  }
                                  if (found != null)
                                  {
                                      iRooms.Remove(found);
                                      OnEventRoomRemoved(found);
                                      found.Dispose();
                                  }
                              }
                          }
                      });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        #endregion

        private Topology.IHouse iHouse;
        private List<Room> iRooms;
        private object iLockObject;
        private IInvoker iInvoker;
        private bool iOpen;
        private IModelSenders iModelSenders;
        private Scheduler iScheduler;
    }

    /// <summary>
    /// Defines a room as an ordered collection of <see cref="Linn.Kinsky.ISource"/>s with standby and volume state as well as current track and media time information.
    /// Generally a room defines a single preamp with attached sources on a properly configured system, although it is possible to configure a system with more than one preamp in the room.
    /// At any time, the volume/track/time info reflects the state of the preamp attached to the currently playing source.
    /// </summary>
    /// <remarks>Important: the Volume/Track/Time properties will only return a meaningful value when their EventXXXOpened event is subscribed to and has been raised (e.g. <see cref="EventVolumeOpened"/> and <see cref="EventVolumeClosed"/>).</remarks>
    /// <example>
    /// <code>
    /// IHouse myHouse = ...
    /// ...
    /// ...
    /// ...
    /// IRoom myRoom = myHouse.Rooms[0]; //assume a room has been added
    /// // subscribe to change notifications on volume
    /// myRoom.EventVolumeChanged += EventVolumeChangedHandler;
    /// // must subscribe to these next 2 events for reliable volume indication of the room.
    /// myRoom.EventVolumeOpened += EventVolumeOpenedHandler;
    /// myRoom.EventVolumeClosed += EventVolumeClosedHandler;
    /// ...
    /// private void EventVolumeOpenedHandler(object sender, EventArgs e)
    /// {
    ///     // only now is it safe to use Volume property.
    ///     iVolumeOpen = true;
    /// }
    /// private void EventVolumeClosedHandler(object sender, EventArgs e)
    /// {
    ///     // no longer safe to rely on Volume property.
    ///     iVolumeOpen = false;
    /// }
    /// private void EventVolumeChangedHandler(object sender, EventArgs e)
    /// {
    ///     // check it is safe to rely on Volume property.
    ///     if (iVolumeOpen){
    ///         Console.WriteLine(String.Format("Current volume in room {0} is {1}", myRoom.Name, myRoom.Volume));
    ///     }
    /// }
    /// </code>
    /// </example>
    public class Room : IComparable<Room>, IRoom, IDisposable
    {
        /// <summary>
        /// Creates a new instance of Room
        /// </summary>
        /// <param name="aRoom">Wrapped topology room.</param>
        /// <param name="aInvoker">An instance of <see cref="Linn.Kinsky.IInvoker"/> which ensures a single threaded mode of operation.</param>
        /// <param name="aModelFactory">A factory to create Topology model instances or retrieve existing instances from cache.</param>
        /// <param name="aModelSenders">Provides a list of Openhome Songcast senders.</param>
        internal Room(House aHouse, Topology.IRoom aRoom, IInvoker aInvoker, IModelFactory aModelFactory, IModelSenders aModelSenders, Scheduler aScheduler)
        {
            iHouse = aHouse;
            iScheduler = aScheduler;
            iModelFactory = aModelFactory;
            iInvoker = aInvoker;
            iModelSenders = aModelSenders;
            iSources = new List<Source>();
            Assert.Check(aRoom != null);
            iRoom = aRoom;
            iCurrentSource = null;
            iCurrentPreamp = null;
            iOpen = false;
        }

        #region IRoom Members


        public void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (!iOpen)
            {
                iRoom.EventSourceAdded += EventSourceAddedHandler;
                iRoom.EventSourceRemoved += EventSourceRemovedHandler;
                iRoom.EventCurrentChanged += EventCurrentChangedHandler;
                foreach (Topology.ISource s in iRoom.Sources)
                {
                    EventSourceAddedHandler(this, new EventArgsSource(s));
                }
                EventCurrentChangedHandler(this, EventArgs.Empty);
                iRoom.EventPreampChanged += EventPreampChangedHandler;
                EventPreampChangedHandler(this, EventArgs.Empty);
                iOpen = true;
            }
        }

        public void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                iRoom.EventSourceAdded -= EventSourceAddedHandler;
                iRoom.EventSourceRemoved -= EventSourceRemovedHandler;
                iRoom.EventCurrentChanged -= EventCurrentChangedHandler;
                iRoom.EventPreampChanged -= EventPreampChangedHandler;
                List<Source> sources = new List<Source>();
                foreach (Source s in iSources)
                {
                    sources.Add(s);
                }
                foreach (Source source in sources)
                {
                    EventSourceRemovedHandler(this, new EventArgsSource(source.TopologySource));
                    if (iCurrentSource == source)
                    {
                        iCurrentSource = null;
                    }
                }
                if (iCurrentSource != null)
                {
                    iCurrentSource.Close();
                    iCurrentSource = null;
                }
                Assert.Check(iSources.Count == 0);
                iCurrentPreamp = null;
                iOpen = false;
            }
        }
        public string Name
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iRoom.Name;
            }
        }

        private delegate void DSetStandby(bool aStandby);
        public bool Standby
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iRoom.Standby;
            }
            set
            {
                Delegate del = new DSetStandby(delegate(bool aStandby)
                {
                    if (iRoom.Standby != aStandby)
                    {
                        iRoom.SetStandby(aStandby);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        private delegate void DSetCurrent(ISource aCurrent);
        public ISource Current
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iCurrentSource;
            }
        }

        private delegate void DSetPreamp(IPreamp aPreamp);
        public IPreamp Preamp
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iCurrentPreamp;
            }
        }

        public ReadOnlyCollection<ISource> Sources
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                List<ISource> sources = new List<ISource>();
                foreach (Source s in iSources)
                {
                    sources.Add(s);
                }
                return new ReadOnlyCollection<ISource>(sources);
            }
        }

        public event EventHandler<EventArgs> EventCurrentChanged;
        private void OnEventCurrentChanged()
        {
            EventHandler<EventArgs> eventHandler = EventCurrentChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private EventHandler<EventArgs> DEventStandbyChanged;
        public event EventHandler<EventArgs> EventStandbyChanged
        {
            add
            {
                lock (iRoom)
                {
                    if (DEventStandbyChanged == null)
                    {
                        iRoom.EventStandbyChanged += EventStandbyChangedHandler;
                    }
                    DEventStandbyChanged += value;
                }
            }
            remove
            {
                lock (iRoom)
                {
                    DEventStandbyChanged -= value;
                    if (DEventStandbyChanged == null)
                    {
                        iRoom.EventStandbyChanged -= EventStandbyChangedHandler;
                    }
                }
            }
        }
        private void OnEventStandbyChanged()
        {
            EventHandler<EventArgs> eventHandler = DEventStandbyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgsItemInsert<ISource>> EventSourceInserted;
        private void OnEventSourceInserted(int aIndex, ISource aSource)
        {
            EventHandler<EventArgsItemInsert<ISource>> eventHandler = EventSourceInserted;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgsItemInsert<ISource>(aIndex, aSource));
            }
        }

        public event EventHandler<EventArgsItem<ISource>> EventSourceRemoved;
        private void OnEventSourceRemoved(ISource aSource)
        {
            EventHandler<EventArgsItem<ISource>> eventHandler = EventSourceRemoved;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgsItem<ISource>(aSource));
            }
        }

        public IRoomInfo Info
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iInfoModel == null)
                {
                    iInfoModel = new RoomInfo(this, iInvoker, iModelFactory, iScheduler);
                }
                return iInfoModel;
            }
        }

        public IRoomTime Time
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iTimeModel == null)
                {
                    iTimeModel = new RoomTime(this, iInvoker, iModelFactory, iScheduler);
                }
                return iTimeModel;
            }
        }

        public IRoomVolume Volume
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iVolumeModel == null)
                {
                    iVolumeModel = new RoomVolume(this, iInvoker, iModelFactory, iScheduler);
                }
                return iVolumeModel;
            }
        }
        #endregion

        internal House House
        {
            get
            {
                return iHouse;
            }
        }

        #region IComparable<Room> Members

        public int CompareTo(Room other)
        {
            if (other == null) return -1;
            return Name.CompareTo(other.Name);
        }

        #endregion

        #region Helper Methods

        private delegate void DEventPreampChangedHandler();
        private void EventPreampChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventPreampChangedHandler(delegate()
            {
                if (iRoom.Preamp != iCurrentPreamp)
                {
                    iCurrentPreamp = iRoom.Preamp;
                    if (iVolumeModel != null)
                    {
                        iVolumeModel.Refresh();
                    }
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventStandbyChangedHandler();
        private void EventStandbyChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventStandbyChangedHandler(delegate()
            {
                OnEventStandbyChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventCurrentChangedHandler();
        private void EventCurrentChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventCurrentChangedHandler(delegate()
            {
                bool changed = false;
                ISource oldSource = iCurrentSource;
                Topology.ISource current = iRoom.Current;
                IGroup oldGroup = iCurrentSource != null ? iCurrentSource.TopologySource.Group : null;
                if (current == null)
                {
                    changed = iCurrentSource != null;
                    if (changed && !iSources.Contains(iCurrentSource))
                    {
                        iCurrentSource.Close();
                        iCurrentSource = null;
                    }
                    Trace.WriteLine(Trace.kKinsky, "CurrentChanged: " + Name + ", null");
                }
                else
                {
                    bool found = false;
                    foreach (Source s in iSources)
                    {
                        if (s.TopologySource == iRoom.Current)
                        {
                            changed = iCurrentSource != s;
                            iCurrentSource = s;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        changed = true;
                        if (current != null)
                        {
                            if (iCurrentSource == null || iCurrentSource.TopologySource != current)
                            {
                                iCurrentSource = SourceFactory.CreateSource(current, this, iInvoker, iModelFactory, iModelSenders, iScheduler);
                            }
                        }
                    }
                    Trace.WriteLine(Trace.kKinsky, "CurrentChanged: " + found + ", " + Name + ", " + iCurrentSource != null ? iCurrentSource.Name : ", null");
                }
                if (changed)
                {
                    OnEventCurrentChanged();
                    IGroup newGroup = iCurrentSource != null ? iCurrentSource.TopologySource.Group : null;
                    if (iInfoModel != null && (newGroup != oldGroup || (oldSource.Type == Source.kSourceUpnpAv && oldSource != iCurrentSource)))
                    {
                        iInfoModel.Refresh();
                    }
                    if (iTimeModel != null && (newGroup != oldGroup || (oldSource.Type == Source.kSourceUpnpAv && oldSource != iCurrentSource)))
                    {
                        iTimeModel.Refresh();
                    }
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventSourceAddedHandler(Topology.ISource aSource);
        private void EventSourceAddedHandler(object sender, Topology.EventArgsSource e)
        {
            Delegate del = new DEventSourceAddedHandler(delegate(Topology.ISource aSource)
            {
                bool currentChanged = false;
                int insertIndex = 0;
                Source newSource = SourceFactory.CreateSource(aSource, this, iInvoker, iModelFactory, iModelSenders, iScheduler);
                List<Source> updatedSources = new List<Source>();
                if (aSource == iRoom.Current)
                {
                    currentChanged = true;
                }
                foreach (Source s in iSources)
                {
                    if (s.TopologySource == aSource)
                    {
                        // source already exists, do not re-add
                        return;
                    }
                    if (s.Name == newSource.SourceName)
                    {
                        s.Name = String.Format("{0} ({1})", s.SourceName, s.GroupName);
                        updatedSources.Add(s);
                    }
                }
                for (int i = 0; i < iSources.Count; i++)
                {
                    if (newSource.CompareTo(iSources[i]) < 0)
                    {
                        break;
                    }
                    insertIndex++;
                }
                if (updatedSources.Count > 0)
                {
                    newSource.Name = String.Format("{0} ({1})", newSource.SourceName, newSource.GroupName);
                }
                iSources.Insert(insertIndex, newSource);
                OnEventSourceInserted(insertIndex, newSource);
                if (currentChanged)
                {
                    EventCurrentChangedHandler(this, new EventArgs());
                }
            });
            if (iInvoker.TryBeginInvoke(del, e.Source))
                return;
            del.Method.Invoke(del.Target, new object[] { e.Source });
        }

        private delegate void DEventSourceRemovedHandler(Topology.ISource aSource);
        private void EventSourceRemovedHandler(object sender, Topology.EventArgsSource e)
        {
            Delegate del = new DEventSourceRemovedHandler(delegate(Topology.ISource aSource)
            {
                if (aSource == null)
                {
                    UserLog.WriteLine("EventSourceRemovedHandler: aSource is null");
                    Assert.Check(false);
                }
                if (iRoom == null)
                {
                    UserLog.WriteLine("EventSourceRemovedHandler: iRoom is null");
                    Assert.Check(false);
                }
                bool currentChanged = false;
                Source removedSource = null;
                List<Source> updatedSources = new List<Source>();
                if (aSource == iRoom.Current)
                {
                    currentChanged = true;
                }
                foreach (Source s in iSources)
                {
                    if (s == null)
                    {
                        UserLog.WriteLine("EventSourceRemovedHandler: s is null");
                        Assert.Check(false);
                    }
                    if (s.TopologySource == aSource)
                    {
                        removedSource = s;
                        break;
                    }
                }
                // ensure source has not already been removed
                if (removedSource != null)
                {
                    removedSource.Close();
                    iSources.Remove(removedSource);
                    foreach (Source s in iSources)
                    {
                        if (s == null)
                        {
                            UserLog.WriteLine("EventSourceRemovedHandler: s is null");
                            Assert.Check(false);
                        }
                        if (s.SourceName == removedSource.SourceName)
                        {
                            updatedSources.Add(s);
                        }
                    }

                    if (updatedSources.Count == 1)
                    {
                        updatedSources[0].Name = updatedSources[0].SourceName;
                    }
                    OnEventSourceRemoved(removedSource);
                }
                if (currentChanged)
                {
                    EventCurrentChangedHandler(this, new EventArgs());
                }
            });
            if (iInvoker.TryBeginInvoke(del, e.Source))
                return;
            del.Method.Invoke(del.Target, new object[] { e.Source });
        }

        internal Topology.IRoom TopologyRoom
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iRoom;
            }
        }

        #endregion

        public override string ToString()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            return string.Format("Kinsky.Room: {0} ", Name);
        }

        private IPreamp iCurrentPreamp;
        private Source iCurrentSource;
        private List<Source> iSources;
        private Topology.IRoom iRoom;
        private IInvoker iInvoker;
        private IModelFactory iModelFactory;
        private IModelSenders iModelSenders;
        private Scheduler iScheduler;
        private RoomInfo iInfoModel;
        private RoomTime iTimeModel;
        private RoomVolume iVolumeModel;
        private bool iOpen;
        private House iHouse;

        #region IDisposable Members

        public void Dispose()
        {
            if (iOpen)
            {
                Close();
            }
            if (iVolumeModel != null)
            {
                iVolumeModel.Dispose();
                iVolumeModel = null;
            }
            if (iInfoModel != null)
            {
                iInfoModel.Dispose();
                iInfoModel = null;
            }
            if (iVolumeModel != null)
            {
                iVolumeModel.Dispose();
                iVolumeModel = null;
            }
        }

        #endregion
    }

    public class RoomInfo : IRoomInfo, IDisposable
    {

        public RoomInfo(Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
        {
            iRoom = aRoom;
            iInvoker = aInvoker;
            iModelFactory = aModelFactory;
            iScheduler = aScheduler;
            iSubscriptionCount = 0;
            CreateModel();
        }

        public event EventHandler<EventArgs> EventTrackChanged;
        private void OnEventTrackChanged()
        {
            EventHandler<EventArgs> eventHandler = EventTrackChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventMetatextChanged;
        private void OnEventMetatextChanged()
        {
            EventHandler<EventArgs> eventHandler = EventMetatextChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventDetailsChanged;
        private void OnEventDetailsChanged()
        {
            EventHandler<EventArgs> eventHandler = EventDetailsChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                Topology.ISource currentSource = iRoom.TopologyRoom.Current;
                if (iRoom.Current != null && currentSource != null && currentSource.Group.HasInfo)
                {
                    iModel = iModelFactory.CreateModelInfo(currentSource);
                }
                else
                {
                    iModel = null;
                }
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + iRoom.Name + ", " + ex);
                iModel = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && !iModelOpen)
            {
                iModel.EventMetaTextChanged += EventMetatextChangedHandler;
                iModel.EventDetailsChanged += EventDetailsChangedHandler;
                iModel.EventTrackChanged += EventTrackChangedHandler;
                iModel.EventInitialised += EventInitialisedHandler;
                iModel.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelInfo model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && iModelOpen)
            {
                iModel.EventMetaTextChanged -= EventMetatextChangedHandler;
                iModel.EventDetailsChanged -= EventDetailsChangedHandler;
                iModel.EventTrackChanged -= EventTrackChangedHandler;
                iModel.EventInitialised -= EventInitialisedHandler;
                iModel.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelInfo model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelInitialised = false;
                iModelOpen = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModel != null)
                {
                    iRoom.House.RemoveDevice(iModel.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventTrackChangedHandler();
        private void EventTrackChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventTrackChangedHandler(delegate()
            {
                OnEventTrackChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventMetatextChangedHandler();
        private void EventMetatextChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventMetatextChangedHandler(delegate()
            {
                OnEventMetatextChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventDetailsChangedHandler();
        private void EventDetailsChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventDetailsChangedHandler(delegate()
            {
                OnEventDetailsChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventInitialisedHandler();
        private void EventInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });

        }

        public upnpObject Track
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                upnpObject upnpObject = null;
                if (iModel != null)
                {
                    Channel channel = iModel.Track;
                    if (channel != null && channel.DidlLite != null && channel.DidlLite.Count > 0)
                    {
                        upnpObject = channel.DidlLite[0];
                    }
                }
                return upnpObject;
            }
        }
        public upnpObject Metatext
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                upnpObject upnpObject = null;
                if (iModel != null)
                {
                    DidlLite metatext = iModel.Metatext;
                    if (metatext != null && metatext.Count > 0)
                    {
                        upnpObject = metatext[0];
                    }
                }
                return upnpObject;
            }
        }
        public uint Bitrate
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return (uint)(iModel.Bitrate / 1000.0f);
                }
                return 0;
            }
        }

        public float SampleRate
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.SampleRate / 1000.0f;
                }
                return 0f;
            }
        }

        public uint BitDepth
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.BitDepth;
                }
                return 0;
            }
        }

        public string Codec
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                string codec = string.Empty;
                if (iModel != null && iModel.CodecName != null)
                {
                    codec = iModel.CodecName;
                }
                return codec;
            }
        }

        public bool Lossless
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.Lossless;
                }
                return false;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        private void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventClosed;
        private void OnEventClosed()
        {
            EventHandler<EventArgs> eventHandler = EventClosed;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CloseModel();
        }

        #endregion

        internal void Refresh()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private IModelInfo iModel;
        private bool iModelInitialised;
        private bool iModelOpen;
        private Room iRoom;
        private IInvoker iInvoker;
        private Scheduler iScheduler;
        private IModelFactory iModelFactory;
        private int iSubscriptionCount;
    }

    public class RoomTime : IRoomTime, IDisposable
    {

        public RoomTime(Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
        {
            iRoom = aRoom;
            iInvoker = aInvoker;
            iModelFactory = aModelFactory;
            iScheduler = aScheduler;
            iSubscriptionCount = 0;
            CreateModel();
        }

        #region ITime Members

        public event EventHandler<EventArgs> EventDurationChanged;
        private void OnEventDurationChanged()
        {
            EventHandler<EventArgs> eventHandler = EventDurationChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventSecondsChanged;
        private void OnEventSecondsChanged()
        {
            EventHandler<EventArgs> eventHandler = EventSecondsChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public uint Duration
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.Duration;
                }
                return 0;
            }
        }

        public uint Seconds
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.Seconds;
                }
                return 0;
            }
        }

        #endregion

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                Topology.ISource currentSource = iRoom.TopologyRoom.Current;
                if (iRoom.Current != null && currentSource != null && currentSource.Group.HasTime)
                {
                    iModel = iModelFactory.CreateModelTime(currentSource);
                }
                else
                {
                    iModel = null;
                }
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + iRoom.Name + ", " + ex);
                iModel = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && !iModelOpen)
            {
                iModel.EventDurationChanged += EventDurationChangedHandler;
                iModel.EventSecondsChanged += EventSecondsChangedHandler;
                iModel.EventInitialised += EventInitialisedHandler;
                iModel.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelTime model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && iModelOpen)
            {
                iModel.EventDurationChanged -= EventDurationChangedHandler;
                iModel.EventSecondsChanged -= EventSecondsChangedHandler;
                iModel.EventInitialised -= EventInitialisedHandler;
                iModel.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelTime model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelInitialised = false;
                iModelOpen = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModel != null)
                {
                    iRoom.House.RemoveDevice(iModel.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventDurationChangedHandler();
        private void EventDurationChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventDurationChangedHandler(delegate()
            {
                OnEventDurationChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventSecondsChangedHandler();
        private void EventSecondsChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventSecondsChangedHandler(delegate()
            {
                OnEventSecondsChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventInitialisedHandler();
        private void EventInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }
        private void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventClosed;
        private void OnEventClosed()
        {
            EventHandler<EventArgs> eventHandler = EventClosed;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CloseModel();
        }

        #endregion

        internal void Refresh()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private IModelTime iModel;
        private bool iModelInitialised;
        private bool iModelOpen;
        private Room iRoom;
        private IInvoker iInvoker;
        private Scheduler iScheduler;
        private IModelFactory iModelFactory;
        private int iSubscriptionCount;
    }

    public class RoomVolume : IRoomVolume, IDisposable
    {

        public RoomVolume(Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
        {
            iRoom = aRoom;
            iInvoker = aInvoker;
            iModelFactory = aModelFactory;
            iScheduler = aScheduler;
            iSubscriptionCount = 0;
            CreateModel();
        }

        #region IVolume Members

        public void IncrementVolume()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null)
            {
                iModel.IncrementVolume();
            }
        }

        public void DecrementVolume()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null)
            {
                iModel.DecrementVolume();
            }
        }

        public void ToggleMute()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null)
            {
                iModel.ToggleMute();
            }
        }

        public uint VolumeLimit
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.VolumeLimit;
                }
                return 0;
            }
        }

        public uint Volume
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.Volume;
                }
                return 0;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    iModel.SetVolume(value);
                }
            }
        }

        public bool Mute
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    return iModel.Mute;
                }
                return false;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModel != null)
                {
                    iModel.SetMute(value);
                }
            }
        }


        public event EventHandler<EventArgs> EventVolumeLimitChanged;
        private void OnEventVolumeLimitChanged()
        {
            EventHandler<EventArgs> eventHandler = EventVolumeLimitChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventVolumeChanged;
        private void OnEventVolumeChanged()
        {
            EventHandler<EventArgs> eventHandler = EventVolumeChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventMuteChanged;
        private void OnEventMuteChanged()
        {
            EventHandler<EventArgs> eventHandler = EventMuteChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion


        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                if (iRoom.Preamp != null)
                {
                    iModel = iModelFactory.CreateModelVolumeControl(iRoom.TopologyRoom.Preamp);
                }
                else
                {
                    iModel = null;
                }
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + iRoom.Name + ", " + ex);
                iModel = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }

        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && !iModelOpen)
            {
                iModel.EventVolumeLimitChanged += EventVolumeLimitChangedHandler;
                iModel.EventVolumeChanged += EventVolumeChangedHandler;
                iModel.EventMuteStateChanged += EventMuteStateChangedHandler;
                iModel.EventInitialised += EventInitialisedHandler;
                iModel.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelVolumeControl model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModel != null && iModelOpen)
            {
                iModel.EventVolumeLimitChanged -= EventVolumeLimitChangedHandler;
                iModel.EventVolumeChanged -= EventVolumeChangedHandler;
                iModel.EventMuteStateChanged -= EventMuteStateChangedHandler;
                iModel.EventInitialised -= EventInitialisedHandler;
                iModel.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelVolumeControl model = iModel;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelInitialised = false;
                iModelOpen = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModel != null)
                {
                    iRoom.House.RemoveDevice(iModel.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventVolumeLimitChangedHandler();
        private void EventVolumeLimitChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventVolumeLimitChangedHandler(delegate()
            {
                OnEventVolumeLimitChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventVolumeChangedHandler();
        private void EventVolumeChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventVolumeChangedHandler(delegate()
            {
                OnEventVolumeChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventMuteStateChangedHandler();
        private void EventMuteStateChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventMuteStateChangedHandler(delegate()
            {
                OnEventMuteChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventInitialisedHandler();
        private void EventInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        private void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventClosed;
        private void OnEventClosed()
        {
            EventHandler<EventArgs> eventHandler = EventClosed;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CloseModel();
        }

        #endregion

        internal void Refresh()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private IModelVolumeControl iModel;
        private bool iModelInitialised;
        private bool iModelOpen;
        private Room iRoom;
        private IInvoker iInvoker;
        private Scheduler iScheduler;
        private IModelFactory iModelFactory;
        private int iSubscriptionCount;
    }

    internal static class SourceFactory
    {
        public static Source CreateSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, IModelSenders aModelSenders, Scheduler aScheduler)
        {
            if (aSource.Type == Source.kSourceDs || aSource.Type == Source.kSourceUpnpAv)
            {
                return new PlaylistSource(aSource, aRoom, aInvoker, aModelFactory, aScheduler);
            }
            else if (aSource.Type == Source.kSourceRadio)
            {
                return new RadioSource(aSource, aRoom, aInvoker, aModelFactory, aScheduler);
            }
            else if (aSource.Type == Source.kSourceReceiver)
            {
                return new ReceiverSource(aSource, aRoom, aInvoker, aModelFactory, aModelSenders, aScheduler);
            }
            else if (aSource.Type == Source.kSourceDisc)
            {
                return new DiscSource(aSource, aRoom, aInvoker, aModelFactory, aScheduler);
            }
            else
            {
                return new AuxSource(aSource, aRoom, aInvoker, aModelFactory, aScheduler);
            }
        }
    }

    /// <summary>
    /// Defines a single source of music within a room.  For example a playlist, radio receiver or aux input on a preamp.
    /// </summary>
    /// <remarks>Important: the TransportState property will only return a meaningful value when the EventOpened event is subscribed to and has been raised (see <see cref="EventOpened"/> and <see cref="EventClosed"/>).</remarks>
    /// <example>
    /// <code>
    /// IHouse myHouse = ...; // assume a house has been created and started
    /// IRoom myRoom = myHouse.Rooms[0]; //assume a room inserted event has previously been received from the house
    /// ISource mySource = myRoom.Sources[0]; // assume a source inserted event has previously been received from the room
    /// // subscribe to change notifications on TransportState
    /// mySource.EventTransportStateChanged += EventTransportStateChangedHandler;
    /// // must subscribe to these next 2 events for reliable volume indication of the room.
    /// mySource.EventOpened += EventOpenedHandler;
    /// mySource.EventClosed += EventClosedHandler;
    /// ...
    /// private void EventOpenedHandler(object sender, EventArgs e)
    /// {
    ///     // only now is it safe to use TransportState property.
    ///     iOpen = true;
    /// }
    /// private void EventClosedHandler(object sender, EventArgs e)
    /// {
    ///     // no longer safe to rely on TransportState property.
    ///     iOpen = false;
    /// }
    /// private void EventTransportStateChangedHandler(object sender, EventArgs e)
    /// {
    ///     // check it is safe to rely on TransportState property.
    ///     if (iOpen){
    ///         Console.WriteLine(String.Format("Current transport state in source {0} is {1}", mySource.Name, mySource.TransportState));
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class Source : IComparable<Source>, ISource
    {
        public const string kSourceDs = "Playlist";
        public const string kSourceDisc = "Disc";
        public const string kSourceUpnpAv = "UpnpAv";
        public const string kSourceTuner = "Tuner";
        public const string kSourceRadio = "Radio";
        public const string kSourceAux = "Aux";
        public const string kSourceAnalog = "Analog";
        public const string kSourceSpdif = "Spdif";
        public const string kSourceToslink = "Toslink";
        public const string kSourceReceiver = "Receiver";

        internal Source(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
        {
            iScheduler = aScheduler;
            iModelFactory = aModelFactory;
            iInvoker = aInvoker;
            Assert.Check(aSource != null);
            iSource = aSource;
            iName = iSource.Name;
            iRoom = aRoom;
        }

        #region ISource Members

        public string Udn
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iSource.Udn;
            }
        }

        public void Select()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Select();
        }

        private delegate void DSetName(string aName);
        public string Name
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iName;
            }
            internal set
            {
                Delegate del = new DSetName(delegate(string aName)
                {
                    iName = value;
                    OnEventNameChanged();
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        public string Type
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iSource.Type;
            }
        }

        public IRoom Room
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iRoom;
            }
        }

        public abstract void Seek(uint aSeconds);

        public abstract void Pause();

        public abstract void Play();

        public abstract void Stop();

        public abstract bool Previous();

        public abstract bool Next();

        public abstract void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback);

        public abstract void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback);

        public abstract void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback);

        public abstract bool AllowSeeking { get; }

        public abstract bool AllowSkipping { get; }

        public abstract bool AllowSetTransportState { get; }

        public abstract bool AllowPlayNowNextLater { get; }

        public abstract ETransportState TransportState { get; }

        public event EventHandler<EventArgs> EventNameChanged;
        private void OnEventNameChanged()
        {
            EventHandler<EventArgs> eventHandler = EventNameChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public abstract event EventHandler<EventArgs> EventOpened;
        protected abstract void OnEventOpened();

        public event EventHandler<EventArgs> EventClosed;
        protected void OnEventClosed()
        {
            EventHandler<EventArgs> eventHandler = EventClosed;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventTransportStateChanged;
        protected void OnEventTransportStateChanged()
        {
            EventHandler<EventArgs> eventHandler = EventTransportStateChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventStandbyChanged;
        private void OnEventStandbyChanged()
        {
            EventHandler<EventArgs> eventHandler = EventStandbyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IComparable<Source> Members

        public int CompareTo(Source other)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (other == null)
            {
                return -1;
            }
            if (Type == kSourceDs || other.Type == kSourceDs)
            {
                if (iSource.Type == kSourceDs && other.Type == kSourceDs)
                {
                    return GroupName.CompareTo(other.GroupName);
                }
                return Type == kSourceDs ? -1 : 1;
            }
            if (Type == kSourceRadio || other.Type == kSourceRadio)
            {
                if (Type == kSourceRadio && other.Type == kSourceRadio)
                {
                    return GroupName.CompareTo(other.GroupName);
                }
                return Type == kSourceRadio ? -1 : 1;
            }
            if (Type == kSourceReceiver || other.Type == kSourceReceiver)
            {
                if (Type == kSourceReceiver && other.Type == kSourceReceiver)
                {
                    return GroupName.CompareTo(other.GroupName);
                }
                return Type == kSourceReceiver ? -1 : 1;
            }
            if (GroupName != other.GroupName)
            {
                return GroupName.CompareTo(other.GroupName);
            }
            else return SourceName.CompareTo(other.SourceName);
        }

        #endregion

        #region Helper Methods

        internal Topology.ISource TopologySource
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iSource;
            }
        }

        internal virtual void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
        }

        internal string GroupName
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iSource.Group.Name;
            }
        }

        internal string SourceName
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iSource.Name;
            }
        }

        private delegate void DSetStandby(bool aStandby);
        internal bool Standby
        {
            set
            {
                Delegate del = new DSetStandby(delegate(bool aStandby)
                {
                    if (iSource.Standby != aStandby)
                    {
                        iSource.Standby = aStandby;
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        #endregion

        public override string ToString()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            return string.Format("Kinsky.Source: {0}:{1}", GroupName, SourceName);
        }

        private Topology.ISource iSource;
        private string iName;
        protected IInvoker iInvoker;
        protected Room iRoom;
        protected IModelFactory iModelFactory;
        protected Scheduler iScheduler;
    }


    public class AuxSource : Source
    {

        public AuxSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
            : base(aSource, aRoom, aInvoker, aModelFactory, aScheduler)
        { }

        #region Source Overrides

        public override void Seek(uint aSeconds)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void Pause()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void Play()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void Stop()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool Previous()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool Next()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool AllowSeeking
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override bool AllowSkipping
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override bool AllowSetTransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override bool AllowPlayNowNextLater
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override ETransportState TransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return ETransportState.ePlaying;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public override event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    aHandler.Invoke(this, EventArgs.Empty);
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        protected override void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

    }

    public class DiscSource : Source, IDiscSource
    {

        public DiscSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
            : base(aSource, aRoom, aInvoker, aModelFactory, aScheduler)
        {
            Open();
        }

        #region Source Overrides

        internal override void Close()
        {
            base.Close();
            CloseModel();
            iModelSource = null;
        }

        public override void Seek(uint aSeconds)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.SeekSeconds(aSeconds);
            }
        }

        public override void Pause()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Pause();
            }
        }

        public override void Play()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Play();
            }
        }

        public override void Stop()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Stop();
            }
        }

        public override bool Previous()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Previous();
                return true;
            }
            return false;
        }

        public override bool Next()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Next();
                return true;
            }
            return false;
        }

        public override void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool AllowSeeking
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSkipping
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSetTransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowPlayNowNextLater
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override ETransportState TransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    ModelSourceDiscPlayer.EPlayState playState = iModelSource.PlayState;
                    switch (playState)
                    {
                        case ModelSourceDiscPlayer.EPlayState.ePlaying:
                            return ETransportState.ePlaying;
                        case ModelSourceDiscPlayer.EPlayState.eSuspended:
                        case ModelSourceDiscPlayer.EPlayState.ePaused:
                            return ETransportState.ePaused;
                        case ModelSourceDiscPlayer.EPlayState.eStopped:
                            return ETransportState.eStopped;
                    }
                    return ETransportState.eUnknown;
                }
                return ETransportState.eUnknown;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public override event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        protected override void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IDiscSource Members

        public void Eject()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Eject();
            }
        }

        #endregion

        #region IPlayModeProvider Members

        public bool Shuffle
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.ProgramMode == ModelSourceDiscPlayer.EProgramMode.eShuffle;
                }
                return false;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    bool isShuffle = iModelSource.ProgramMode == ModelSourceDiscPlayer.EProgramMode.eShuffle;
                    if (isShuffle != value)
                    {
                        iModelSource.ToggleShuffle();
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventShuffleChanged;
        private void OnEventShuffleChanged()
        {
            EventHandler<EventArgs> eventHandler = EventShuffleChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public bool Repeat
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.RepeatMode == ModelSourceDiscPlayer.ERepeatMode.eAll;
                }
                return false;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    bool isRepeat = iModelSource.RepeatMode == ModelSourceDiscPlayer.ERepeatMode.eAll;
                    if (isRepeat != value)
                    {
                        iModelSource.ToggleRepeat();
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventRepeatChanged;
        protected void OnEventRepeatChanged()
        {
            EventHandler<EventArgs> eventHandler = EventRepeatChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Helper Methods

        private void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                iModelSource = iModelFactory.CreateModelSourceDiscPlayer(this.TopologySource);
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + this.Name + ", " + ex);
                iModelSource = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && !iModelOpen)
            {
                iModelSource.EventPlayStateChanged += EventPlayStateChangedHandler;
                iModelSource.EventRepeatModeChanged += EventRepeatModeChangedHandler;
                iModelSource.EventProgramModeChanged += EventProgramModeChangedHandler;
                iModelSource.EventInitialised += EventInitialisedHandler;
                iModelSource.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelSourceDiscPlayer model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && iModelOpen)
            {
                iModelSource.EventPlayStateChanged -= EventPlayStateChangedHandler;
                iModelSource.EventRepeatModeChanged -= EventRepeatModeChangedHandler;
                iModelSource.EventProgramModeChanged -= EventProgramModeChangedHandler;
                iModelSource.EventInitialised -= EventInitialisedHandler;
                iModelSource.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelSourceDiscPlayer model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelOpen = false;
                iModelInitialised = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModelSource != null)
                {
                    iRoom.House.RemoveDevice(iModelSource.Source.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventPlayStateChangedHandler();
        private void EventPlayStateChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventPlayStateChangedHandler(delegate()
            {
                OnEventTransportStateChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventProgramModeChangedHandler();
        private void EventProgramModeChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventProgramModeChangedHandler(delegate()
            {
                OnEventShuffleChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventRepeatModeChangedHandler();
        private void EventRepeatModeChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventRepeatModeChangedHandler(delegate()
            {
                OnEventRepeatChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventInitialisedHandler();
        private void EventInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        #endregion

        private IModelSourceDiscPlayer iModelSource;
        private bool iModelOpen;
        private bool iModelInitialised;
        private uint iSubscriptionCount;
    }

    public class PlaylistSource : Source, IPlaylistSource
    {

        public PlaylistSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
            : base(aSource, aRoom, aInvoker, aModelFactory, aScheduler)
        {
            iItems = new List<MrItem>();
            iCurrent = null;
            Open();
        }

        #region Source Overrides

        internal override void Close()
        {
            base.Close();
            CloseModel();
            iModelSource = null;
        }

        public override void Seek(uint aSeconds)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.SeekSeconds(aSeconds);
            }
        }

        public override void Pause()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Pause();
            }
        }

        public override void Play()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Play();
            }
        }

        public override void Stop()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Stop();
            }
        }

        public override bool Previous()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Previous();
                return true;
            }
            return false;
        }

        public override bool Next()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Next();
                return true;
            }
            return false;
        }

        public override void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            uint count = 0;
            IModelSourceMediaRenderer model = iModelSource;
            if (model != null)
            {
                // sync operation - move off invoker thread
                iScheduler.Schedule(() =>
                {
                    try
                    {
                        DidlLite didl = aRetriever.Media;

                        Trace.WriteLine(Trace.kKinsky, "PlayNow about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayNow about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                        count = model.PlayNow(aRetriever.Media);

                        Trace.WriteLine(Trace.kKinsky, "PlayNow inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayNow inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(Trace.kKinsky, "PlayNow error: " + ex);
                        UserLog.WriteLine("PlayNow error: " + ex);
                    }
                    finally
                    {
                        if (aCallback != null)
                        {
                            aCallback(count);
                        }
                    }
                });
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback(count);
                }
            }
        }

        public override void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            uint count = 0;
            IModelSourceMediaRenderer model = iModelSource;
            if (model != null)
            {
                // sync operation - move off invoker thread
                iScheduler.Schedule(() =>
                {
                    try
                    {
                        DidlLite didl = aRetriever.Media;

                        Trace.WriteLine(Trace.kKinsky, "PlayNext about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayNext about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                        count = model.PlayNext(aRetriever.Media);

                        Trace.WriteLine(Trace.kKinsky, "PlayNext inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayNext inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(Trace.kKinsky, "PlayNext error: " + ex);
                        UserLog.WriteLine("PlayNext error: " + ex);
                    }
                    finally
                    {
                        if (aCallback != null)
                        {
                            aCallback(count);
                        }
                    }
                });
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback(count);
                }
            }
        }

        public override void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            uint count = 0;
            IModelSourceMediaRenderer model = iModelSource;
            if (model != null)
            {
                // sync operation - move off invoker thread
                iScheduler.Schedule(() =>
                {
                    try
                    {
                        DidlLite didl = aRetriever.Media;

                        Trace.WriteLine(Trace.kKinsky, "PlayLater about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayLater about to insert " + didl.Count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                        count = model.PlayLater(aRetriever.Media);

                        Trace.WriteLine(Trace.kKinsky, "PlayLater inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("PlayLater inserted " + count + ((didl.Count == 1) ? " item" : " items") + " into playlist");

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(Trace.kKinsky, "PlayLater error: " + ex);
                        UserLog.WriteLine("PlayLater error: " + ex);
                    }
                    finally
                    {
                        if (aCallback != null)
                        {
                            aCallback(count);
                        }
                    }
                });
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback(count);
                }
            }
        }

        public override bool AllowSeeking
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSkipping
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSetTransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override ETransportState TransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {

                    ModelSourceMediaRenderer.ETransportState transportState = iModelSource.TransportState;

                    switch (transportState)
                    {
                        case ModelSourceMediaRenderer.ETransportState.eBuffering:
                            return ETransportState.eBuffering;
                        case ModelSourceMediaRenderer.ETransportState.ePlaying:
                            return ETransportState.ePlaying;
                        case ModelSourceMediaRenderer.ETransportState.ePaused:
                            return ETransportState.ePaused;
                        case ModelSourceMediaRenderer.ETransportState.eStopped:
                            return ETransportState.eStopped;
                    }
                    return ETransportState.eUnknown;
                }
                return ETransportState.eUnknown;
            }
        }

        public override bool AllowPlayNowNextLater
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public override event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iControlInitialised && iPlaylistInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        protected override void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }
        #endregion

        #region IPlaylistSource Members

        public void Insert(uint aInsertAfterId, IMediaRetriever aMediaRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            uint count = 0;
            IModelSourceMediaRenderer model = iModelSource;
            if (model != null)
            {
                // sync operation - move off invoker thread
                iScheduler.Schedule(() =>
                {
                    try
                    {
                        count = model.PlaylistInsert(aInsertAfterId, aMediaRetriever.Media);
                        Trace.WriteLine(Trace.kKinsky, "Inserted " + count + ((count == 1) ? " item" : " items") + " into playlist");
                        UserLog.WriteLine("Inserted " + count + ((count == 1) ? " item" : " items") + " into playlist");

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(Trace.kKinsky, "PlaylistInsert error: " + ex);
                        UserLog.WriteLine("PlaylistInsert error: " + ex);
                    }
                    finally
                    {
                        if (aCallback != null)
                        {
                            aCallback(count);
                        }
                    }
                });
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback(count);
                }
            }
        }

        public void Move(uint aInsertAfterId, IList<MrItem> aPlaylistItems, System.Action aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            IModelSourceMediaRenderer model = iModelSource;
            if (model != null)
            {
                // sync operation - move off invoker thread
                iScheduler.Schedule(() =>
                {
                    model.PlaylistMove(aInsertAfterId, aPlaylistItems);
                    if (aCallback != null)
                    {
                        aCallback();
                    }
                });
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback();
                }
            }
        }

        public void Delete(IList<MrItem> aPlaylistItems)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.PlaylistDelete(aPlaylistItems);
            }
        }

        public void DeleteAll()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.PlaylistDeleteAll();
            }
        }

        #endregion

        #region IPlayModeProvider Members

        public bool Shuffle
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.Shuffle;
                }
                return false;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    bool shuffle = iModelSource.Shuffle;
                    if (shuffle != value)
                    {
                        iModelSource.ToggleShuffle();
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventShuffleChanged;
        private void OnEventShuffleChanged()
        {
            EventHandler<EventArgs> eventHandler = EventShuffleChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public bool Repeat
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.Repeat;
                }
                return false;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    bool repeat = iModelSource.Repeat;
                    if (repeat != value)
                    {
                        iModelSource.ToggleRepeat();
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventRepeatChanged;

        private void OnEventRepeatChanged()
        {
            EventHandler<EventArgs> eventHandler = EventRepeatChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }
        #endregion

        #region IItemsProvider<MrItem> Members

        public ReadOnlyCollection<MrItem> Items
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return new ReadOnlyCollection<MrItem>(iItems);
            }
        }

        public MrItem Current
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iCurrent;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null && value != null)
                {
                    if (iItems.IndexOf(value) != -1)
                    {
                        iModelSource.SeekTrack((uint)iItems.IndexOf(value));
                    }
                }
            }
        }

        public event EventHandler<EventArgs> EventItemsChanged;
        protected void OnEventItemsChanged()
        {
            EventHandler<EventArgs> eventHandler = EventItemsChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventCurrentChanged;
        protected void OnEventCurrentChanged()
        {
            EventHandler<EventArgs> eventHandler = EventCurrentChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }


        #endregion

        #region Helper Methods


        private void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                iModelSource = iModelFactory.CreateModelSourceMediaRenderer(this.TopologySource);
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + this.Name + ", " + ex);
                iModelSource = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && !iModelOpen)
            {
                iModelSource.EventTransportStateChanged += EventTransportStateChangedHandler;
                iModelSource.EventRepeatChanged += EventRepeatChangedHandler;
                iModelSource.EventShuffleChanged += EventShuffleChangedHandler;
                iModelSource.EventControlInitialised += EventControlInitialisedHandler;
                iModelSource.EventPlaylistInitialised += EventPlaylistInitialisedHandler;
                iModelSource.EventPlaylistChanged += EventPlaylistChangedHandler;
                iModelSource.EventTrackChanged += EventTrackChangedHandler;
                iModelSource.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelSourceMediaRenderer model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && iModelOpen)
            {
                iModelSource.EventTransportStateChanged -= EventTransportStateChangedHandler;
                iModelSource.EventRepeatChanged -= EventRepeatChangedHandler;
                iModelSource.EventShuffleChanged -= EventShuffleChangedHandler;
                iModelSource.EventControlInitialised -= EventControlInitialisedHandler;
                iModelSource.EventPlaylistInitialised -= EventPlaylistInitialisedHandler;
                iModelSource.EventPlaylistChanged -= EventPlaylistChangedHandler;
                iModelSource.EventTrackChanged -= EventTrackChangedHandler;
                iModelSource.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelSourceMediaRenderer model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelOpen = false;
                iControlInitialised = false;
                iPlaylistInitialised = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModelSource != null)
                {
                    iRoom.House.RemoveDevice(iModelSource.Source.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventTrackChangedHandler();
        private void EventTrackChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventTrackChangedHandler(delegate()
            {
                if (iModelSource != null)
                {
                    iCurrent = iModelSource.TrackPlaylistItem;
                    OnEventCurrentChanged();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventPlaylistChangedHandler();
        private void EventPlaylistChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventPlaylistChangedHandler(delegate()
            {
                if (iModelSource != null)
                {
                    List<MrItem> list = new List<MrItem>();

                    try
                    {
                        iModelSource.Lock();
                        for (uint i = 0; i < iModelSource.PlaylistTrackCount; ++i)
                        {
                            list.Add(iModelSource.PlaylistItem(i));
                        }
                    }
                    finally
                    {
                        iModelSource.Unlock();
                    }
                    iItems = list;

                    OnEventItemsChanged();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventRepeatChangedHandler();
        private void EventRepeatChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventRepeatChangedHandler(delegate()
            {
                OnEventRepeatChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventShuffleChangedHandler();
        private void EventShuffleChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventShuffleChangedHandler(delegate()
            {
                OnEventShuffleChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventTransportStateChangedHandler();
        private void EventTransportStateChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventTransportStateChangedHandler(delegate()
            {
                OnEventTransportStateChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventControlInitialisedHandler();
        private void EventControlInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventControlInitialisedHandler(delegate()
            {
                if (!iControlInitialised)
                {
                    iControlInitialised = true;
                    if (iPlaylistInitialised)
                    {
                        OnEventOpened();
                    }
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventPlaylistInitialisedHandler();
        private void EventPlaylistInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventPlaylistInitialisedHandler(delegate()
            {
                if (!iPlaylistInitialised)
                {
                    iPlaylistInitialised = true;
                    if (iControlInitialised)
                    {
                        OnEventOpened();
                    }
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        #endregion

        private IModelSourceMediaRenderer iModelSource;
        private bool iModelOpen;
        private bool iControlInitialised;
        private bool iPlaylistInitialised;
        private uint iSubscriptionCount;
        private List<MrItem> iItems;
        private MrItem iCurrent;
    }

    public class RadioSource : Source, IRadioSource
    {

        public RadioSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, Scheduler aScheduler)
            : base(aSource, aRoom, aInvoker, aModelFactory, aScheduler)
        {
            iItems = new List<MrItem>();
            Open();
        }

        #region Source Overrides

        internal override void Close()
        {
            base.Close();
            CloseModel();
            iModelSource = null;
        }

        public override void Seek(uint aSeconds)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.SeekSeconds(aSeconds);
            }
        }

        public override void Pause()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Pause();
            }
        }

        public override void Play()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Play();
            }
        }

        public override void Stop()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Stop();
            }
        }

        public override bool Previous()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                return iModelSource.Previous();
            }
            return false;
        }

        public override bool Next()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                return iModelSource.Next();
            }
            return false;
        }

        public override void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool AllowSeeking
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSkipping
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override bool AllowSetTransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override ETransportState TransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {

                    ModelSourceRadio.ETransportState transportState = iModelSource.TransportState;

                    switch (transportState)
                    {
                        case ModelSourceRadio.ETransportState.eBuffering:
                            return ETransportState.eBuffering;
                        case ModelSourceRadio.ETransportState.ePlaying:
                            return ETransportState.ePlaying;
                        case ModelSourceRadio.ETransportState.eStopped:
                            return ETransportState.eStopped;
                    }
                    return ETransportState.eUnknown;
                }
                return ETransportState.eUnknown;
            }
        }


        public override bool AllowPlayNowNextLater
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public override event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        protected override void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IItemsProvider<MrItem> Members


        public ReadOnlyCollection<MrItem> Items
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return new ReadOnlyCollection<MrItem>(iItems);
            }
        }

        public MrItem Current
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iCurrent;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null && value != null)
                {
                    iModelSource.SetPreset(value);
                }
            }
        }

        public event EventHandler<EventArgs> EventCurrentChanged;
        private void OnEventCurrentChanged()
        {
            EventHandler<EventArgs> eventHandler = EventCurrentChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventItemsChanged;
        private void OnEventItemsChanged()
        {
            EventHandler<EventArgs> eventHandler = EventItemsChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion        

        #region IChannelSource Members

        public Channel Channel
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.Channel;
                }
                return null;
            }
        }

        public void SetChannel(DidlLite aDidlLite, bool aPlay)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                if (aPlay)
                {
                    iModelSource.PlayNow(aDidlLite);
                }else
                {
                    iModelSource.SetChannel(aDidlLite);
                }
            }
        }

        public event EventHandler<EventArgs> EventChannelChanged;
        protected void OnEventChannelChanged()
        {
            EventHandler<EventArgs> eventHandler = EventChannelChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Helper Methods


        private void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
        }

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                iModelSource = iModelFactory.CreateModelSourceRadio(this.TopologySource);
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + this.Name + ", " + ex);
                iModelSource = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && !iModelOpen)
            {
                iModelSource.EventTransportStateChanged += EventTransportStateChangedHandler;
                iModelSource.EventControlInitialised += EventControlInitialisedHandler;
                iModelSource.EventPresetsChanged += EventPresetsChangedHandler;
                iModelSource.EventPresetChanged += EventPresetChangedHandler;
                iModelSource.EventChannelChanged += EventChannelChangedHandler;
                iModelSource.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelSourceRadio model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }

        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && iModelOpen)
            {
                iModelSource.EventTransportStateChanged -= EventTransportStateChangedHandler;
                iModelSource.EventControlInitialised -= EventControlInitialisedHandler;
                iModelSource.EventPresetsChanged -= EventPresetsChangedHandler;
                iModelSource.EventPresetChanged -= EventPresetChangedHandler;
                iModelSource.EventChannelChanged -= EventChannelChangedHandler;
                iModelSource.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelSourceRadio model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelOpen = false;
                iModelInitialised = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModelSource != null)
                {
                    iRoom.House.RemoveDevice(iModelSource.Source.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventTransportStateChangedHandler();
        private void EventTransportStateChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventTransportStateChangedHandler(delegate()
            {
                OnEventTransportStateChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventControlInitialisedHandler();
        private void EventControlInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventControlInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventPresetChangedHandler();
        private void EventPresetChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventPresetChangedHandler(delegate()
            {
                if (iModelSource != null)
                {
                    if (iModelSource.PresetIndex >= 0)
                    {
                        RefreshItemsList();
                        iCurrent = iModelSource.Preset((uint)iModelSource.PresetIndex);
                    }
                    else
                    {
                        iCurrent = null;
                    }
                    OnEventCurrentChanged();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventChannelChangedHandler();
        private void EventChannelChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventChannelChangedHandler(delegate()
            {
                OnEventChannelChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventPresetsChangedHandler();
        private void EventPresetsChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventPresetChangedHandler(delegate()
            {
                if (iModelSource != null)
                {
                    RefreshItemsList();
                    OnEventItemsChanged();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private void RefreshItemsList()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            List<MrItem> list = new List<MrItem>();

            iModelSource.Lock();
            bool ignoreEmptyItems = true;
            for (uint i = 0; i < ModelSourceRadio.kMaxPresets; ++i)
            {
                MrItem item = iModelSource.Preset(ModelSourceRadio.kMaxPresets - i - 1);
                if (!ignoreEmptyItems || item != RadioIdArray.kEmptyPreset)
                {
                    list.Insert(0, item);
                    ignoreEmptyItems = false;
                }
            }
            iModelSource.Unlock();
            iItems = list;
        }

        #endregion

        private IModelSourceRadio iModelSource;
        private bool iModelOpen;
        private bool iModelInitialised;
        private uint iSubscriptionCount;
        private List<MrItem> iItems;
        private MrItem iCurrent;
    }

    public class ReceiverSource : Source, IReceiverSource
    {

        public ReceiverSource(Topology.ISource aSource, Room aRoom, IInvoker aInvoker, IModelFactory aModelFactory, IModelSenders aModelSenders, Scheduler aScheduler)
            : base(aSource, aRoom, aInvoker, aModelFactory, aScheduler)
        {
            iModelSenders = aModelSenders;
            Open();
        }

        #region Source Overrides

        public override void Seek(uint aSeconds)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void Pause()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void Play()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Play();
            }
        }

        public override void Stop()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                iModelSource.Stop();
            }
        }

        public override bool Previous()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            return false;
        }

        public override bool Next()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            return false;
        }

        public override void PlayNow(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayNext(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override void PlayLater(IMediaRetriever aRetriever, Action<uint> aCallback)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            throw new InvalidOperationException();
        }

        public override bool AllowSeeking
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override bool AllowSkipping
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        public override bool AllowSetTransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return true;
            }
        }

        public override ETransportState TransportState
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {

                    ModelSourceReceiver.ETransportState transportState = iModelSource.TransportState;

                    switch (transportState)
                    {
                        case ModelSourceReceiver.ETransportState.eBuffering:
                            return ETransportState.eBuffering;
                        case ModelSourceReceiver.ETransportState.ePlaying:
                            return ETransportState.ePlaying;
                        case ModelSourceReceiver.ETransportState.eStopped:
                            return ETransportState.eStopped;
                        case ModelSourceReceiver.ETransportState.eWaiting:
                            return ETransportState.eWaiting;
                    }
                    return ETransportState.eUnknown;
                }
                return ETransportState.eUnknown;
            }
        }

        public override bool AllowPlayNowNextLater
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return false;
            }
        }

        private delegate void DEventOpened(EventHandler<EventArgs> aHandler);
        private EventHandler<EventArgs> iEventOpened;
        public override event EventHandler<EventArgs> EventOpened
        {
            add
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened += aHandler;
                    iSubscriptionCount += 1;
                    if (iSubscriptionCount == 1)
                    {
                        OpenModel();
                    }
                    else if (iModelInitialised)
                    {
                        aHandler.Invoke(this, EventArgs.Empty);
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
            remove
            {
                Delegate del = new DEventOpened(delegate(EventHandler<EventArgs> aHandler)
                {
                    iEventOpened -= aHandler;
                    iSubscriptionCount -= 1;
                    if (iSubscriptionCount == 0)
                    {
                        CloseModel();
                    }
                });
                if (iInvoker.TryBeginInvoke(del, value))
                    return;
                del.Method.Invoke(del.Target, new object[] { value });
            }
        }

        protected override void OnEventOpened()
        {
            EventHandler<EventArgs> eventHandler = iEventOpened;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IItemsProvider<IModelSender> Members


        public ReadOnlyCollection<IModelSender> Items
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iItems;
            }
        }

        public IModelSender Current
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                return iCurrent;
            }
            set
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                Assert.Check(value != null && value.Metadata != null);
                SetChannel(value.Metadata, false);
            }
        }

        public event EventHandler<EventArgs> EventCurrentChanged;
        private void OnEventCurrentChanged()
        {
            EventHandler<EventArgs> eventHandler = EventCurrentChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventItemsChanged;
        private void OnEventItemsChanged()
        {
            EventHandler<EventArgs> eventHandler = EventItemsChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IChannelSource Members

        public Channel Channel
        {
            get
            {
                if (iInvoker.InvokeRequired) { throw new InvocationException(); }
                if (iModelSource != null)
                {
                    return iModelSource.Channel;
                }
                return null;
            }
        }

        public void SetChannel(DidlLite aDidlLite, bool aPlay)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null)
            {
                if (aPlay)
                {
                    iModelSource.PlayNow(aDidlLite);
                }
                else
                {
                    iModelSource.SetChannel(aDidlLite);
                }
            }
        }


        public event EventHandler<EventArgs> EventChannelChanged;
        private void OnEventChannelChanged()
        {
            EventHandler<EventArgs> eventHandler = EventChannelChanged;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Helper Methods

        private void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            CreateModel();
            iModelSenders.EventSenderAdded += EventSenderAddedHandler;
            iModelSenders.EventSenderRemoved += EventSenderRemovedHandler;
            iModelSenders.EventSenderChanged += EventSenderChangedHandler;
        }

        internal override void Close()
        {
            base.Close();
            CloseModel();
            iModelSource = null;
            iModelSenders.EventSenderAdded -= EventSenderAddedHandler;
            iModelSenders.EventSenderRemoved -= EventSenderRemovedHandler;
            iModelSenders.EventSenderChanged -= EventSenderChangedHandler;
        }

        private void SetSenders()
        {
            IList<IModelSender> senders = iModelSenders.SendersList;
            List<IModelSender> result = new List<IModelSender>();
            foreach (IModelSender model in senders)
            {
                if (model.Udn != this.Udn)
                {
                    result.Add(model);
                }
            }
            iItems = result.AsReadOnly();
            OnEventItemsChanged();
        }

        private void SetChannel()
        {
            IList<IModelSender> senders = iModelSenders.SendersList;
            Channel channel = null;
            if (iModelSource != null)
            {
                channel = iModelSource.Channel;
            }
            IModelSender found = null;
            foreach (IModelSender model in senders)
            {
                if (model.Udn != this.Udn)
                {
                    if (channel != null && model.Metadata[0].Title == channel.DidlLite[0].Title)
                    {
                        found = model;
                    }
                }
            }
            if (iCurrent != found)
            {
                iCurrent = found;
                OnEventCurrentChanged();
            }
        }

        private void CreateModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSubscriptionCount > 0)
            {
                CloseModel();
            }
            try
            {
                iModelSource = iModelFactory.CreateModelSourceReceiver(this.TopologySource);
            }
            catch (ModelSourceException ex)
            {
                UserLog.WriteLine("ModelServiceException caught: " + this.Name + ", " + ex);
                iModelSource = null;
            }
            if (iSubscriptionCount > 0)
            {
                OpenModel();
            }
        }

        private void OpenModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && !iModelOpen)
            {
                iModelSource.EventTransportStateChanged += EventTransportStateChangedHandler;
                iModelSource.EventControlInitialised += EventControlInitialisedHandler;
                iModelSource.EventChannelChanged += EventChannelChangedHandler;
                iModelSource.EventSubscriptionError += EventSubscriptionErrorHandler;
                IModelSourceReceiver model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Open();
                }));
                iModelOpen = true;
            }
        }
        private void CloseModel()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iModelSource != null && iModelOpen)
            {
                iModelSource.EventTransportStateChanged -= EventTransportStateChangedHandler;
                iModelSource.EventControlInitialised -= EventControlInitialisedHandler;
                iModelSource.EventChannelChanged -= EventChannelChangedHandler;
                iModelSource.EventSubscriptionError -= EventSubscriptionErrorHandler;
                IModelSourceReceiver model = iModelSource;
                iScheduler.Schedule(new Scheduler.DCallback(() =>
                {
                    model.Close();
                }));
                iModelOpen = false;
                iModelInitialised = false;
            }
            OnEventClosed();
        }

        private delegate void DEventSubscriptionErrorHandler();
        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventSubscriptionErrorHandler(delegate()
            {
                if (iModelSource != null)
                {
                    iRoom.House.RemoveDevice(iModelSource.Source.Device);
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventSenderAddedHandler();
        private void EventSenderAddedHandler(object sender, ModelSenders.EventArgsSender e)
        {
            Delegate del = new DEventSenderAddedHandler(delegate()
            {
                SetSenders();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventSenderRemovedHandler();
        private void EventSenderRemovedHandler(object sender, ModelSenders.EventArgsSender e)
        {
            Delegate del = new DEventSenderRemovedHandler(delegate()
            {
                SetSenders();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventSenderChangedHandler();
        private void EventSenderChangedHandler(object sender, ModelSenders.EventArgsSender e)
        {
            Delegate del = new DEventSenderChangedHandler(delegate()
            {
                SetSenders();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventChannelChangedHandler();
        private void EventChannelChangedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventChannelChangedHandler(delegate()
            {
                SetChannel();
                OnEventChannelChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventControlInitialisedHandler();
        private void EventControlInitialisedHandler(object sender, EventArgs args)
        {
            Delegate del = new DEventControlInitialisedHandler(delegate()
            {
                if (!iModelInitialised)
                {
                    iModelInitialised = true;
                    SetChannel();
                    SetSenders();
                    OnEventOpened();
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        private delegate void DEventTransportStateChangedHandler();
        private void EventTransportStateChangedHandler(object sender, EventArgs e)
        {
            Delegate del = new DEventTransportStateChangedHandler(delegate()
            {
                OnEventTransportStateChanged();
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        #endregion

        private IModelSourceReceiver iModelSource;
        private bool iModelOpen;
        private bool iModelInitialised;
        private uint iSubscriptionCount;
        private IModelSenders iModelSenders;
        private ReadOnlyCollection<IModelSender> iItems;
        private IModelSender iCurrent;
    }



    public class EventArgsPlay : EventArgs
    {
        public EventArgsPlay(IMediaRetriever aRetriever)
        {
            Retriever = aRetriever;
        }

        public EventArgsPlay(IMediaRetriever aRetriever, Action<uint> aCallback)
            : this(aRetriever)
        {
            Callback = aCallback;
        }

        public IMediaRetriever Retriever;
        public Action<uint> Callback;
    }

    public class EventArgsInsert : EventArgs
    {
        public EventArgsInsert(uint aInsertAfterId, IMediaRetriever aRetriever)
        {
            InsertAfterId = aInsertAfterId;
            Retriever = aRetriever;
        }

        public EventArgsInsert(uint aInsertAfterId, IMediaRetriever aRetriever, Action<uint> aCallback)
            : this(aInsertAfterId, aRetriever)
        {
            Callback = aCallback;
        }

        public uint InsertAfterId;
        public IMediaRetriever Retriever;
        public Action<uint> Callback;
    }

    public class EventArgsMove : EventArgs
    {
        public EventArgsMove(uint aInsertAfterId, IList<MrItem> aMoveItems, System.Action aCallback)
        {
            InsertAfterId = aInsertAfterId;
            MoveItems = aMoveItems;
            Callback = aCallback;
        }

        public uint InsertAfterId;
        public IList<MrItem> MoveItems;
        public System.Action Callback;
    }

    public interface IPlaylistSupport
    {
        void Open();
        void Close();

        bool IsOpen();
        bool IsInserting();
        bool IsDragging();
        bool IsInsertAllowed();

        void SetDragging(bool aDragging);
        void SetInsertAllowed(bool aInsertAllowed);

        void PlayNow(IMediaRetriever aMediaRetriever);
        void PlayNext(IMediaRetriever aMediaRetriever);
        void PlayLater(IMediaRetriever aMediaRetriever);
        void PlayInsert(uint aInsertAfterId, IMediaRetriever aMediaRetriever);

        void Move(uint aInsertAfterId, IList<MrItem> aPlaylistItems);

        event EventHandler<EventArgs> EventIsOpenChanged;
        event EventHandler<EventArgs> EventIsInsertingChanged;
        event EventHandler<EventArgs> EventIsDraggingChanged;
        event EventHandler<EventArgs> EventIsInsertAllowedChanged;

        event EventHandler<EventArgsPlay> EventPlayNow;
        event EventHandler<EventArgsPlay> EventPlayNext;
        event EventHandler<EventArgsPlay> EventPlayLater;
        event EventHandler<EventArgsInsert> EventPlayInsert;
        event EventHandler<EventArgsMove> EventMove;
    }

    public interface IModel
    {
        IModelWidgetSelector<Room> ModelWidgetSelectorRoom { get; }
        IViewWidgetButton ModelWidgetButtonStandby { get; }
        IModelWidgetSelector<Source> ModelWidgetSelectorSource { get; }
        IViewWidgetVolumeControl ModelWidgetVolumeControl { get; }
        IViewWidgetMediaTime ModelWidgetMediaTime { get; }
        IViewWidgetTransportControl ModelWidgetTransportControlMediaRenderer { get; }
        IViewWidgetTransportControl ModelWidgetTransportControlDiscPlayer { get; }
        IViewWidgetTransportControl ModelWidgetTransportControlRadio { get; }
        IViewWidgetTrack ModelWidgetTrack { get; }
        IViewWidgetPlayMode ModelWidgetPlayMode { get; }
        IViewWidgetPlaylist ModelWidgetPlaylist { get; }
        IViewWidgetPlaylistRadio ModelWidgetPlaylistRadio { get; }
        IViewWidgetPlaylistReceiver ModelWidgetPlaylistReceiver { get; }
        IViewWidgetPlaylistAux ModelWidgetPlaylistAux { get; }
        IViewWidgetPlaylistDiscPlayer ModelWidgetPlaylistDiscPlayer { get; }
        IViewWidgetButton ModelWidgetButtonSave { get; }
        IViewWidgetButton ModelWidgetButtonWasteBin { get; }
        IViewWidgetReceivers ModelWidgetReceivers { get; }
        IViewWidgetButton ModelWidgetButtonReceivers { get; }
        IPlaylistSupport ModelSupport { get; }
    }

    public class Model : IModel
    {
        public Model(IView aView, IPlaylistSupport aSupport)
        {
            iView = aView;
            iSupport = aSupport;
            iModelSelectorRoom = new ModelWidgetSelector<Room>(aView.ViewWidgetSelectorRoom);
            iModelSelectorSource = new ModelWidgetSelector<Source>(aView.ViewWidgetSelectorSource);
        }

        public IModelWidgetSelector<Room> ModelWidgetSelectorRoom
        {
            get
            {
                return iModelSelectorRoom;
            }
        }

        public IViewWidgetButton ModelWidgetButtonStandby
        {
            get
            {
                return iView.ViewWidgetButtonStandby;
            }
        }

        public IModelWidgetSelector<Source> ModelWidgetSelectorSource
        {
            get
            {
                return iModelSelectorSource;
            }
        }

        public IViewWidgetVolumeControl ModelWidgetVolumeControl
        {
            get
            {
                return iView.ViewWidgetVolumeControl;
            }
        }

        public IViewWidgetMediaTime ModelWidgetMediaTime
        {
            get
            {
                return iView.ViewWidgetMediaTime;
            }
        }

        public IViewWidgetTransportControl ModelWidgetTransportControlMediaRenderer
        {
            get
            {
                return iView.ViewWidgetTransportControlMediaRenderer;
            }
        }

        public IViewWidgetTransportControl ModelWidgetTransportControlDiscPlayer
        {
            get
            {
                return iView.ViewWidgetTransportControlDiscPlayer;
            }
        }

        public IViewWidgetTransportControl ModelWidgetTransportControlRadio
        {
            get
            {
                return iView.ViewWidgetTransportControlRadio;
            }
        }

        public IViewWidgetTrack ModelWidgetTrack
        {
            get
            {
                return iView.ViewWidgetTrack;
            }
        }

        public IViewWidgetPlayMode ModelWidgetPlayMode
        {
            get
            {
                return iView.ViewWidgetPlayMode;
            }
        }

        public IViewWidgetPlaylist ModelWidgetPlaylist
        {
            get
            {
                return iView.ViewWidgetPlaylist;
            }
        }

        public IViewWidgetPlaylistRadio ModelWidgetPlaylistRadio
        {
            get
            {
                return iView.ViewWidgetPlaylistRadio;
            }
        }

        public IViewWidgetPlaylistReceiver ModelWidgetPlaylistReceiver
        {
            get
            {
                return iView.ViewWidgetPlaylistReceiver;
            }
        }

        public IViewWidgetPlaylistAux ModelWidgetPlaylistAux
        {
            get
            {
                return iView.ViewWidgetPlaylistAux;
            }
        }

        public IViewWidgetPlaylistDiscPlayer ModelWidgetPlaylistDiscPlayer
        {
            get
            {
                return iView.ViewWidgetPlaylistDiscPlayer;
            }
        }

        public IViewWidgetButton ModelWidgetButtonSave
        {
            get
            {
                return iView.ViewWidgetButtonSave;
            }
        }

        public IViewWidgetButton ModelWidgetButtonWasteBin
        {
            get
            {
                return iView.ViewWidgetButtonWasteBin;
            }
        }

        public IViewWidgetReceivers ModelWidgetReceivers
        {
            get
            {
                return iView.ViewWidgetReceivers;
            }
        }

        public IViewWidgetButton ModelWidgetButtonReceivers
        {
            get
            {
                return iView.ViewWidgetButtonReceivers;
            }
        }

        public IPlaylistSupport ModelSupport
        {
            get
            {
                return iSupport;
            }
        }

        private IView iView;
        private IPlaylistSupport iSupport;
        private IModelWidgetSelector<Room> iModelSelectorRoom;
        private IModelWidgetSelector<Source> iModelSelectorSource;
    }

    public interface IModelWidgetSelector<T>
    {
        void Open();
        void Close();

        void InsertItem(int aIndex, T aItem);
        void RemoveItem(T aItem);
        void ItemChanged(T aItem);

        void SetSelected(T aItem);
        T Selected { get; }

        event EventHandler<EventArgs> EventSelectionChanged;
    }

    public class ModelWidgetSelector<T> : IModelWidgetSelector<T> where T : class
    {
        public ModelWidgetSelector(IViewWidgetSelector<T> aView)
        {
            iView = aView;
        }

        public void Open()
        {
            iView.EventSelectionChanged += EventSelectionChangedResponse;

            iView.Open();
        }

        public void Close()
        {
            iView.Close();

            iView.EventSelectionChanged -= EventSelectionChangedResponse;

            SetSelected(null);
            iSelected = null;
        }

        public void InsertItem(int aIndex, T aItem)
        {
            iView.InsertItem(aIndex, aItem);
        }

        public void RemoveItem(T aItem)
        {
            iView.RemoveItem(aItem);

            if (aItem == iSelected)
            {
                EventSelectionChangedResponse(this, new EventArgsSelection<T>(null));
            }
        }

        public void ItemChanged(T aItem)
        {
            iView.ItemChanged(aItem);
        }

        public void SetSelected(T aItem)
        {
            Trace.WriteLine(Trace.kKinsky, "ModelWidgetSelector.SetSelected: " + aItem);
            iView.SetSelected(aItem);
        }

        public T Selected
        {
            get
            {
                return iSelected;
            }
        }

        public event EventHandler<EventArgs> EventSelectionChanged;

        private void EventSelectionChangedResponse(object sender, EventArgsSelection<T> e)
        {
            Trace.WriteLine(Trace.kKinsky, "ModelWidgetSelector.EventSelectionChangedResponse: " + e.Tag);

            iSelected = e.Tag;

            if (EventSelectionChanged != null)
            {
                EventSelectionChanged(this, EventArgs.Empty);
            }
        }

        private IViewWidgetSelector<T> iView;

        private T iSelected;
    }

} // Linn.Kinsky
