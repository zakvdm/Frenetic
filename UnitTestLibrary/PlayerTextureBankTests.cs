using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Graphics;
using System;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTextureBankTests
    {
        [Test]
        public void CanBuildBank()
        {
            var stubContentManager = MockRepository.GenerateStub<IContentManager>();
            XnaTextureBank<TestTextures> playerTextureBank = new XnaTextureBank<TestTextures>(stubContentManager);

            stubContentManager.AssertWasCalled(x => x.Load<ITexture>(XnaTextureBank<TestTextures>.PlayerTextureDirectory + TestTextures.Texture1.ToString()));
            stubContentManager.AssertWasCalled(x => x.Load<ITexture>(XnaTextureBank<TestTextures>.PlayerTextureDirectory + TestTextures.Texture2.ToString()));
        }

        [Test]
        public void CanIndexTextureBank()
        {
            var stubContentManager = MockRepository.GenerateStub<IContentManager>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            stubContentManager.Stub(x => x.Load<ITexture>(XnaTextureBank<TestTextures>.PlayerTextureDirectory + TestTextures.Texture2.ToString())).Return(stubTexture);
            XnaTextureBank<TestTextures> playerTextureBank = new XnaTextureBank<TestTextures>(stubContentManager);

            Assert.AreEqual(stubTexture, playerTextureBank[TestTextures.Texture2]);
        }


        enum TestTextures
        {
            Texture1,
            Texture2
        }
    }
}
