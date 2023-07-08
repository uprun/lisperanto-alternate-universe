// start 2023-06-10 17:02 CET, Munich
using System.Reflection;
using System.Text.Json;


class TestClass
{
    static Dictionary<string, List<string>> arguments_needed_to_be_evaluated_for_function = new Dictionary<string, List<string>>();

    static List<EvaluationState> evaluation_stack = new List<EvaluationState>();
    static Dictionary<JsonElement, Dictionary<string, object>> evaluation_result = new Dictionary<JsonElement, Dictionary<string, object>>();
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
            var result = evaluate_element(top);
            print_evaluation(result);

            System.Threading.Thread.Sleep(millisecondsTimeout: 1);
        }
    }

    static void print_evaluation(Dictionary<string, object> eval_res)
    {
        foreach(var kvp in eval_res)
        {
            Console.WriteLine($"{kvp.Key} :: {kvp.Value}");
        }
    }


static Dictionary<string, object> evaluate_plus_decimal(EvaluationState state)
{
    var result = new Dictionary<string, object>();
    result["type"] = "decimal-result";
    decimal accumulator = 0;
    foreach(var element in state.dependencies)
    {
        var possibly_decimal_result = evaluation_result[element.starting_element];
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
            continue;
        }
    }
    return result;
}

static Dictionary<string, object> evaluate_element(EvaluationState state)
{
    
    var element = state.starting_element;
    if (element.ValueKind == JsonValueKind.Number)
    {
        var result = new Dictionary<string, object>();
        var raw = element.GetRawText();
        if (Decimal.TryParse(raw, System.Globalization.NumberStyles.Any, null, out decimal parsed))
        {
            result["type"] = "decimal-result";
            result["result"] = parsed;
        }
        else
        {
            result["type"] = "failed-parsing-of-decimal";
            result["raw"] = raw;
        }
        evaluation_result[element] = result;
        return result;
    }
    if (element.ValueKind == JsonValueKind.Object)
    {
        if (element.TryGetProperty("type", out JsonElement typeElement))
        {
            if(typeElement.GetString() == "function-call")
            {
                if (state.dependencies_were_expanded)
                {
                    var result = evaluate_plus_decimal(state);
                    evaluation_result[element] = result;
                    return result;
                }
                else
                {
                    evaluation_stack.Add(state);
                    if(element.TryGetProperty("arguments", out JsonElement argumentsElement))
                    {
                        var arguments_for_function_call = argumentsElement.EnumerateArray().ToList();
                        var dependencies = arguments_for_function_call.Select(argument => new EvaluationState
                        {
                            dependencies_were_expanded = false,
                            starting_element = argument
                        });
                        evaluation_stack.AddRange(dependencies);
                        state.dependencies.AddRange(dependencies);
                    }
                    state.dependencies_were_expanded = true;

                }
                
            }
        }
    }
    return new Dictionary<string, object>();


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
    
}



// List<string> prepare_transpiled(JsonElement key, bool is_inside_block)
// {
//     List<string> result = new List<string>();
//     if (key.ValueKind == JsonValueKind.Object)
//     {
//         var type = key.GetProperty("type").GetString();
//         if (type == "function-call")
//         {
//             result.AddRange(transpile_function_call(key, is_inside_block));
//             return result;
//         }
        
//         if (type == "code-block")
//         {
//             result.Add("{");
//             foreach(var inner_key in key.GetProperty("statements").EnumerateArray())
//             {
//                 result.AddRange(prepare_transpiled(inner_key, true).Select(q => "    " + q));

//             }
//             result.Add("}");
//             return result;
//         }
//         if (type == "function-definition")
//         {
//             result.AddRange(transpile_function_definition(key));
//             return result;
//         }
//         if (type == "return-statement")
//         {
//             result.Add("return");
//             result.AddRange(prepare_transpiled(key.GetProperty("return-expression"), false).Select(q => "    " + q));
//             result.Add(";");
//             return result;
//         }
//         if (type == "variable-declaration")
//         {
//             result.Add($"{key.GetProperty("variable-type").GetString()} {key.GetProperty("variable-name").GetString()} =");
//             result.AddRange(prepare_transpiled(key.GetProperty("value-expression"), false).Select(q => "    " + q));
//             result.Add(";");
//             return result;
//         }
//         if (type == "path-reference")
//         {
//             var path = key.GetProperty("path").EnumerateArray().ToList();
//             var transformed_path = path.Select( (q, i) => i == (path.Count - 1) ? q.GetString() : $"{q.GetString()}.");
//             result.AddRange(transformed_path);
//             return result;
//         }
//     }
//     result.Add(key.GetRawText());
//     return result;
// }

// List<string> transpile_function_definition(JsonElement key)
// {
//     List<string> result = new List<string>();
//     result.Add($"{key.GetProperty("return-type")} {key.GetProperty("name").GetString()}(");
//     var parameters = key.GetProperty("parameters").EnumerateArray().ToArray();
//     for (var index = 0; index < parameters.Length; index++)
//     {
//         var inner_key = parameters[index];
//         if (inner_key.ValueKind == JsonValueKind.Object
//             && inner_key.GetProperty("type").GetString() == "parameter-declaration"
//             )
//         {
//             var comma_possibly = index == parameters.Length - 1 ? "" : ",";
//             var parameter_type = inner_key.GetProperty("parameter-type").GetString();
//             var parameter_name = inner_key.GetProperty("parameter-name").GetString();
//             result.Add($"    {parameter_type} {parameter_name}{comma_possibly}");

//         }
//     }
//     result.Add(")");
//     result.AddRange(prepare_transpiled(key.GetProperty("body"), false));
//     return result;
// }

// List<string> transpile_function_call(JsonElement key, bool is_inside_block)
// {
//     var result = new List<string>();
//     result.AddRange(prepare_transpiled(key.GetProperty("function-object"), false));
//     // 2023-06-10 ~18:00 I can do evaluation as well as code generation
//     // 2023-06-10 18:34 but lets finish with generation first
//     var transformed_parameters = key.GetProperty("arguments").EnumerateArray().Select(q => prepare_transpiled(q, false)).ToArray();
//     var semicolon_possibly = is_inside_block? ";" : "";
//     result.Add($"(");
//     for(var i = 0; i < transformed_parameters.Length; i++)
//     {
//         var inner = transformed_parameters[i];
//         result.AddRange(inner.Select(q => "    " + q));
//         if ( i != transformed_parameters.Length - 1)
//         {
//             result[result.Count - 1] = result[result.Count - 1] + ",";
//         }
//     }
//     result.Add($"){semicolon_possibly}");
//     return result;
// }