namespace KC.Application.Common.Rules
{
    public record RuleResult<T>(string WorkflowName, string RuleName, T? Output);
}
