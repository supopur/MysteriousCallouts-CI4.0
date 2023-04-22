using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MysteriousCallouts.HelperSystems
{
    internal abstract class DecisionNode
    {
        internal abstract string Evaluate(Dictionary<string, object> inputs);
    }

    internal class AttributeNode : DecisionNode
    {
        internal string attributeName;
        internal object attributeValue;
        internal DecisionNode trueBranch;
        internal DecisionNode falseBranch;

        internal AttributeNode(string name, object value, DecisionNode trueBranch, DecisionNode falseBranch)
        {
            attributeName = name;
            attributeValue = value;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }

        internal override string Evaluate(Dictionary<string, object> inputs)
        {
            object value;
            if (inputs.TryGetValue(attributeName, out value))
            {
                Logger.Normal("Evaluate() in DecisionTree.cs",$"{value.ToString()}");
                return value.Equals(attributeValue) ? trueBranch.Evaluate(inputs) : falseBranch.Evaluate(inputs);
            }
            else
            {
                Logger.Error("Evaluate() in DecisionTree.cs",$"Missing input value for attribute {attributeName}");
                throw new ArgumentException();
            }
        }
    }

    internal class DecisionLeaf : DecisionNode
    {
        private string decision;

        internal DecisionLeaf(string value)
        {
            decision = value;
        }

        internal override string Evaluate(Dictionary<string, object> inputs)
        {
            return decision;
        }
    }

}