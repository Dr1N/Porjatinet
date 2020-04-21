using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Model;
using Xunit;

namespace RepositoryTests
{
    public class CrudRepositoryTest
    {
        [Fact]
        public void AddVideo_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video = MakeTestVideo();

            // Act

            repo.Add(video);
            var result = repo.GetVideo(video.VideoUrl);

            // Assert

            Assert.Equal(video, result);
        }

        [Fact]
        public void AddSameVideo_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video1 = MakeTestVideo();
            var video2 = MakeTestVideo();

            // Act

            repo.Add(video1);
            repo.Add(video2);
            var result = repo.GetAllVideos();

            // Assert

            Assert.True(result.Count == 1);
            Assert.Equal(result.First(), video1);
        }

        [Fact]
        public void AddNull_Exception_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();

            // Act

            Assert.Throws<ArgumentNullException>(() => repo.Add(null));
        }

        [Fact]
        public void RemoveByVideo_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video = MakeTestVideo();
            repo.Add(video);

            // Act

            repo.Remove(video);
            var result = repo.GetVideo(video.VideoUrl);

            // Assert

            Assert.Null(result);
        }

        [Fact]
        public void RemoveByUrl_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video = MakeTestVideo();
            repo.Add(video);

            // Act

            repo.Remove(video.VideoUrl);
            var result = repo.GetVideo(video.VideoUrl);

            // Assert

            Assert.Null(result);
        }

        [Fact]
        public void ClearVideos_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            for (var i = 0; i < 10; i++)
            {
                var video = MakeTestVideo($"url_{i}");
                repo.Add(video);
            }

            // Act

            repo.Clear();
            var result = repo.GetAllVideos();

            // Assert

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void GetVideo_Success_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video = MakeTestVideo();
            repo.Add(video);

            // Act

            var result = repo.GetVideo(video.VideoUrl);

            // Assert

            Assert.Equal(result, video);
        }

        [Fact]
        public void GetVideo_Null_Test()
        {
            // Arrange

            var repo = new JsonVideoRepository();
            var video = MakeTestVideo();
            repo.Add(video);

            // Act

            var result = repo.GetVideo("unknown");

            // Assert

            Assert.Null(result);
        }

        [Fact]
        public void GetAllVideos_Success_Test()
        {
            // Arrange

            const int count = 10;
            var repo = new JsonVideoRepository();
            var videoList = new List<Video>(10);
            for (var i = 0; i < count; i++)
            {
                var video = MakeTestVideo($"url_{i}");
                repo.Add(video);
                videoList.Add(video);
            }

            // Act

            var result = repo.GetAllVideos();

            // Assert

            Assert.True(result.Count == count);
            Assert.True(result.All(v => videoList.Contains(v)));
        }

        private static Video MakeTestVideo(string url = "testUrl")
        {
            return new Video(url)
            {
                PostUrl = "page",
                ImageUrl = "image",
                Title = "title",
                Description = "description",
                Category = "category",
                Author = "author",
                Publish = new DateTime(2020, 04, 01),
                Parsed = new DateTime(2020, 04, 01),
            };
        }
    }
}