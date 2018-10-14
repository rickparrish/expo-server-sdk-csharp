using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace expo_server_sdk_csharp {
    public class Message {
        /// <summary>
        /// An Expo push token specifying the recipient of this message.
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// A JSON object delivered to your app. It may be up to about 4KiB; the total
        /// notification payload sent to Apple and Google must be at most 4KiB or else
        /// you will get a "Message Too Big" error.
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// The title to display in the notification. Devices often display this in
        /// bold above the notification body.Only the title might be displayed on
        /// devices with smaller screens like Apple Watch.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The message to display in the notification
        /// </summary>
        public string body { get; set; }

        /// <summary>
        /// Time to Live: the number of seconds for which the message may be kept
        /// around for redelivery if it hasn't been delivered yet. Defaults to 0.
        ///
        /// On Android, we make a best effort to deliver messages with zero TTL
        /// immediately and do not throttle them
        ///
        /// This field takes precedence over `expiration` when both are specified.
        /// </summary>
        public int? ttl { get; set; }

        /// <summary>
        /// A timestamp since the UNIX epoch specifying when the message expires. This
        /// has the same effect as the `ttl` field and is just an absolute timestamp
        /// instead of a relative time.
        /// </summary>
        public int? expiration { get; set; }

        /// <summary>
        /// The delivery priority of the message. Specify "default" or omit this field
        /// to use the default priority on each platform, which is "normal" on Android
        /// and "high" on iOS.
        ///
        /// On Android, normal-priority messages won't open network connections on
        /// sleeping devices and their delivery may be delayed to conserve the battery.
        /// High-priority messages are delivered immediately if possible and may wake
        /// sleeping devices to open network connections, consuming energy.
        ///
        /// On iOS, normal-priority messages are sent at a time that takes into account
        /// power considerations for the device, and may be grouped and delivered in
        /// bursts. They are throttled and may not be delivered by Apple. High-priority
        /// messages are sent immediately.Normal priority corresponds to APNs priority
        /// level 5 and high priority to 10.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Priority? priority { get; set; }

        /// <summary>
        /// A sound to play when the recipient receives this notification. Specify
        /// "default" to play the device's default notification sound, or omit this
        /// field to play no sound.
        ///
        /// Note that on apps that target Android 8.0+ (if using `expo build`, built
        /// in June 2018 or later), this setting will have no effect on Android.
        /// Instead, use `channelId` and a channel with the desired setting.
        /// 
        /// iOS Specific
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Sound? sound { get; set; }

        /// <summary>
        /// Number to display in the badge on the app icon. Specify zero to clear the
        /// badge.
        /// 
        /// iOS Specific
        /// </summary>
        public int? badge { get; set; }

        /// <summary>
        /// ID of the Notification Channel through which to display this notification
        /// on Android devices.If an ID is specified but the corresponding channel
        /// does not exist on the device(i.e.has not yet been created by your app),
        /// the notification will not be displayed to the user.
        ///
        /// If left null, a "Default" channel will be used, and Expo will create the
        /// channel on the device if it does not yet exist.However, use caution, as
        /// the "Default" channel is user-facing and you may not be able to fully
        /// delete it.
        /// </summary>
        public string channelId { get; set; }
    }
}
