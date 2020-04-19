using System;
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

        private static Video MakeTestVideo()
        {
            return new Video()
            {
                PageUrl = "page",
                VideoUrl = "testUrl",
                ImageUrl = "image",
                Title = "title",
                Description = "description",
                Category = "category",
                Author = "author",
                Publish = new DateTime(2020,04,01),
                Parsed = new DateTime(2020,04,01),
            };
        }
    }
}