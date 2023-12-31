namespace PodcastRewind.TestData;

internal static partial class Data
{
    public static string SamplePodcastFeed =>
	    """
	    <?xml version="1.0" encoding="utf-8"?>
	    <?xml-stylesheet type="text/xsl" href="/global/feed/rss.xslt" ?>
	    <rss version="2.0"
	    	xmlns:atom="http://www.w3.org/2005/Atom"
	    	xmlns:googleplay="http://www.google.com/schemas/play-podcasts/1.0"
	    	xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd"
	    	xmlns:media="http://search.yahoo.com/mrss/">
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
	    			<url>https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=640&amp;h=640&amp;auto=format&amp;fit=crop</url>
	    			<link>https://unsplash.com/photos/bokeh-photography-of-condenser-microphone-Y20JJ_ddy9M</link>
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
	    			<description><![CDATA[<p>This is the description of a pretend episode of the pretend podcast feed used for testing.</p>]]></description>
	    			<itunes:summary><![CDATA[<p>This is the iTunes summary of a pretend episode of the pretend podcast feed used for testing.</p>]]></itunes:summary>
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
	    			<description><![CDATA[<p>This is the description of a pretend episode of the pretend podcast feed used for testing.</p>]]></description>
	    			<itunes:summary><![CDATA[<p>This is the iTunes summary of a pretend episode of the pretend podcast feed used for testing.</p>]]></itunes:summary>
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
	    			<description><![CDATA[<p>This is the description of a pretend episode of the pretend podcast feed used for testing.</p>]]></description>
	    			<itunes:summary><![CDATA[<p>This is the iTunes summary of a pretend episode of the pretend podcast feed used for testing.</p>]]></itunes:summary>
	    		</item>
	    
	    	</channel>
	    </rss>
	    """;
}
