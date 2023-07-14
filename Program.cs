// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;


class Lisperanto
{
    static Dictionary<string, List<string>> arguments_needed_to_be_evaluated_for_function = new Dictionary<string, List<string>>();

    static List<EvaluationState> evaluation_stack = new List<EvaluationState>();

    static List<EvaluationState> recently_evaluated = new List<EvaluationState>();
    static async Task Main(string[] args)
    {
        var eval_res = await evaluate_file("sum-example.json");
        while(true)
        {
            var top = evaluation_stack.LastOrDefault();
            if (top == null)
            {
                System.Threading.Thread.Sleep(millisecondsTimeout: 100);
                continue;
            }
            evaluation_stack.Remove(top);
            evaluate_element(top);
            print_evaluation(top);

            var recent = recently_evaluated.LastOrDefault();
            if (recent != null && recent.parent != null)
            {
                evaluation_stack.Add(recent.parent);
                recently_evaluated.Remove(recent);
            }

            System.Threading.Thread.Sleep(millisecondsTimeout: 1);
        }
    }

    static void print_evaluation(EvaluationState state)
    {
        Console.WriteLine("-----");
        Console.WriteLine(state.starting_element.GetRawText());
        Console.WriteLine("::::");
        if (state.evaluation == null)
        {
            Console.WriteLine("Not yet evalueated");
        }
        else
        {
            foreach(var kvp in state.evaluation)
            {
                Console.WriteLine($"{kvp.Key} :: {kvp.Value}");
            }
        }
        
    }


static Dictionary<string, object> evaluate_plus_decimal(EvaluationState state)
{
    var result = new Dictionary<string, object>();
    result["type"] = "decimal-result";
    decimal accumulator = 0;
    bool partially_evaluated = false;
    result["result"] = accumulator;
    foreach(var element in state.dependencies)
    {
        if (element.evaluation == null)
        {
            partially_evaluated = true;
            continue;
        }
        var possibly_decimal_result = element.evaluation!;
        if ( (possibly_decimal_result["type"] as string) == "decimal-result" )
        {
            accumulator += (decimal) possibly_decimal_result["result"];
            result["result"] = accumulator;
            continue;
        }
        if ( (possibly_decimal_result["type"] as string) == "failed-parsing-of-decimal" )
        {
            result["type"] = "partially-failed-plus-evaluation";
            if (!result.ContainsKey("failed-parsing-for"))
            {
                result["failed-parsing-for"] = new List<string?>();
            }
            (result["failed-parsing-for"] as List<string?>)?.Add(possibly_decimal_result["raw"] as string);
            partially_evaluated = true;
            continue;
        }
    }
    result["partially-evaluated"] = partially_evaluated;
    return result;
}

static void evaluate_element(EvaluationState state)
{
    
    var element = state.starting_element;
    if (element.ValueKind == JsonValueKind.Number)
    {
        state.evaluation = new Dictionary<string, object>();
        var raw = element.GetRawText();
        if (Decimal.TryParse(raw, System.Globalization.NumberStyles.Any, null, out decimal parsed))
        {
            state.evaluation["type"] = "decimal-result";
            state.evaluation["result"] = parsed;
        }
        else
        {
            state.evaluation["type"] = "failed-parsing-of-decimal";
            state.evaluation["raw"] = raw;
        }
        recently_evaluated.Add(state);
        return;
    }
    if (element.ValueKind == JsonValueKind.Object)
    {
        if (element.TryGetProperty("type", out JsonElement typeElement))
        {
            if(typeElement.GetString() == "function-call")
            {
                if (state.dependencies_were_expanded)
                {
                    //if (element.TryGetProperty("function-object", out JsonElement function_object)
                    var result = evaluate_plus_decimal(state);
                    state.evaluation = result;
                    recently_evaluated.Add(state);
                    return;
                }
                else
                {
                    evaluation_stack.Add(state);
                    if(element.TryGetProperty("arguments", out JsonElement argumentsElement))
                    {
                        // TODO arguments can be evaluated in parallel
                        var arguments_for_function_call = argumentsElement.EnumerateArray().ToList();
                        var dependencies = arguments_for_function_call.Select(argument => new EvaluationState
                        {
                            dependencies_were_expanded = false,
                            starting_element = argument,
                            parent = state
                        }).ToList(); // before ToList this was IEnumerable and produced two collections :'(  beginner mistake...
                        evaluation_stack.AddRange(dependencies);
                        state.dependencies.AddRange(dependencies);
                    }
                    state.dependencies_were_expanded = true;
                    return;

                }
                
            }
        }
    }
    return;
}

    static async Task<JsonElement> evaluate_file(string path)
    {
        using(var str = new StreamReader(path))
        {
            var result = await JsonSerializer.DeserializeAsync<JsonElement>(str.BaseStream);

            evaluate_element(new EvaluationState
            {
                dependencies_were_expanded = false,
                starting_element = result
            });
            return result;
        }
    }
}

class EvaluationState 
{
    public JsonElement starting_element;
    public bool dependencies_were_expanded = false;
    public List<EvaluationState> dependencies = new List<EvaluationState>();
    public Dictionary<string, object> evaluation;
    public EvaluationState parent;
    Dictionary<string, Dictionary<string, object>> captured_variables = new Dictionary<string, Dictionary<string, object>>();

    public override string ToString()
    {
        return starting_element.GetRawText();
    }

    
}