using Grpc.Core;
using GrpcVideoStreaming;

namespace GrpcService.Services;

public class VideoStreamService : VideoStream.VideoStreamBase
{
    public override async Task StreamVideo(VideoRequest request, IServerStreamWriter<VideoChunk> responseStream, ServerCallContext context)
    {
        string videoPath = Path.Combine("Videos", $"{request.VideoId}.mp4"); // Assume videos are stored as mp4
        const int bufferSize = 64 * 1024; // 64KB chunk size
        byte[] buffer = new byte[bufferSize];

        using (var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read))
        {
            int bytesRead;
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Stream video chunk by chunk
                var chunk = new VideoChunk
                {
                    Content = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead),
                    ChunkSize = bytesRead
                };
                await responseStream.WriteAsync(chunk);

                // Optionally, delay the response to simulate real-time streaming (not needed for file transfer)
                await Task.Delay(50); // Simulate streaming speed
            }
        }
    }
}