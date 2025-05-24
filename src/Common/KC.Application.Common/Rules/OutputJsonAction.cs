using System.Threading.Tasks;
using RulesEngine.Actions;
using RulesEngine.Models;

namespace KC.Application.Common.Rules
{
    public class OutputJsonAction : ActionBase
    {
        public override async ValueTask<object> Run(ActionContext context, RuleParameter[] ruleParameters)
        {
            return await ValueTask.FromResult(context.GetContext<object>("Output"));
        }
    }
}
