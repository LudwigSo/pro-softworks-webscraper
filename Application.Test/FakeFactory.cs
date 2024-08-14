using Application.Ports;
using Domain.Model;
using Domain.Ports;
using FakeItEasy;

namespace Application.Test;

public static class FakeFactory
{
    public static IReadContext NewReadContext(List<Project>? returnedProjects = null)
    {
        returnedProjects ??= new();
        var fakeReadContext = A.Fake<IReadContext>();
        A.CallTo(() => fakeReadContext.Projects).Returns(returnedProjects.AsQueryable());
        return fakeReadContext;
    }
}