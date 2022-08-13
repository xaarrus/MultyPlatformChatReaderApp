using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MultyPlatformChatReaderApp.Models.WASD
{
    public class WASDModels
    {
        public class WASDUserLocalInfo
        {
            public Brush StreamStatus { get; set; } = new SolidColorBrush(Colors.Gray);
            public string AvatarLink { get; set; } = "pack://application:,,,/Data/img/app-anonavatar.png";
            public string NameOnWASD { get; set; } = "";
            public string WASDName { get; set; } = "";
            public string GameName { get; set; } = "Game not set or WASD not response";
            public string StreamTitle { get; set; } = "";
            public int CountViewers { get; set; } = 0;
        }
        public class ResponseV2BroadcastsPublic
        {
            public Result result { get; set; } = new();
            public class Channel
            {
                public DateTime created_at { get; set; } = DateTime.MinValue;
                public DateTime updated_at { get; set; } = DateTime.MinValue;
                //public DateTime deleted_at { get; set; } = DateTime.MinValue;
                public int channel_id { get; set; }
                public string channel_name { get; set; } = String.Empty;
                public int user_id { get; set; }
                public int followers_count { get; set; }
                public int channel_subscribers_count { get; set; }
                public bool channel_is_live { get; set; }
                public string channel_description { get; set; } = String.Empty;
                public bool channel_description_enabled { get; set; }
                public string channel_donation_url { get; set; } = String.Empty;
                public ChannelImage channel_image { get; set; } = new();
                public string channel_status { get; set; } = String.Empty;
                public bool notification { get; set; }
                public bool channel_subscription_seller { get; set; }
                public ChannelOwner channel_owner { get; set; } = new();
                public string channel_alias { get; set; } = String.Empty;
                public int channel_priority { get; set; }
                public int channel_clips_count { get; set; }
                public bool premoderation_enabled { get; set; }
                public DateTime last_activity_date { get; set; }
                public RaidInfo raid_info { get; set; } = new();
                public Meta meta { get; set; } = new();
                public bool is_user_follower { get; set; }
                public bool is_partner { get; set; }
            }

            public class ChannelImage
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class ChannelOwner
            {
                public DateTime created_at { get; set; } = DateTime.MinValue;
                public DateTime updated_at { get; set; } = DateTime.MinValue;
                //public DateTime deleted_at { get; set; } = DateTime.MinValue;
                public int user_id { get; set; }
                public string user_login { get; set; } = String.Empty;
                public int channel_id { get; set; }
                public string profile_description { get; set; } = String.Empty;
                public string profile_stream_view_url { get; set; } = String.Empty;
                public string profile_stream_preview_image_url { get; set; } = String.Empty;
                public bool profile_is_live { get; set; }
                public ProfileImage profile_image { get; set; } = new();
                public ProfileBackground profile_background { get; set; } = new();
            }

            public class Game
            {
                public int game_id { get; set; }
                public string game_name { get; set; } = String.Empty;
                public GameIcon game_icon { get; set; } = new();
                public string game_color_hex { get; set; } = String.Empty;
            }

            public class GameIcon
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class MediaContainer
            {
                public int media_container_id { get; set; }
                public string media_container_name { get; set; } = String.Empty;
                public string media_container_description { get; set; } = String.Empty;
                public string media_container_type { get; set; } = String.Empty;
                public string media_container_status { get; set; } = String.Empty;
                public string media_container_online_status { get; set; } = String.Empty;
                public int user_id { get; set; }
                public int channel_id { get; set; }
                public DateTime created_at { get; set; }
                public Game game { get; set; } = new();
                public List<MediaContainerStream> media_container_streams { get; set; } = new();
                public List<Tag> tags { get; set; } = new();
                public bool is_mature_content { get; set; }
                public DateTime published_at { get; set; }
            }

            public class MediaContainerStream
            {
                public int stream_id { get; set; }
                public int stream_total_viewers { get; set; }
                public int stream_current_viewers { get; set; }
                public int stream_current_active_viewers { get; set; }
                public List<StreamMedium> stream_media { get; set; } = new();
            }

            public class MediaMeta
            {
                public string media_archive_url { get; set; } = String.Empty;
                public int media_current_bitrate { get; set; }
                public int media_current_fps { get; set; }
                public string media_current_resolution { get; set; } = String.Empty;
                public string media_url { get; set; } = String.Empty;
                public string media_preview_url { get; set; } = String.Empty;
                public string media_preview_archive_url { get; set; } = String.Empty;
                public MediaPreviewArchiveImages media_preview_archive_images { get; set; } = new();
                public MediaPreviewImages media_preview_images { get; set; } = new();
            }

            public class MediaPreviewArchiveImages
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class MediaPreviewImages
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class Meta
            {
                public bool required_isso { get; set; }
            }

            public class ProfileBackground
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class ProfileImage
            {
                public string large { get; set; } = String.Empty;
                public string medium { get; set; } = String.Empty;
                public string small { get; set; } = String.Empty;
            }

            public class RaidInfo
            {
                public string begin_at { get; set; } = String.Empty;
                public string expire_at { get; set; } = String.Empty;
                public string channel_name { get; set; } = String.Empty;
                public int raid_channel_id { get; set; }
                public int raid_mc_id { get; set; }
            }

            public class Result
            {
                public Channel channel { get; set; } = new();
                public MediaContainer media_container { get; set; } = new();
                public bool need_isso { get; set; }
                public bool not_simple_auth_flow { get; set; }
            }

            public class StreamMedium
            {
                public int media_id { get; set; }
                public string media_type { get; set; } = String.Empty;
                public MediaMeta media_meta { get; set; } = new();
                public int media_duration { get; set; }
                public string media_status { get; set; } = String.Empty;
            }

            public class Tag
            {
                public int tag_id { get; set; }
                public string tag_name { get; set; } = String.Empty;
                public string tag_description { get; set; } = String.Empty;
                public TagMeta tag_meta { get; set; } = new();
                public string tag_type { get; set; } = String.Empty;
                public int tag_media_containers_online_count { get; set; }
            }

            public class TagMeta
            {
            }
        }
        public class ResponseChatToken
        {
            public string result { get; set; } = String.Empty;
        }
    }
}
