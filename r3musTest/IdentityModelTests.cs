using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using r3mus.Infrastructure;
using r3mus.Models;

namespace r3musTest
{
    [TestClass]
    public class IdentityModelTests
    {
        [TestMethod]
        public void GetCharacterCorpOrAllianceId_Returns_AllianceId()
        {
            //  Arrange
            int result = 0;
            var mock = new Mock<IApplicationUser>();
            mock.Setup(m => m.GetCorpOrAllianceId(r3mus.Models.ApplicationUser.IDType.Alliance, 0, "")).Returns(result);
            //  Act
            IApplicationUser user = new ApplicationUser();
            //  Assert
        }
    }
}
