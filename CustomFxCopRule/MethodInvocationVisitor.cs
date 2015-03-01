using System.Collections;
using Microsoft.FxCop.Sdk;

namespace CustomFxCopRule
{
    public class MethodInvocationVisitor : BinaryReadOnlyVisitor
    {
        private IList collaborators = new ArrayList();
        private IList enviedTypes = new ArrayList(); 

        private TypeNode callingObjectType;

        public MethodInvocationVisitor(TypeNode callingObjectType)
        {
            this.callingObjectType = callingObjectType;
        }

        public IList EnviedTypes
        {
            get { return enviedTypes; }
        }

        public override void VisitMemberBinding(MemberBinding memberBinding)
        {
            base.VisitMemberBinding(memberBinding);
            var boundMember = memberBinding.BoundMember;
            if (boundMember.NodeType == NodeType.Method)
            {
                InspectCollaboratingType(memberBinding, boundMember);
            }
        }

        private void InspectCollaboratingType(MemberBinding memberBinding, Member boundMember)
        {
            var declaringType = boundMember.DeclaringType;
            if (declaringType != callingObjectType)
            {
                CheckForFeatureEnvy(declaringType);
            }
        }

        private void CheckForFeatureEnvy(TypeNode declaringType)
        {
            if (collaborators.Contains(declaringType))
            {
                enviedTypes.Add(declaringType);
            }
            collaborators.Add(declaringType);
        }
    }
}