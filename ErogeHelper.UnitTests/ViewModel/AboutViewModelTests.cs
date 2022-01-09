using Config.Net;
using ErogeHelper.Model.Repositories;
using ErogeHelper.Model.Repositories.Interface;
using ErogeHelper.ViewModel.Preference;
using NUnit.Framework;

namespace ErogeHelper.UnitTests.ViewModel
{
    public class AboutViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            var configRepo = new ConfigurationBuilder<IEHConfigRepository>()
                .UseInMemoryDictionary()
                .Build();
            var updateService = new UpdateService();

            _aboutViewModel = new AboutViewModel(
                ehConfigRepository: configRepo,
                updateService: updateService);
        }

        private AboutViewModel _aboutViewModel = null!;
    }
}
