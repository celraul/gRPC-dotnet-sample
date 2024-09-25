using Grpc.Net.Client;
using GrpcVideoStreaming;

using var channel = GrpcChannel.ForAddress("https://localhost:7155");
var client = new VideoStream.VideoStreamClient(channel);

using var videoStream = client.StreamVideo(new VideoRequest { VideoId = "sample" });

// Create a file to save the video stream
using var fileStream = new FileStream("downloaded_video.mp4", FileMode.Create, FileAccess.Write);

while (await videoStream.ResponseStream.MoveNext(CancellationToken.None))
{
    var videoChunk = videoStream.ResponseStream.Current;
    fileStream.Write(videoChunk.Content.ToByteArray(), 0, videoChunk.ChunkSize);
    Console.WriteLine($"Received chunk of size: {videoChunk.ChunkSize}");
}

Console.WriteLine("Video streaming complete.");