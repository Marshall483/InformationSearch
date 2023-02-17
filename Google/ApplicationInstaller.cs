using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Google.Scrapper.Abstractions;
using Google.Сrawler.Abstractions;
using Google.Сrawler.Implementations;

namespace Google;

public class ApplicationCastleInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(Component.For<ICrawler>().ImplementedBy<Crawler>());
        container.Register(Component.For<IScrapper>().ImplementedBy<Scrapper.Implementations.Scrapper>());
    }
}