using System;
using FluentAssertions;
using Haydabase.BackgroundTaskr.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DemoApp.Controllers
{
    public class DemoControllerTests
    {
        [Fact]
        public void CreateBackgroundTask_RunsTaskInBackground()
        {
            // Arrange
            var backgroundTaskr = new SynchronousBackgroundTaskr(
                services => services
                    .AddScoped<IDelayer, FakeDelayer>()
            );
            var controller = new DemoController(backgroundTaskr);
            var name = Guid.NewGuid().ToString();
            
            // Act
            var result = controller.CreateBackgroundTask(name);
            
            // Assert
            result.Should().BeOfType<OkResult>();
            var invocation = backgroundTaskr.Invocations.Should().ContainSingle().Subject;
            invocation.TaskName.Should().Be(name);
            invocation.Exception.Should().BeNull();
        }
        
        [Fact]
        public void CreateErroringBackgroundTask_RunsErroringTaskInBackground()
        {
            // Arrange
            var backgroundTaskr = new SynchronousBackgroundTaskr(
                services => services
                    .AddScoped<IDelayer, FakeDelayer>()
            );
            var controller = new DemoController(backgroundTaskr);
            var name = Guid.NewGuid().ToString();
            
            // Act
            var result = controller.CreateErroringBackgroundTask(name);
            
            // Assert
            result.Should().BeOfType<OkResult>();
            var invocation = backgroundTaskr.Invocations.Should().ContainSingle().Subject;
            invocation.TaskName.Should().Be(name);
            invocation.Exception.Should().BeOfType<Exception>().Which.Message.Should().Be("Test Erroring Background Task");
        }
        
        [Fact]
        public void CreateErroringBackgroundTask_WithMessage_RunsErroringTaskInBackground()
        {
            // Arrange
            var backgroundTaskr = new SynchronousBackgroundTaskr(
                services => services
                    .AddScoped<IDelayer, FakeDelayer>()
            );
            var controller = new DemoController(backgroundTaskr);
            var name = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            
            // Act
            var result = controller.CreateErroringBackgroundTask(name, message);
            
            // Assert
            result.Should().BeOfType<OkResult>();
            var invocation = backgroundTaskr.Invocations.Should().ContainSingle().Subject;
            invocation.TaskName.Should().Be(name);
            invocation.Exception.Should().BeOfType<Exception>().Which.Message.Should().Be(message);
        }
    }
}