using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Models;

namespace KC.Application.Common.Rules
{
    public interface IRulesEvaluator
    {
        Task<IList<RuleResult<object>>> ExecuteRulesAsync(IEnumerable<Workflow> workflows, params RuleParameter[] inputParameters);

        Task<IList<RuleResult<T>>> ExecuteRulesAsync<T>(IEnumerable<Workflow> workflows, params RuleParameter[] inputParameters);
    }
}
