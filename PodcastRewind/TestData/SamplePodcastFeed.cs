// ReSharper disable StringLiteralTypo

namespace PodcastRewind.TestData;

internal static partial class Data
{
    public static string SamplePodcastFeed =>
        """
        <?xml version="1.0" encoding="utf-8"?>
        <?xml-stylesheet type="text/xsl" href="/global/feed/rss.xslt" ?>
        <rss version="2.0"
        	xmlns:atom="http://www.w3.org/2005/Atom"
        	xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd"
        	xmlns:content="http://purl.org/rss/1.0/modules/content/">
        	<channel>
        		<ttl>60</ttl>
        		<generator>Podcast Rewind Test Feed</generator>
        		<title>Podcast Rewind Test Podcast</title>
        		<link>https://github.com/dougwaldron/podcast-rewind</link>
        		<atom:link href="https://github.com/dougwaldron/podcast-rewind" rel="self" type="application/rss+xml"/>
        		<language>en</language>
        		<itunes:summary><![CDATA[<p>This is a summary of the test podcast feed for the Podcast Rewind app.</p>]]></itunes:summary>
        		<description><![CDATA[<p>This is the description of the test podcast feed for the Podcast Rewind app.</p>]]></description>
        		<itunes:explicit>no</itunes:explicit>
        		<itunes:type>serial</itunes:type>
        		<itunes:image href="https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=640&amp;h=640&amp;auto=format&amp;fit=crop"/>
        		<image>
        			<url>data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gVXBsb2FkZWQgdG86IFNWRyBSZXBvLCB3d3cuc3ZncmVwby5jb20sIEdlbmVyYXRvcjogU1ZHIFJlcG8gTWl4ZXIgVG9vbHMgLS0+DQo8c3ZnIHdpZHRoPSI4MDBweCIgaGVpZ2h0PSI4MDBweCIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgY2xhc3M9Imljb24iICB2ZXJzaW9uPSIxLjEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PHBhdGggZD0iTTY0Ny42OCAyMTIuNDhjMC03MC40LTYyLjcyLTEyOC0xNDAuOC0xMjhzLTE0MC44IDU3LjYtMTQwLjggMTI4djI4MS42YzAgNzAuNCA2Mi43MiAxMjggMTQwLjggMTI4czE0MC44LTU3LjYgMTQwLjgtMTI4di0yODEuNnoiIGZpbGw9IiNGMkU1Q0EiIC8+PHBhdGggZD0iTTUwNi44OCA2MzQuODhjLTg0LjQ4IDAtMTUzLjYtNjIuNzItMTUzLjYtMTQwLjh2LTI4MS42YzAtNzguMDggNjkuMTItMTQwLjggMTUzLjYtMTQwLjhzMTUzLjYgNjIuNzIgMTUzLjYgMTQwLjh2MjgxLjZjMCA3OC4wOC02OS4xMiAxNDAuOC0xNTMuNiAxNDAuOHogbTAtNTM3LjZjLTcwLjQgMC0xMjggNTEuMi0xMjggMTE1LjJ2MjgxLjZjMCA2NCA1Ny42IDExNS4yIDEyOCAxMTUuMnMxMjgtNTEuMiAxMjgtMTE1LjJ2LTI4MS42YzAtNjQtNTcuNi0xMTUuMi0xMjgtMTE1LjJ6IiBmaWxsPSIjMjMxQzFDIiAvPjxwYXRoIGQ9Ik01MDguMTYgNjk4Ljg4Yy0xMjAuMzIgMC0yMTcuNi04Ny4wNC0yMTcuNi0xOTQuNTZWMzY3LjM2aDI1LjZ2MTM1LjY4YzAgOTMuNDQgODUuNzYgMTY4Ljk2IDE5MiAxNjguOTZzMTkyLTc1LjUyIDE5Mi0xNjguOTZWMzY3LjM2aDI1LjZ2MTM1LjY4YzAgMTA4LjgtOTcuMjggMTk1Ljg0LTIxNy42IDE5NS44NHoiIGZpbGw9IiMyMzFDMUMiIC8+PHBhdGggZD0iTTM2Ni4wOCAzMDIuMDhoMjgxLjZ2ODkuNmgtMjgxLjZ6IiBmaWxsPSIjRUVCRTAwIiAvPjxwYXRoIGQ9Ik02NDcuNjggNDA0LjQ4aC0yODEuNmMtNy42OCAwLTEyLjgtNS4xMi0xMi44LTEyLjh2LTg5LjZjMC03LjY4IDUuMTItMTIuOCAxMi44LTEyLjhoMjgxLjZjNy42OCAwIDEyLjggNS4xMiAxMi44IDEyLjh2ODkuNmMwIDYuNC01LjEyIDEyLjgtMTIuOCAxMi44eiBtLTI2OC44LTI1LjZoMjU2di02NGgtMjU2djY0eiIgZmlsbD0iIzIzMUMxQyIgLz48cGF0aCBkPSJNNjk4Ljg4IDg5MC44OGgtMzg0YzAtNjQgODUuNzYtMTE1LjIgMTkyLTExNS4yczE5MiA1MS4yIDE5MiAxMTUuMnoiIGZpbGw9IiNFRUJFMDAiIC8+PHBhdGggZD0iTTY5OC44OCA5MDMuNjhoLTM4NGMtNy42OCAwLTEyLjgtNS4xMi0xMi44LTEyLjggMC03MS42OCA4OS42LTEyOCAyMDQuOC0xMjhzMjA0LjggNTYuMzIgMjA0LjggMTI4YzAgNy42OC01LjEyIDEyLjgtMTIuOCAxMi44eiBtLTM2OS45Mi0yNS42aDM1NS44NGMtMTEuNTItNDkuOTItODguMzItODkuNi0xNzcuOTItODkuNnMtMTY2LjQgMzkuNjgtMTc3LjkyIDg5LjZ6IiBmaWxsPSIjMjMxQzFDIiAvPjxwYXRoIGQ9Ik00OTUuMzYgNjg2LjA4aDI1LjZWNzgwLjhoLTI1LjZ6TTMyNy42OCA0MTcuMjhoLTI1LjZjLTcuNjggMC0xMi44LTUuMTItMTIuOC0xMi44di03Ni44YzAtNy42OCA1LjEyLTEyLjggMTIuOC0xMi44aDI1LjZjNy42OCAwIDEyLjggNS4xMiAxMi44IDEyLjh2NzYuOGMwIDYuNC01LjEyIDEyLjgtMTIuOCAxMi44ek03MTEuNjggNDE3LjI4aC0yNS42Yy03LjY4IDAtMTIuOC01LjEyLTEyLjgtMTIuOHYtNzYuOGMwLTcuNjggNS4xMi0xMi44IDEyLjgtMTIuOGgyNS42YzcuNjggMCAxMi44IDUuMTIgMTIuOCAxMi44djc2LjhjMCA2LjQtNS4xMiAxMi44LTEyLjggMTIuOHpNMzY2LjA4IDE5OS42OGg4OS42djI1LjZoLTg5LjZ6TTM3OC44OCAxNDguNDhoNzYuOHYyNS42aC03Ni44ek01NTguMDggMTQ4LjQ4aDc2Ljh2MjUuNmgtNzYuOHpNNTU4LjA4IDE5OS42OGg4OS42djI1LjZoLTg5LjZ6TTU1OC4wOCA0NTUuNjhoODkuNnYyNS42aC04OS42eiIgZmlsbD0iIzIzMUMxQyIgLz48cGF0aCBkPSJNNTU4LjA4IDUwNi44OGg3Ni44djI1LjZoLTc2Ljh6IiBmaWxsPSIjMjMxQzFDIiAvPjxwYXRoIGQ9Ik0zNzguODggNTA2Ljg4aDc2Ljh2MjUuNmgtNzYuOHoiIGZpbGw9IiMyMzFDMUMiIC8+PHBhdGggZD0iTTM2Ni4wOCA0NTUuNjhoODkuNnYyNS42aC04OS42eiIgZmlsbD0iIzIzMUMxQyIgLz48L3N2Zz4=</url>
        			<link>https://www.svgrepo.com/svg/501812/microphone-broadcast</link>
        			<title>Podcast Rewind Image</title>
        		</image>
        		<itunes:new-feed-url>https://github.com/dougwaldron/podcast-rewind</itunes:new-feed-url>
        
        		<item>
        			<title>Title of pretend episode 3</title>
        			<itunes:title>iTunes Title of pretend episode 3</itunes:title>
        			<pubDate>Mon, 18 Dec 2023 16:00:00 GMT</pubDate>
        			<itunes:duration>00:01</itunes:duration>
        			<enclosure url="https://example.com/media.mp3" length="1" type="audio/mpeg"/>
        			<guid isPermaLink="false">20000000-0000-0000-0000-000000000003</guid>
        			<itunes:explicit>no</itunes:explicit>
        			<link>https://example.com/podcast/episode 3</link>
        			<itunes:subtitle>Subtitle of sample podcast episode three</itunes:subtitle>
        			<itunes:episodeType>full</itunes:episodeType>
        			<itunes:summary><![CDATA[<p>This is the iTunes summary for episode 3.</p>]]></itunes:summary>
        			<content:encoded><![CDATA[<p>This is the "content:encoded" element for episode 3.</p>]]></content:encoded>
        		</item>
        
        		<item>
        			<title>Title of pretend episode 2</title>
        			<itunes:title>iTunes Title of pretend episode 2</itunes:title>
        			<pubDate>Mon, 11 Dec 2023 16:00:00 GMT</pubDate>
        			<itunes:duration>00:01</itunes:duration>
        			<enclosure url="https://example.com/media.mp3" length="1" type="audio/mpeg"/>
        			<guid isPermaLink="false">20000000-0000-0000-0000-000000000002</guid>
        			<itunes:explicit>no</itunes:explicit>
        			<link>https://example.com/podcast/episode 2</link>
        			<itunes:subtitle>Subtitle of sample podcast episode two</itunes:subtitle>
        			<itunes:episodeType>full</itunes:episodeType>
        			<description>This is the description for episode 2.</description>
        			<content:encoded><![CDATA[<p>This is the "content:encoded" element for episode 2.</p>]]></content:encoded>
        		</item>
        
        		<item>
        			<title>Title of pretend episode 1</title>
        			<itunes:title>iTunes Title of pretend episode 1</itunes:title>
        			<pubDate>Mon, 4 Dec 2023 16:00:00 GMT</pubDate>
        			<itunes:duration>00:01</itunes:duration>
        			<enclosure url="https://example.com/media.mp3" length="1" type="audio/mpeg"/>
        			<guid isPermaLink="false">20000000-0000-0000-0000-000000000001</guid>
        			<itunes:explicit>no</itunes:explicit>
        			<link>https://example.com/podcast/episode/1</link>
        			<itunes:subtitle>Subtitle of sample podcast episode one</itunes:subtitle>
        			<itunes:episodeType>full</itunes:episodeType>
        			<description>This is the description for episode 1.</description>
        			<itunes:summary><![CDATA[<p>This is the iTunes summary for episode 1.</p>]]></itunes:summary>
        		</item>
        
        	</channel>
        </rss>
        """;
}
