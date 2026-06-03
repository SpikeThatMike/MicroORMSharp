namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public sealed class TransactionContextTests
    {
        [TestMethod]
        public void TransactionContext_DoesNotExposeCommitRollbackConnectionOrTransaction()
        {
            var methods = typeof(TransactionContext).GetMethods();
            var dapperParameterTypes = typeof(TransactionDapperWrapper)
                .GetMethods()
                .SelectMany(x => x.GetParameters())
                .Select(x => x.ParameterType)
                .ToList();

            Assert.IsFalse(methods.Any(x => x.Name == "Commit"), "TransactionContext should not expose Commit");
            Assert.IsFalse(methods.Any(x => x.Name == "Rollback"), "TransactionContext should not expose Rollback");
            Assert.IsFalse(dapperParameterTypes.Contains(typeof(System.Data.IDbConnection)), "TransactionDapperWrapper should not expose connection parameters");
            Assert.IsFalse(dapperParameterTypes.Contains(typeof(System.Data.IDbTransaction)), "TransactionDapperWrapper should not expose transaction parameters");
        }
    }
}
