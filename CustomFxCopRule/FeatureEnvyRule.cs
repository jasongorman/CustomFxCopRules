using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FxCop.Sdk;

namespace CustomFxCopRule
{
    public class FeatureEnvyRule : BaseIntrospectionRule
    {
        public FeatureEnvyRule()
            : base("FeatureEnvyRule", "CustomFxCopRule.FeatureEnvyRuleMetadata", typeof(FeatureEnvyRule).Assembly)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            if (member.NodeType == NodeType.Method)
            {
                InspectMethodBody(member as Method);
            }
            return this.Problems;
        }

        private void InspectMethodBody(Method method)
        {
            if (method != null)
            {
                var visitor = VisitBodyStatements(method);
                CheckForFeatureEnvy(method, visitor);
            }
        }

        private MethodInvocationVisitor VisitBodyStatements(Method method)
        {
            MethodInvocationVisitor visitor = new MethodInvocationVisitor(method.DeclaringType);
            visitor.VisitStatements(method.Body.Statements);
            return visitor;
        }

        private void CheckForFeatureEnvy(Method method, MethodInvocationVisitor visitor)
        {
            var enviedTypes = visitor.EnviedTypes;
            if (enviedTypes.Count > 0)
            {
                AddFeatureEnvyProblem(method, enviedTypes);
            }
        }

        private void AddFeatureEnvyProblem(Method method, IList enviedTypes)
        {
            string enviedTypeNames = "";
            foreach (TypeNode type in enviedTypes)
            {
                enviedTypeNames += GetMemberName(type) + ", ";
            }
            this.Problems.Add(
                new Problem(new Resolution("Detected Feature Envy in {0} for types {1}",
                    new string[] {GetMemberName(method), enviedTypeNames})));
        }

        private string GetMemberName(Member member)
        {
            return member.Name.Name;
        }
    }
}
