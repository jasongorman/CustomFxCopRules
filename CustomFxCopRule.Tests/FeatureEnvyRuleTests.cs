using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FxCop.Sdk;
using NUnit.Framework;

namespace CustomFxCopRule.Tests
{
    [TestFixture]
    public class FeatureEnvyRuleTests
    {
        [Test]
        [TestCase("MethodThatMakesOneCallOnOneSupplier", 0)]
        [TestCase("MethodThatMakesTwoCallsOnOneSupplier", 1)]
        [TestCase("MethodThatMakesTwoCallsOnDifferentSuppliers", 0)]
        [TestCase("MethodThatCreatesObjectsThenMakesOneFeatureCall", 0)]
        [TestCase("MethodThatUsesSystemTypeFeatureMultipleTimes", 0)]
        public void MethodsThatMakeMultipleCallsToCollaboratorsHaveFeatureEnvy(string methodName, int expectedProblemCount)
        {
            FeatureEnvyRule rule = new FeatureEnvyRule();
            rule.Check(GetMemberToCheck(methodName));
            Assert.AreEqual(expectedProblemCount, GetProblemCount(rule));
        }

        private static int GetProblemCount(FeatureEnvyRule rule)
        {
            return rule.Problems.Count;
        }

        private Member GetMemberToCheck(string methodName)
        {
            Type type = typeof(Client);
            AssemblyNode assembly = AssemblyNode.GetAssembly(type.Module.Assembly.Location);
            TypeNode typeNode = assembly.GetType(GetIdentifier(type.Namespace), GetIdentifier(type.Name));
            Member methodToCheck = GetMethodByName(typeNode, methodName);
            return methodToCheck;
        }

        private static Identifier GetIdentifier(string name)
        {
            return Identifier.For(name);
        }

        private Member GetMethodByName(TypeNode typeNode, string methodName)
        {
            foreach (Member member in typeNode.Members)
            {
                if (member.Name.Name == methodName)
                {
                    return member;
                }
            }
            return null;
        }
    }

    internal class Client
    {
        private SupplierA supplierA = new SupplierA();
        private SupplierB supplierB = new SupplierB();

        public void MethodThatMakesOneCallOnOneSupplier()
        {
            supplierA.FeatureA();
        }

        public void MethodThatMakesTwoCallsOnOneSupplier()
        {
            supplierA.FeatureA();
            supplierA.FeatureB();
        }

        public void MethodThatMakesTwoCallsOnDifferentSuppliers()
        {
            supplierA.FeatureA();
            supplierB.FeatureA();
        }

        public void MethodThatCreatesObjectsThenMakesOneFeatureCall()
        {
            SupplierA anotherSupplierA = new SupplierA();
            supplierA.FeatureA();
        }

        public void MethodThatUsesSystemTypeFeatureMultipleTimes()
        {
            DateTime now = DateTime.Now;
            now = DateTime.Now;
        }
    }

    internal class SupplierA
    {
        internal void FeatureA()
        {
        }

        internal void FeatureB()
        {
        }
    }

    internal class SupplierB
    {
        internal void FeatureA()
        {
        }
    }
}
