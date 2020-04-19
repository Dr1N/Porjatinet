using System;
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

            Assert.Throws<ArgumentNullException>(() =>  repo.Add(null));
        }
        
        private static Video MakeTestVideo()
        {
            return new Video("testUrl")
            {
                PageUrl = "page",
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