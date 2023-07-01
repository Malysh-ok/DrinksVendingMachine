using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DomainTests;

[TestClass]
public class EntitiesTests
{
    [TestMethod]
    public void PurchaseTest()
    {
        var purchases = Purchase.GetEmptyPurchases();
    }
}