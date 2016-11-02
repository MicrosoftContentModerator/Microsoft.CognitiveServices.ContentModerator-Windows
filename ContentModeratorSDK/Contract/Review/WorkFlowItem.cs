using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.CognitiveServices.ContentModerator.Contract.Review
{
    public class WorkFlowItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Expression Expression { get; set; }
    }

    public abstract class Expression
    {
        public ExpressionType Type { get; set; }
    }

    public class Condition : Expression
    {
        public Condition()
        {
            this.Type = ExpressionType.Condition;
        }
        public string ConnectorName { get; set; }
        public string OutputName { get; set; }

        public ConditionOperator Operator { get; set; }

        public string Value { get; set; }
    }

    public class Combination : Expression
    {
        public Combination()
        {
            this.Type = ExpressionType.Combine;
        }
        public Expression Left { get; set; }

        public Expression Right { get; set; }

        public CombineCondition Combine { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CombineCondition
    {
        AND,
        OR
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConditionOperator
    {
        gt, //GreaterThan
        lt, //LessThan
        ge, //GreaterThanOrEqual
        le, //LessThanOrEqual
        eq //equal
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExpressionType
    {
        Condition,
        Combine
    }
}
