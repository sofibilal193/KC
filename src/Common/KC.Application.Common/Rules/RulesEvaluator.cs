using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RulesEngine.Models;

namespace KC.Application.Common.Rules
{
    public class RulesEvaluator : IRulesEvaluator
    {
        public async Task<IList<RuleResult<object>>> ExecuteRulesAsync(IEnumerable<Workflow> workflows, params RuleParameter[] inputParameters)
        {
            return await ExecuteRulesAsync<object>(workflows, inputParameters);
        }

        public async Task<IList<RuleResult<T>>> ExecuteRulesAsync<T>(IEnumerable<Workflow> workflows, params RuleParameter[] inputParameters)
        {
            var items = new List<RuleResult<T>>();
            var errors = new List<ValidationFailure>();
            var settings = new ReSettings
            {
                CustomActions = new Dictionary<string, Func<RulesEngine.Actions.ActionBase>>
                {
                    { "OutputJson", () => new OutputJsonAction() }
                }
            };
            var rulesEngine = new RulesEngine.RulesEngine(workflows.ToArray(), settings);
            foreach (var workflow in workflows)
            {
                var results = await rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName, inputParameters);
                foreach (var result in results)
                {
                    var rule = $"{workflow.WorkflowName}/{result.Rule.RuleName}";
                    if (!string.IsNullOrEmpty(result.ExceptionMessage))
                    {
                        errors.Add(new ValidationFailure(rule, result.ExceptionMessage));
                        continue;
                    }
                    if (result.ActionResult.Exception is not null)
                    {
                        errors.Add(new ValidationFailure(rule, result.ActionResult.Exception.Message));
                        continue;
                    }
                    if (result.ActionResult.Output is not null)
                    {
                        var actionName = (result.Rule.Actions.OnSuccess ?? result.Rule.Actions.OnFailure).Name;
                        if (actionName == "OutputExpression")
                        {
                            if (result.ActionResult.Output is T item)
                            {
                                items.Add(new RuleResult<T>(workflow.WorkflowName, result.Rule.RuleName, item));
                            }
                            else if (result.ActionResult.Output is IList<T> list)
                            {
                                items.AddRange(list.Select(i => new RuleResult<T>(
                                    workflow.WorkflowName, result.Rule.RuleName, i)));
                            }
                            else
                            {
                                errors.Add(new ValidationFailure(rule,
                                    $"Result could not be converted to type: '{typeof(T).FullName}'"));
                            }
                        }
                        else if (actionName == "OutputJson")
                        {
                            var json = result.ActionResult.Output.ToString();
                            if (json is not null)
                            {
                                try
                                {
                                    if (json.StartsWith('['))
                                    {
                                        var list = JsonSerializer.Deserialize<List<T>>(json);
                                        if (list is not null)
                                        {
                                            items.AddRange(list.Select(i => new RuleResult<T>(
                                                workflow.WorkflowName, result.Rule.RuleName, i)));
                                        }
                                    }
                                    else
                                    {
                                        var item = JsonSerializer.Deserialize<T>(json);
                                        if (item is not null)
                                        {
                                            items.Add(new RuleResult<T>(workflow.WorkflowName, result.Rule.RuleName, item));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errors.Add(new ValidationFailure(rule, ex.Message));
                                }
                            }
                        }
                    }
                }
            }
            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
            return items;
        }
    }
}
